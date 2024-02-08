// Door

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace GameManager.Sprites.GameObjects
{
  internal class Door : GameObject
  {
    public int doorId;
    public int goesToId;
    public int doorGroundPosition;
    public Vector2 doorGoesToThisPosition;
    public Game1.Room doorGoesToThisRoom;

    public Door(Game1.Room doorlocation, Rectangle doorhitbox, int thedoorid, int goestoid)
      : base(GameObject.ObjectType.Door)
    {
      this.hitbox = doorhitbox;
      this.pos = new Vector2((float) (doorhitbox.X + doorhitbox.Width / 2), (float) (doorhitbox.Y + doorhitbox.Height / 2));
      this.doorId = thedoorid;
      this.goesToId = goestoid;
      this.location = doorlocation;
    }

    public override void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
    {
      this.textureHitbox = new Texture2D(graphicsDevice, 1, 1);
      this.textureHitbox.SetData<Color>(new Color[1]
      {
        Color.White
      });
      base.LoadContent(content, graphicsDevice);
    }

    public override void Update(GameTime gameTime)
    {
    }

    public override void Draw(SpriteBatch sb) => base.Draw(sb);
  }
}
