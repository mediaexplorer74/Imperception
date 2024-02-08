// ScreenManager

using GameManager.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

#nullable disable
namespace GameManager
{
  public class ScreenManager
  {
    private Texture2D texHeadphones;
    private Texture2D texIntro;
    private Texture2D texBlackScreen;
    private Texture2D texBloodSplat;
    public static List<SoundEffect> soundEffects;
    public int cooldownToPressEnter = 60;
    public bool fadingToWhite;
    public float fadeOfBlack;
    public int doorEnterCooldown = 40;
    public static bool requestGameOver;
    public Vector2 posBlackScreenOnBlood;

    public ScreenManager()
    {
      Game1.gamePhase = Game1.GamePhase.HeadphonesRecommended;
      this.posBlackScreenOnBlood = new Vector2(0.0f, 0.0f);
      this.fadeOfBlack = 0.0f;
      ScreenManager.soundEffects = new List<SoundEffect>();
    }

    public void LoadContent(ContentManager content)
    {
      this.texHeadphones = content.Load<Texture2D>("Art/headphones");
      this.texIntro = content.Load<Texture2D>("Art/story");
      this.texBlackScreen = content.Load<Texture2D>("Art/blackscreen");
      this.texBloodSplat = content.Load<Texture2D>("Art/bloodsplat");
      ScreenManager.soundEffects.Add(content.Load<SoundEffect>("Audio/bloodsplatsound"));
    }

    // Update
    public void Update(KeyboardState key)
    {
      if (/*key.IsKeyDown(Keys.Enter) && */ Game1.gamePhase == Game1.GamePhase.Intro
            && this.cooldownToPressEnter < 0)
      {
         Game1.gamePhase = Game1.GamePhase.Game;
         this.cooldownToPressEnter = 120;
      }

      if (/*key.IsKeyDown(Keys.Enter) && */Game1.gamePhase == Game1.GamePhase.HeadphonesRecommended
                && this.cooldownToPressEnter < 0)
      {
        Game1.gamePhase = Game1.GamePhase.Intro;
        this.cooldownToPressEnter = 120;
      }


      if (Game1.gameState == Game1.GameState.ChangingRoom)
      {
            this.FadeForRoomChanging();
      }

      --this.cooldownToPressEnter;

      if (this.cooldownToPressEnter >= -5)
        return;

      this.cooldownToPressEnter = -1;

    }//Update



    // Draw
    public void Draw(SpriteBatch sb)
    {
     
        if (Game1.gamePhase == Game1.GamePhase.HeadphonesRecommended)
            sb.Draw(this.texHeadphones, new Vector2(0.0f, 0.0f), Color.White);
     
        if (Game1.gamePhase == Game1.GamePhase.Intro)
            sb.Draw(this.texIntro, new Vector2(0.0f, 0.0f), Color.White);
     
        if (Game1.gamePhase == Game1.GamePhase.Game)
            sb.Draw(this.texBlackScreen, new Vector2(0.0f, 0.0f), 
        new Rectangle?(), Color.White * 0.2f, 0.0f, new Vector2(0.0f, 0.0f), 1f, SpriteEffects.None, 0.1f);
            
        sb.Draw(this.texBlackScreen, new Vector2(0.0f, 0.0f), 
            new Rectangle?(), Color.White * this.fadeOfBlack, 0.0f, new Vector2(0.0f, 0.0f), 1f, 
            SpriteEffects.None, 0.1f);

        if (Game1.gamePhase != Game1.GamePhase.GameOverSplatter  || !ScreenManager.requestGameOver)
         return;
     
       sb.Draw(this.texBlackScreen, new Vector2(0.0f, 0.0f), new Rectangle?(), 
          Color.White, 0.0f, new Vector2(0.0f, 0.0f), 1f, SpriteEffects.None, 0.9f);
       sb.Draw(this.texBloodSplat, new Vector2(0.0f, 0.0f), new Rectangle?(),
          Color.White, 0.0f, new Vector2(0.0f, 0.0f), 1f, SpriteEffects.None, 0.8f);
       sb.Draw(this.texBlackScreen, this.posBlackScreenOnBlood, new Rectangle?(), 
          Color.White, 0.0f, new Vector2(0.0f, 0.0f), 1f, SpriteEffects.None, 0.7f);
    }//Draw


    // FadeForRoomChanging
    public void FadeForRoomChanging()
    {
      if (!this.fadingToWhite)
      {
        if ((double) this.fadeOfBlack <= 1.0)
          this.fadeOfBlack += 0.05f;

        if ((double) this.fadeOfBlack >= 1.0)
        {
          Player.location = Player.pendingLocation;
          Player.pos.X = Player.pendingNextDoorPosition.X;
          Player.pos.Y = Player.pendingNextDoorPosition.Y;
          Camera.FocusOnPlayer();
          this.fadingToWhite = true;
        }
      }

      if (!this.fadingToWhite)
        return;

      this.fadeOfBlack -= 0.05f;
      if ((double) this.fadeOfBlack >= 0.0)
        return;

      Game1.gameState = Game1.GameState.Active;
      this.fadingToWhite = false;
    }

    public void HitByMonster()
    {
      this.posBlackScreenOnBlood = new Vector2(0.0f, 0.0f);

      ScreenManager.requestGameOver = true;

      ScreenManager.soundEffects[0].Play(1f, 0.0f, 0.0f);

      Game1.gameState = Game1.GameState.Paused;

      Game1.gamePhase = Game1.GamePhase.GameOverSplatter;
    }


    public void InitiateGameOver()
    {
      this.posBlackScreenOnBlood.X += 50f;
      //Console.WriteLine((object) this.posBlackScreenOnBlood);
      Debug.WriteLine("[i] InitiateGameOver, posBlackScreenOnBlood = : " + this.posBlackScreenOnBlood.X);
    }
  }
}
