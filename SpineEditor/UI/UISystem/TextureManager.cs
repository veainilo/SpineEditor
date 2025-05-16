using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpineEditor.UI.UISystem
{
    /// <summary>
    /// 纹理管理器，提供共享纹理
    /// </summary>
    public static class TextureManager
    {
        public static Texture2D Pixel { get; private set; }
        
        public static void Initialize(GraphicsDevice graphicsDevice)
        {
            if (Pixel == null)
            {
                Pixel = new Texture2D(graphicsDevice, 1, 1);
                Pixel.SetData(new[] { Color.White });
            }
        }
    }
}
