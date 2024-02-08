// Camera

using GameManager.Sprites;
using Microsoft.Xna.Framework;

#nullable disable
namespace GameManager
{
  internal class Camera
  {
    public static Matrix transform;
    public static Vector2 camPosition;
    private static int screenWidth;
    private static int screenHeight;
    public static int maximumRightPan;

    public Camera(int screenwidth, int screenheight)
    {
      Camera.screenWidth = screenwidth;
      Camera.screenHeight = screenheight;
    }

    public void Update()
    {
      Camera.camPosition = Player.pos;
      Camera.transform = Matrix.CreateTranslation(new Vector3((float) (Camera.screenWidth / 2), 0.0f, 0.0f)) * Matrix.CreateTranslation(new Vector3(-Camera.camPosition.X, 0.0f, 0.0f));
      if ((double) Camera.camPosition.X <= (double) (Camera.screenWidth / 2))
        Camera.camPosition.X = (float) (Camera.screenWidth / 2);
      if ((double) Camera.camPosition.X >= (double) (Camera.maximumRightPan - Camera.screenWidth / 2))
        Camera.camPosition.X = (float) (Camera.maximumRightPan - Camera.screenWidth / 2);
      Camera.transform = Matrix.CreateTranslation(new Vector3((float) (Camera.screenWidth / 2), 0.0f, 0.0f)) * Matrix.CreateTranslation(new Vector3(-Camera.camPosition.X, 0.0f, 0.0f));
    }

    public static void FocusOnPlayer()
    {
      Camera.transform = Matrix.CreateTranslation(new Vector3((float) (Camera.screenWidth / 2), 0.0f, 0.0f)) * Matrix.CreateTranslation(new Vector3(-Camera.camPosition.X, 0.0f, 0.0f));
    }
  }
}
