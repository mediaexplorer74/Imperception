// HidingSpot

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace GameManager.Sprites.GameObjects
{
  internal class HidingSpot : GameObject
  {
    public string nameOfSpot;
    public int hidingValue;
    public Game1.Room hidingSpotLocation;
    public int posMiddleOfSpot;

    public HidingSpot(
      Game1.Room hidingspotlocation,
      Rectangle hitBox,
      string nameofspot,
      int hidingvalue)
      : base(GameObject.ObjectType.HidingSpot)
    {
      this.hitbox = hitBox;
      this.nameOfSpot = nameofspot;
      this.hidingValue = hidingvalue;
      this.posMiddleOfSpot = this.hitbox.X + this.hitbox.Width / 2;
      this.hidingSpotLocation = hidingspotlocation;
    }

    public void LoadGraphics(GraphicsDevice graphicsDevice)
    {
      this.textureHitbox = new Texture2D(graphicsDevice, 1, 1);
      this.textureHitbox.SetData<Color>(new Color[1]
      {
        Color.White
      });
    }

    public override void Draw(SpriteBatch sb)
    {
    }
  }
}
