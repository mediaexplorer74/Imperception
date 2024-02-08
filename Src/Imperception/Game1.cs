// Game1

using GameManager.Maps;
using GameManager.Sprites;
using GameManager.Sprites.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

#nullable disable
namespace GameManager
{
  public class Game1 : Game
  {
    private GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;
    Vector2 baseScreenSize = new Vector2(800, 480);
    private Matrix globalTransformation;
    int backbufferWidth, backbufferHeight;


    public static Game1.GamePhase gamePhase;
    public static Game1.GameState gameState;
    private Camera camera;
    private ScreenManager screenManager;
    private SpriteFont spriteFont;
    private RenderTarget2D renderTarget;
    private Player player;
    private Monster monster;
    private List<Map> mapList;


        // Game1 constructor
        public Game1()
    {
             graphics = new GraphicsDeviceManager(this);

#if WINDOWS_PHONE
            TargetElapsedTime = TimeSpan.FromTicks(333333);
#endif
            graphics.IsFullScreen = true;//true;//set it true for W10M

            //RnD
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 480;

            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft
                | DisplayOrientation.LandscapeRight;// | DisplayOrientation.Portrait;
        }//Game1


        // Initialize
        protected override void Initialize()
        {
            this.camera = new Camera(this.graphics.PreferredBackBufferWidth,
                this.graphics.PreferredBackBufferHeight);
            Game1.gamePhase = Game1.GamePhase.HeadphonesRecommended;

            this.screenManager = new ScreenManager();
            this.renderTarget = new RenderTarget2D(this.GraphicsDevice, 800, 480, false,
                this.GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);

            this.mapList = new List<Map>();
            this.mapList.Add(new Map(Game1.Room.GirlBedroom, 445, "MapImages/girlbedroom4", 50, 713));

            this.mapList[0].doorList.Add((GameObject)new Door(
                Game1.Room.GirlBedroom, new Rectangle(185, 200, 88, 186), 1, 2));

            this.mapList[0].hidingSpotList.Add((GameObject)new HidingSpot(
                Game1.Room.GirlBedroom, new Rectangle(350, 330, 40, 72), "Inside Box", 100));

            this.mapList[0].hidingSpotList.Add((GameObject)new HidingSpot(
                Game1.Room.GirlBedroom, new Rectangle(670, 385, 50, 100), "Under the Bed", 100));

            this.mapList[0].itemList.Add((GameObject)new Item(
                Game1.Room.GirlBedroom, "Artifact", new Vector2(260f, 300f), 0.2f, "Art/artifact"));

            this.mapList.Add(
                new Map(Game1.Room.Corridor, 465, "MapImages/corridor", 50, 1130));

            this.mapList[1].doorList.Add(
                (GameObject)new Door(Game1.Room.Corridor, new Rectangle(770, 298, 95, 147), 2, 1));

            this.mapList[1].doorList.Add(
                (GameObject)new Door(Game1.Room.Corridor, new Rectangle(100, 200, 140, 260), 3, 4));

            this.mapList.Add(new Map(Game1.Room.ParentsRoom, 360, "MapImages/parentsroom", 80, 630));

            this.mapList[2].doorList.Add(
                (GameObject)new Door(Game1.Room.ParentsRoom, new Rectangle(250, 100, 90, 180), 4, 3));

            foreach (Map map1 in this.mapList)
            {
                foreach (Door door1 in map1.doorList)
                {
                    door1.doorGroundPosition = map1.groundPosition;
                    foreach (Map map2 in this.mapList)
                    {
                        foreach (Door door2 in map2.doorList)
                        {
                            if (door1.doorId == door2.goesToId)
                            {
                                door2.doorGoesToThisPosition.Y = (float)door1.doorGroundPosition;
                                door1.doorGoesToThisPosition.X = door2.pos.X;
                                door1.doorGoesToThisRoom = door2.location;
                            }
                        }
                    }
                }
            }
            this.player = new Player();
            this.monster = new Monster();
            base.Initialize();
        }//Initialize


        // LoadContent
        protected override void LoadContent()
    {

      this.Content.RootDirectory = "Content";

      this.spriteFont = this.Content.Load<SpriteFont>("spritefont");
      this.spriteBatch = new SpriteBatch(this.GraphicsDevice);
      this.screenManager.LoadContent(this.Content);

        foreach (Map map in this.mapList)
        {
            map.LoadContent(this.Content);
        }
      
      foreach (Map map in this.mapList)
      {

        if (map.doorList.Count > 0)
        {

          foreach (GameObject door in map.doorList)
            door.LoadContent(this.Content, this.GraphicsDevice);
          foreach (HidingSpot hidingSpot in map.hidingSpotList)
            hidingSpot.LoadGraphics(this.GraphicsDevice);

          foreach (Item obj in map.itemList)
            obj.LoadContent(this.Content);
        }
      }

      this.player.LoadContent(this.Content, this.GraphicsDevice);

      this.monster.LoadContent(this.Content, this.GraphicsDevice);

      this.monster.pos.Y = (float) (this.mapList[0].groundPosition + 20);

      Player.pos.Y = (float) this.mapList[0].groundPosition;

      Player.pos.X = 500f;

        ScalePresentationArea();

            // virtualGamePad = new VirtualGamePad(baseScreenSize, 
            //globalTransformation, 
            //    Content.Load<Texture2D>("Sprites/VirtualControlArrow"));

            /*
             #if !__IOS__
            //Known issue that you get exceptions if you use Media PLayer while connected to your PC
            //See http://social.msdn.microsoft.com/Forums/en/windowsphone7series/thread/c8a243d2-d360-46b1-96bd-62b1ef268c66
            //Which means its impossible to test this from VS.
            //So we have to catch the exception and throw it away
            try
            {
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(Content.Load<Song>("Sounds/Music"));
            }
            catch { }
#endif
             
             */

        }//LoadContent


        // UnloadContent
        protected override void UnloadContent()
    {
    }//UnloadContent

        //!
        public void ScalePresentationArea()
        {
            //Work out how much we need to scale our graphics to fill the screen
            backbufferWidth = GraphicsDevice.PresentationParameters.BackBufferWidth - 0; // 40 - dirty hack for Astoria!
            backbufferHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;

            float horScaling = backbufferWidth / baseScreenSize.X;
            float verScaling = backbufferHeight / baseScreenSize.Y;

            Vector3 screenScalingFactor = new Vector3(horScaling, verScaling, 1);

            globalTransformation = Matrix.CreateScale(screenScalingFactor);

            System.Diagnostics.Debug.WriteLine("Screen Size - Width["
                + GraphicsDevice.PresentationParameters.BackBufferWidth + "] " +
                "Height [" + GraphicsDevice.PresentationParameters.BackBufferHeight + "]");
        }//

        

    // Update
    protected override void Update(GameTime gameTime)
    {
        // !
        //Confirm the screen has not been resized by the user
        if (backbufferHeight != GraphicsDevice.PresentationParameters.BackBufferHeight ||
            backbufferWidth != GraphicsDevice.PresentationParameters.BackBufferWidth)
        {
            ScalePresentationArea();
        }


        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
            || Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            this.Exit();
        }

      KeyboardState state = Keyboard.GetState();

      //RnD
      if (state.IsKeyDown(Keys.F2))
      {
        this.graphics.ToggleFullScreen();
      }
     
      if (Game1.gameState != Game1.GameState.Paused)
      {
        if (Game1.gamePhase == Game1.GamePhase.Game)
        {
          this.player.Update(gameTime, state);
          this.monster.Update(gameTime);
          this.monster.GetPlayerPosition(Player.pos, Player.playerAction);
        }

        foreach (Map map in this.mapList)
        {
          if (Player.location == map.location)
            Camera.maximumRightPan = map.mapTexture.Width;
        }
      }

      foreach (Map map in this.mapList)
      {
        if (Player.location == map.location)
        {
          if ((double) Player.pos.X < (double) map.mapLimitLeft)
            Player.pos.X = (float) map.mapLimitLeft;
          if ((double) Player.pos.X > (double) map.mapLimitRight)
            Player.pos.X = (float) map.mapLimitRight;
        }
      
        if (this.monster.location == map.location)
          this.monster.ConsiderLocationBoundaries(map.mapLimitLeft, map.mapLimitRight);
      }
      
      this.player.isFacingDoor = false;
      this.player.isFacingHidingSpot = false;
     
      foreach (Map map in this.mapList)
      {
        foreach (Door door in map.doorList)
        {
          if ( Player.location == door.location
                && this.player.hitbox.Intersects(door.hitbox)
                && (double) this.player.vel.X == 0.0
                && Game1.gameState != Game1.GameState.ChangingRoom )
          {
            this.player.isFacingDoor = true;

            if (state.IsKeyDown(Keys.W) || state.IsKeyDown(Keys.Up))
            {
              Debug.WriteLine("[i] Up!");

              Player.pendingLocation = door.doorGoesToThisRoom;
              Player.pendingNextDoorPosition.X = door.doorGoesToThisPosition.X;
              Player.pendingNextDoorPosition.Y = door.doorGoesToThisPosition.Y;
              Game1.gameState = Game1.GameState.ChangingRoom;
            }
          }
        }

        foreach (HidingSpot hidingSpot in map.hidingSpotList)
        {
          if (Player.location == hidingSpot.location 
                        && this.player.hitbox.Intersects(hidingSpot.hitbox)
                        && Player.playerAction == Player.PlayerAction.Normal 
                        && Game1.gameState == Game1.GameState.Active)
          {
            this.player.isFacingHidingSpot = true;
            if (state.IsKeyDown(Keys.E) && this.player.cooldownBeforeCanUseHidingSpot < 0)
            {
              this.player.Hide(hidingSpot.nameOfSpot, hidingSpot.posMiddleOfSpot);
              this.player.hidingEfficiencyPercentage = hidingSpot.hidingValue;
              this.player.cooldownBeforeExitingHidingSpot = 30;
            }
          }
        }
      }

      if (this.monster.location == Player.location)
      {
        this.monster.SharesRoomWithPlayer = true;

        //DEBUG
        this.monster.KnowsWherePlayerIs = false;//Player.playerAction != Player.PlayerAction.Hiding;
      }
      else
      {
        this.monster.SharesRoomWithPlayer = false;
        this.monster.KnowsWherePlayerIs = false;
      }

      foreach (Map map in this.mapList)
      {
        foreach (Door door in map.doorList)
        {
          if (door.location == this.monster.location
                        && this.monster.hitbox.Intersects(door.hitbox)
                        && this.monster.cooldownToEnterDoor < 0
                        && this.monster.location != Player.location)
          {
            this.monster.location = door.doorGoesToThisRoom;
            this.monster.pos.X = door.doorGoesToThisPosition.X;
            this.monster.pos.Y = door.doorGoesToThisPosition.Y;
            this.monster.cooldownToEnterDoor = 900;
            this.monster.cooldownToKill = 60;
          }
        }
      }

      // check "hit by monster"...
      if 
      (
        Player.location == this.monster.location
        && this.player.hitbox.Intersects(this.monster.hitbox)
        && Game1.gamePhase != Game1.GamePhase.GameOverSplatter
        && this.monster.cooldownToKill < 0
        && Player.playerAction == Player.PlayerAction.Normal
      )
      {
        //this.screenManager.HitByMonster();
        Debug.WriteLine("[!] screenManager.HitByMonster!");
      }

      if (Game1.gamePhase == Game1.GamePhase.GameOverSplatter)
      {
        this.monster.soundEffects[0].Dispose();
        this.screenManager.InitiateGameOver();

        if ((double)this.screenManager.posBlackScreenOnBlood.X > 8000.0)
        {
                    //Exit the game ?
            //this.Exit();
        }
      }

      this.screenManager.Update(state);
      this.camera.Update();
      Console.WriteLine((object) Player.pos);
      base.Update(gameTime);
    }//Update


    // Draw
    protected override void Draw(GameTime gameTime)
    {
       this.GraphicsDevice.Clear(Color.CornflowerBlue);

        //this.spriteBatch.Begin(SpriteSortMode.BackToFront, transformMatrix: new Matrix?(Camera.transform));
        this.spriteBatch.Begin(SpriteSortMode.BackToFront, null,
        null, null, null, null, globalTransformation);


       if (Game1.gamePhase == Game1.GamePhase.Game || Game1.gamePhase == Game1.GamePhase.GameOverSplatter)
      {
        foreach (Map map in this.mapList)
        {
          if (Player.location == map.location)
          {
            map.Draw(this.spriteBatch);
            foreach (GameObject hidingSpot in map.hidingSpotList)
              hidingSpot.Draw(this.spriteBatch);
          }
          if (map.doorList.Count > 0)
          {
            foreach (Door door in map.doorList)
            {
              if (Player.location == door.location)
                door.Draw(this.spriteBatch);
            }
          }
          if (map.itemList.Count > 0)
          {
            foreach (Item obj in map.itemList)
            {
              if (Player.location == obj.location)
                obj.Draw(this.spriteBatch);
            }
          }
        }
        this.player.Draw(this.spriteBatch);
        this.monster.Draw(this.spriteBatch);
      }

      this.spriteBatch.End();
      
      this.spriteBatch.Begin(SpriteSortMode.BackToFront, null, 
          null, null, null, null, globalTransformation);
      this.screenManager.Draw(this.spriteBatch);

      if (Game1.gamePhase == Game1.GamePhase.Game 
                && Player.location == Game1.Room.ParentsRoom)
      {
        this.spriteBatch.DrawString( this.spriteFont, 
            "Thank you for trying! Ran out of time to finish this demo due to our busy lives~\n" +
            "Theme: Footstep. A blind girl relying on the sound of footsteps (Stereo) " +
            "of a killer demon to survive. \n" +
            "Made by: Alexandre Laframboise, Sylvain Filiatrault & Ne La.\n" +
            "MonoGame Game Jam of 2018.", 
            new Vector2(3f, 0.0f),
            Color.White, 0.0f,
            new Vector2(0.0f, 0.0f),
            1f, SpriteEffects.None, 
            0.0f );

        if (this.monster.location == Game1.Room.ParentsRoom)
          this.spriteBatch.DrawString(this.spriteFont,
              "Oh crap! It's here! I did not put hiding spots here! Sorry!", 
              new Vector2(3f, 100f), Color.White, 0.0f, new Vector2(0.0f, 0.0f),
              1f, SpriteEffects.None, 
              0.0f);
      }
      this.spriteBatch.End();
      base.Draw(gameTime);
    }//Draw


    // GamePhase
    public enum GamePhase
    {
      SplashScreen,
      Title,
      HeadphonesRecommended,
      Intro,
      Game,
      GameOverSplatter,
      GameOverScreen,
      Win,
    }

    public enum GameState
    {
      Active,
      ChangingRoom,
      Paused,
    }

    public enum Room
    {
      GirlBedroom,
      Corridor,
      ParentsRoom,
    }
  }
}
