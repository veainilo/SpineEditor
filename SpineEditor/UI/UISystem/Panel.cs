using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpineEditor.UI.UISystem
{
    /// <summary>
    /// 面板控件，可以包含其他控件
    /// </summary>
    public class Panel : UIElement
    {
        public Color BackgroundColor { get; set; } = new Color(30, 30, 35);
        
        protected override void OnDraw(SpriteBatch spriteBatch)
        {
            // 绘制背景
            spriteBatch.Draw(TextureManager.Pixel, Bounds, BackgroundColor);
        }
    }
}
