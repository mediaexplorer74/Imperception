// Monster

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

#nullable disable
namespace GameManager.Sprites
{
  public class Monster : Sprite
  {
    private Texture2D texFaceless1;
    private Texture2D texFaceless2;
    private int cooldownAnimation = 140;
    public Monster.WalkingDirection walkingDirection;
    public List<SoundEffect> soundEffects;
    public Vector2 PreyPosition;
    public int distanceLimitLeft;
    public int distanceLimitRight;
    public SpriteFont spriteFont;
    public int StepCooldown;
    public bool SharesRoomWithPlayer;
    public bool KnowsWherePlayerIs;
    public bool KnowsWherePlayerHides;
    public int cooldownToEnterDoor;
    public int cooldownToKill;

    public Monster()
      : base(Sprite.SpriteType.Monster)
    {
      this.location = Game1.Room.Corridor;
      this.vel = new Vector2(0.0f, 0.0f);
      this.StepCooldown = 0;
      this.SharesRoomWithPlayer = false;
      this.scale = 0.25f;
      this.cooldownToEnterDoor = 60;
      this.walkingDirection = Monster.WalkingDirection.Right;
      this.pos.X = 300f;
      this.soundEffects = new List<SoundEffect>();
    }

    public void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
    {
      this.texFaceless1 = content.Load<Texture2D>("Art/faceless3");
      this.texFaceless2 = content.Load<Texture2D>("Art/faceless4");
      this.texture = this.texFaceless1;
      this.soundEffects.Add(content.Load<SoundEffect>("Audio/foot"));
      this.spriteFont = content.Load<SpriteFont>("spritefont");
      this.origin = new Vector2((float) (this.texture.Width / 2), (float) this.texture.Height);
      this.hitbox = new Rectangle((int) ((double) this.pos.X - (double) (this.texture.Width / 2)), (int) ((double) this.pos.Y - (double) this.texture.Height), this.texture.Width, this.texture.Height);
      this.textureHitbox = new Texture2D(graphicsDevice, 1, 1);
      this.textureHitbox.SetData<Color>(new Color[1]
      {
        Color.White
      });
    }

    public void Update(GameTime gameTime)
    {
      this.WalkingAnimation();
      --this.StepCooldown;
      if (this.StepCooldown < 0)
      {
        this.TakeStep();
        this.StepCooldown = 140;
      }
      this.hitbox = new Rectangle((int) this.pos.X - (int) ((double) this.texture.Width * (double) this.scale / 4.0), (int) this.pos.Y - (int) ((double) this.texture.Height * (double) this.scale), (int) ((double) (this.texture.Width / 2) * (double) this.scale), (int) ((double) this.texture.Height * (double) this.scale));
      if (this.cooldownToEnterDoor > -5)
        --this.cooldownToEnterDoor;
      if (this.cooldownToKill > -5)
        --this.cooldownToKill;
      this.pos = this.pos + this.vel;
      this.vel = this.vel + this.accel;
    }

    public void Draw(SpriteBatch sb)
    {
      if (!this.SharesRoomWithPlayer)
        return;
      if (this.walkingDirection == Monster.WalkingDirection.Right)
        sb.Draw(this.texture, this.pos, new Rectangle?(), Color.White, 0.0f, this.origin, this.scale, SpriteEffects.FlipHorizontally, 0.4f);
      else
        sb.Draw(this.texture, this.pos, new Rectangle?(), Color.White, 0.0f, this.origin, this.scale, SpriteEffects.None, 0.4f);
    }

    public void GetPlayerPosition(Vector2 playerPosition, Player.PlayerAction playeraction)
    {
      this.PreyPosition = playerPosition;
    }

    public void TakeStep()
    {
      if (this.SharesRoomWithPlayer && !this.KnowsWherePlayerIs)
      {
        if (this.KnowsWherePlayerHides)
        {
          if ((double) this.pos.X <= (double) this.PreyPosition.X)
          {
            this.walkingDirection = Monster.WalkingDirection.Right;
            this.soundEffects[0].Play(1f, 0.0f, -0.9f);
            this.pos.X += 60f;
          }
          if ((double) this.pos.X <= (double) this.PreyPosition.X)
            return;
          this.walkingDirection = Monster.WalkingDirection.Left;
          this.soundEffects[0].Play(1f, 0.0f, 0.9f);
          this.pos.X -= 60f;
        }
        else
        {
          if (this.walkingDirection == Monster.WalkingDirection.Left)
            this.vel.X = -0.5f;
          if (this.walkingDirection == Monster.WalkingDirection.Right)
            this.vel.X = 0.5f;
          if ((double) this.pos.X >= (double) this.PreyPosition.X)
          {
            this.soundEffects[0].Play(1f, 0.0f, 0.9f);
          }
          else
          {
            if ((double) this.pos.X >= (double) this.PreyPosition.X)
              return;
            this.soundEffects[0].Play(1f, 0.0f, -0.9f);
          }
        }
      }
      else if (this.SharesRoomWithPlayer && this.KnowsWherePlayerIs)
      {
        if ((double) this.pos.X > (double) this.PreyPosition.X)
        {
          this.walkingDirection = Monster.WalkingDirection.Left;
          this.soundEffects[0].Play(1f, 0.0f, 0.9f);
          this.vel.X = -0.5f;
        }
        if ((double) this.pos.X > (double) this.PreyPosition.X)
          return;
        this.walkingDirection = Monster.WalkingDirection.Right;
        this.soundEffects[0].Play(1f, 0.0f, -0.9f);
        this.vel.X = 0.5f;
      }
      else
      {
        if (!this.SharesRoomWithPlayer && this.KnowsWherePlayerIs || this.SharesRoomWithPlayer || this.KnowsWherePlayerIs)
          return;
        if (this.walkingDirection == Monster.WalkingDirection.Left)
        {
          this.soundEffects[0].Play(0.1f, 0.0f, 0.0f);
          this.vel.X = -0.5f;
        }
        else
        {
          this.soundEffects[0].Play(0.1f, 0.0f, 0.0f);
          this.vel.X = 0.5f;
        }
      }
    }

    public void ConsiderLocationBoundaries(int limitleft, int limitright)
    {
      this.distanceLimitLeft = limitleft;
      this.distanceLimitRight = limitright;
      if ((double) this.pos.X < (double) this.distanceLimitLeft)
      {
        this.pos.X = (float) this.distanceLimitLeft;
        this.walkingDirection = Monster.WalkingDirection.Right;
        this.vel.X *= -1f;
      }
      if ((double) this.pos.X <= (double) this.distanceLimitRight)
        return;
      this.pos.X = (float) this.distanceLimitRight;
      this.walkingDirection = Monster.WalkingDirection.Left;
      this.vel.X *= -1f;
    }

    public void ConsidersEnteringADoor()
    {
    }

    public void WalkingAnimation()
    {
      --this.cooldownAnimation;
      if (this.cooldownAnimation > 70)
        this.texture = this.texFaceless1;
      else
        this.texture = this.texFaceless2;
      if (this.cooldownAnimation >= 0)
        return;
      this.cooldownAnimation = 140;
    }

    public enum WalkingDirection
    {
      Left,
      Right,
    }
  }
}
