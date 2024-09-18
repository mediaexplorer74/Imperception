// Player

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;


namespace GameManager.Sprites
{
  public class Player : Sprite
  {
    private Texture2D texVision;
    private Texture2D texActionKey;
    private Texture2D texActionKeyOne;
    private Texture2D texActionKeyTwo;
    private Texture2D texGirl1;
    private Texture2D texGirl2;
    private SpriteFont spriteFont;
    private Vector2 visionOrigin;
    private List<SoundEffect> soundEffects;
    public Player.FacingDirection facingDirection;
    public bool isWalking;
    private int cooldownWalkingAnimation;
    public new static Vector2 pos;
    public new static Game1.Room location;
    public static Game1.Room pendingLocation;
    public static Vector2 pendingNextDoorLocation;
    public static Vector2 pendingNextDoorPosition;
    public bool isFacingDoor;
    public bool isFacingHidingSpot;
    public static int mapRightSideLimit;
    public int facingDoorId;
    public int presence;
    public int actionKeyInterval = 30;
    public string nameOfHidingSpot;
    public Vector2 hidingSpotLabelDimensions;
    public int posOutsideHidingSpot;
    public int cooldownBeforeExitingHidingSpot;
    public int cooldownBeforeCanUseHidingSpot;
    public bool actionKeyWasReleased = true;
    public bool showHidingLocation;
    private float scaleOfHidingSpotLabel = 1.5f;
    private float hidingSpotLabelTranslation;
    private float hidingSpotLabelVisiblity = 1f;
    public int hidingEfficiencyPercentage;
    public static Player.PlayerAction playerAction;


    // Player
    public Player()
    : base(Sprite.SpriteType.Player)
    {
      Player.location = Game1.Room.GirlBedroom;
      this.presence = 100;
      this.isFacingDoor = false;
      this.scale = 0.21f;
      Player.pos.X = 0.0f;
      Player.pos.Y = 0.0f;
      this.soundEffects = new List<SoundEffect>();
    }

    // LoadContent
    public void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
    {
      this.texGirl1 = content.Load<Texture2D>("Art/girl6");
      this.texGirl2 = content.Load<Texture2D>("Art/girl7");
      this.texture = this.texGirl1;
      this.texVision = content.Load<Texture2D>("Art/peripheral");

      this.texActionKeyOne = content.Load<Texture2D>("Art/actionkey1");
      this.texActionKeyTwo = content.Load<Texture2D>("Art/actionkey2");
      this.texActionKey = this.texActionKeyOne;

      this.spriteFont = content.Load<SpriteFont>("spritefont");
      this.soundEffects.Add(content.Load<SoundEffect>("Audio/bloodsplatsound"));

      this.origin = new Vector2((float) (this.texture.Width / 2), (float) this.texture.Height);
      this.visionOrigin = new Vector2((float) (this.texVision.Width / 2), (float) (this.texVision.Height / 2));

      this.textureHitbox = new Texture2D(graphicsDevice, 1, 1);
      this.textureHitbox.SetData<Color>(new Color[1]
      {
        Color.White
      });
    }

    public void Update(GameTime gameTime, 
        KeyboardState keyboardState,
        GamePadState gamePadState)
    {
      if 
      (
          Game1.gameState == Game1.GameState.Active 
          && Player.playerAction == Player.PlayerAction.Normal
      )
      {
        // if A (Left) or D (Right) key up then stop walking
        if 
        ( keyboardState.IsKeyUp(Keys.A) || keyboardState.IsKeyUp(Keys.D) 
          || keyboardState.IsKeyUp(Keys.Left) || keyboardState.IsKeyUp(Keys.Right))
        {
          this.vel.X = 0.0f;
          this.isWalking = false;
        }

        // if key A (Left) pressed then go walking left
        if 
        (
           gamePadState.IsButtonDown(Buttons.DPadLeft) ||
           keyboardState.IsKeyDown(Keys.A) 
           || keyboardState.IsKeyDown(Keys.Left) 
        )
        {
          this.facingDirection = Player.FacingDirection.Left;
          this.vel.X = -1.3f;
          this.isWalking = true;
        }

        // // if key D (Right) pressed then go walking left
        if 
        (
            gamePadState.IsButtonDown(Buttons.DPadRight) ||
            keyboardState.IsKeyDown(Keys.D)
            || keyboardState.IsKeyDown(Keys.Right) 
        )
        {
          this.facingDirection = Player.FacingDirection.Right;
          this.vel.X = 1.3f;
          this.isWalking = true;
        }

        this.WalkingAnimation();

        if (keyboardState.IsKeyDown(Keys.S))
        {
            Debug.WriteLine("[i] S key is down (pressed)");
        }
                
        if (keyboardState.IsKeyDown(Keys.W))
        {
            Debug.WriteLine("[i] W key is down (pressed)");
        }
      }//if 

      --this.actionKeyInterval;
      if (this.actionKeyInterval < 0)
      {
        this.texActionKey = this.texActionKey != this.texActionKeyOne
                    ? this.texActionKeyOne
                    : this.texActionKeyTwo;

        //RnD
        this.actionKeyInterval = 40; //40
      }

      // if Key E down (pressed)...
      if ( Player.playerAction == Player.PlayerAction.Hiding
                && 
                (
                 keyboardState.IsKeyDown(Keys.E)
                 ||
                 gamePadState.IsButtonDown(Buttons.B)
                 )
                && this.cooldownBeforeExitingHidingSpot < 0 
                && this.actionKeyWasReleased )
      {
        //RnD
        this.cooldownBeforeCanUseHidingSpot = 30;//30

        this.showHidingLocation = false;
        this.hidingSpotLabelTranslation = 0.0f;
        this.hidingSpotLabelVisiblity = 1f;
        this.hidingEfficiencyPercentage = 0;
        Player.playerAction = Player.PlayerAction.Normal;
      }

      if (keyboardState.IsKeyUp(Keys.E))
      {
        this.actionKeyWasReleased = true;
      }

      this.hitbox = new Rectangle((int) Player.pos.X - 
          (int) ( this.texture.Width * this.scale / 8.0),
          (int) Player.pos.Y - (int) ( this.texture.Height * this.scale),
          (int) ((this.texture.Width / 4) * this.scale),
          (int) ( this.texture.Height *  this.scale));

      --this.cooldownBeforeExitingHidingSpot;

      if (this.cooldownBeforeExitingHidingSpot < -5)
        this.cooldownBeforeExitingHidingSpot = -1;
      
      --this.cooldownBeforeCanUseHidingSpot;
      
      if (this.cooldownBeforeCanUseHidingSpot < -5)
        this.cooldownBeforeCanUseHidingSpot = -1;

     if (this.showHidingLocation)
     {
        this.ShowHidingLocation();
     }

      Player.pos += this.vel;
      this.vel = this.vel + this.accel;
    }//Update


    // Draw
    public void Draw(SpriteBatch sb)
    {
      if (Player.playerAction == Player.PlayerAction.Normal)
      {
        if (this.texture == this.texGirl1)
        {

            if (this.facingDirection == Player.FacingDirection.Left)
            {
                sb.Draw(this.texture, new Vector2(Player.pos.X + 10f, Player.pos.Y),
                    new Rectangle?(), Color.White, 0.0f, this.origin, this.scale,
                    SpriteEffects.None, 0.5f);
            }

            if (this.facingDirection == Player.FacingDirection.Right)
            {
                sb.Draw(this.texture, new Vector2(Player.pos.X - 10f, Player.pos.Y),
                    new Rectangle?(), Color.White, 0.0f, this.origin, this.scale,
                    SpriteEffects.FlipHorizontally, 0.5f);
            }
        }

        if (this.texture == this.texGirl2)
        {
            if (this.facingDirection == Player.FacingDirection.Left)
            {
                sb.Draw(this.texture, new Vector2(Player.pos.X + 10f, Player.pos.Y + 1f),
                    new Rectangle?(), Color.White, 0.0f, this.origin, this.scale,
                    SpriteEffects.None, 0.5f);
            }

            if (this.facingDirection == Player.FacingDirection.Right)
            {
                sb.Draw(this.texture, new Vector2(Player.pos.X - 10f, Player.pos.Y + 1f),
                    new Rectangle?(), Color.White, 0.0f, this.origin, this.scale,
                    SpriteEffects.FlipHorizontally, 0.5f);
            }
        }
      }
      sb.Draw(this.texVision, new Vector2(Player.pos.X, (float) ((double) Player.pos.Y - 
          (double) (this.texture.Height / 2) * (double) this.scale - 50.0)),
          new Rectangle?(), Color.White, 0.0f, this.visionOrigin, 2.2f, SpriteEffects.None, 0.35f);

      sb.Draw(this.texVision, new Vector2(Player.pos.X, (float) ((double) Player.pos.Y -
          (double) (this.texture.Height / 2) * (double) this.scale - 50.0)), 
          new Rectangle?(), Color.White * 0.7f, 0.0f, this.visionOrigin, 2f, SpriteEffects.None, 0.34f);

      sb.Draw(this.texVision, new Vector2(Player.pos.X, (float) ((double) Player.pos.Y -
          (double) (this.texture.Height / 2) * (double) this.scale - 50.0)), 
          new Rectangle?(), Color.White * 0.5f, 0.0f, this.visionOrigin, 1.5f, SpriteEffects.None, 0.33f);

            if (this.isFacingHidingSpot)
            {
                sb.Draw(this.texActionKey, new Vector2(Player.pos.X, (float)((double)Player.pos.Y
                    - 30.0 - (double)this.texture.Height * (double)this.scale - 10.0)),
                    new Rectangle?(), Color.White * 0.5f, 0.0f,
                    new Vector2((float)(this.texActionKey.Width / 2),
                    (float)(this.texActionKey.Height / 2)), 0.8f, SpriteEffects.None, 0.2f);
            }

      if (!this.showHidingLocation)
        return;

      string text1 = "(In Hiding)";
      string text2 = "Effect: " + (object) this.presence + "%";
      Vector2 vector2 = this.spriteFont.MeasureString(text1);

      this.spriteFont.MeasureString(text2);

      sb.DrawString(this.spriteFont, text1, new Vector2(Player.pos.X -
          (float) ((double) vector2.X * (double) this.scaleOfHidingSpotLabel / 2.0), 
          (float) ((double) Player.pos.Y - (double) this.texture.Height * (double) this.scale + 10.0)
          + this.hidingSpotLabelTranslation), Color.White * this.hidingSpotLabelVisiblity, 0.0f, 
          new Vector2(0.0f, 0.0f), this.scaleOfHidingSpotLabel, SpriteEffects.None, 0.0f);

      sb.DrawString(this.spriteFont, this.nameOfHidingSpot, 
          new Vector2(Player.pos.X - (float) ((double) this.hidingSpotLabelDimensions.X *
          (double) this.scaleOfHidingSpotLabel / 2.0), (float) ((double) Player.pos.Y - 
          (double) this.texture.Height * (double) this.scale + 40.0) + this.hidingSpotLabelTranslation),
          Color.White * this.hidingSpotLabelVisiblity, 0.0f, new Vector2(0.0f, 0.0f),
          this.scaleOfHidingSpotLabel,
          SpriteEffects.None, 0.0f);

    }//Draw


    // Hide
    public void Hide(string nameofhidingspot, int posoutsideofspot)
    {
      this.nameOfHidingSpot = nameofhidingspot;
      this.hidingSpotLabelDimensions = this.spriteFont.MeasureString(this.nameOfHidingSpot);
      this.posOutsideHidingSpot = posoutsideofspot;
      Player.pos.X = (float) this.posOutsideHidingSpot;
      this.vel.X = 0.0f;
      Player.playerAction = Player.PlayerAction.Hiding;
      this.actionKeyWasReleased = false;
      this.showHidingLocation = true;
    }//Hide


    // ShowHidingLocation
    public void ShowHidingLocation()
    {
      this.hidingSpotLabelTranslation -= 0.3f;

      if ((double) this.hidingSpotLabelTranslation < -10.0)
        this.hidingSpotLabelVisiblity -= 0.02f;

      if ((double) this.hidingSpotLabelVisiblity >= 0.0)
        return;

      this.showHidingLocation = false;
    }//ShowHidingLocation


    // WalkingAnimation
    public void WalkingAnimation()
    {
      if (this.isWalking)
      {
        --this.cooldownWalkingAnimation;

        if (this.cooldownWalkingAnimation > 15)
          this.texture = this.texGirl1;
        else
          this.texture = this.texGirl2;
        
        if (this.cooldownWalkingAnimation >= 0)
          return;
        this.cooldownWalkingAnimation = 30;
      }
      else
        this.cooldownWalkingAnimation = 30;

   }//WalkingAnimation



    public enum FacingDirection
    {
      Left,
      Right,
    }

    public enum PlayerAction
    {
      Normal,
      Hiding,
    }
  }
}
