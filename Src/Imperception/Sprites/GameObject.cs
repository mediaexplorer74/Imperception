// GameObject

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace GameManager.Sprites
{
  internal abstract class GameObject : Sprite
  {
    public GameObject.ObjectType objectType;

    public GameObject(GameObject.ObjectType typeofobject)
      : base(Sprite.SpriteType.Object)
    {
      this.objectType = typeofobject;
    }

    public virtual void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
    {
    }

    public virtual void Update(GameTime gameTime)
    {
    }

    public virtual void Draw(SpriteBatch sb)
    {
    }

    public enum ObjectType
    {
      Door,
      HidingSpot,
      Item,
    }
  }
}
