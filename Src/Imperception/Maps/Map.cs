// Map

using GameManager.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;


namespace GameManager.Maps
{
  internal class Map
  {
    public Texture2D mapTexture;
    public Game1.Room location;
    public int mapLimitLeft;
    public int mapLimitRight;
    private string mapPathName;
    public int groundPosition;
    public List<GameObject> doorList;
    public List<GameObject> hidingSpotList;
    public List<GameObject> itemList;

    public Map(
      Game1.Room nameofroom,
      int positionOfGround,
      string mapPath,
      int maplimitleft,
      int maplimitright)
    {
      this.mapPathName = mapPath;
      this.location = nameofroom;
      this.groundPosition = positionOfGround;
      this.mapLimitLeft = maplimitleft;
      this.mapLimitRight = maplimitright;
      this.doorList = new List<GameObject>();
      this.hidingSpotList = new List<GameObject>();
      this.itemList = new List<GameObject>();
    }

    public void LoadContent(ContentManager content)
    {
      this.mapTexture = content.Load<Texture2D>(this.mapPathName);
    }

    public void Draw(SpriteBatch sb)
    {
      sb.Draw(this.mapTexture, new Vector2(0.0f, 0.0f), new Rectangle?(), Color.White, 0.0f, new Vector2(0.0f, 0.0f), 1f, SpriteEffects.None, 0.9f);
    }
  }
}
