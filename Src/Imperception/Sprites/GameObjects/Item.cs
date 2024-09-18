// Item

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace GameManager.Sprites.GameObjects
{
  internal class Item : GameObject
  {
    public string itemName;
    public string assetPath;
    public bool isLooted;

    public Item(
      Game1.Room itemlocation,
      string itemname,
      Vector2 itemposition,
      float itemscale,
      string assetpath)
      : base(GameObject.ObjectType.Item)
    {
      this.location = itemlocation;
      this.itemName = itemname;
      this.pos = itemposition;
      this.assetPath = assetpath;
      this.scale = itemscale;
    }

    public void LoadContent(ContentManager content)
    {
      this.texture = content.Load<Texture2D>(this.assetPath);
      this.origin = new Vector2((float) (this.texture.Width / 2), (float) this.texture.Height);
      this.hitbox = new Rectangle((int) this.pos.X - (int) ((double) this.texture.Width * (double) this.scale), (int) this.pos.Y - (int) ((double) this.texture.Height * (double) this.scale), (int) ((double) this.texture.Width * (double) this.scale), (int) ((double) this.texture.Height * (double) this.scale));
    }

    public void Update(GraphicsDevice graphicsDevice)
    {
      this.textureHitbox = new Texture2D(graphicsDevice, 1, 1);
      this.textureHitbox.SetData<Color>(new Color[1]
      {
        Color.White
      });
    }

    public override void Draw(SpriteBatch sb)
    {
      if (this.isLooted)
        return;
      sb.Draw(this.texture, this.pos, new Rectangle?(), Color.White, 0.0f, new Vector2(0.0f, 0.0f), this.scale, SpriteEffects.None, 0.6f);
    }
  }
}
