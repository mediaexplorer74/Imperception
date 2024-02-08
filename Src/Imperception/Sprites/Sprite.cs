// Sprite

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace GameManager.Sprites
{
  public abstract class Sprite
  {
    public Sprite.SpriteType spriteType;
    public Texture2D texture;
    public Texture2D textureHitbox;
    public Vector2 pos;
    public Vector2 vel;
    public Vector2 accel;
    public Vector2 origin;
    public Rectangle hitbox;
    public float scale;
    public Game1.Room location;

    public Sprite(Sprite.SpriteType typeOfSprite) => this.spriteType = typeOfSprite;

    public enum SpriteType
    {
      Player,
      Monster,
      Object,
      Effect,
    }
  }
}
