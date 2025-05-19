using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpineEditor.UI.UISystem
{
    /// <summary>
    /// UI标签控件，用于显示文本
    /// </summary>
    public class UILabel : UIElement
    {
        private string _text;
        private SpriteFont _font;
        private Color _textColor = Color.White;
        
        /// <summary>
        /// 获取或设置文本
        /// </summary>
        public string Text
        {
            get => _text;
            set => _text = value;
        }
        
        /// <summary>
        /// 获取或设置字体
        /// </summary>
        public SpriteFont Font
        {
            get => _font;
            set => _font = value;
        }
        
        /// <summary>
        /// 获取或设置文本颜色
        /// </summary>
        public Color TextColor
        {
            get => _textColor;
            set => _textColor = value;
        }
        
        /// <summary>
        /// 创建UI标签
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="font">字体</param>
        public UILabel(string text, SpriteFont font)
        {
            _text = text;
            _font = font;
            
            // 根据文本大小设置初始边界
            if (font != null)
            {
                Vector2 size = font.MeasureString(text);
                Bounds = new Rectangle(0, 0, (int)size.X, (int)size.Y);
            }
            else
            {
                Bounds = new Rectangle(0, 0, 100, 20);
            }
        }
        
        protected override void OnDraw(SpriteBatch spriteBatch)
        {
            if (!string.IsNullOrEmpty(_text) && _font != null)
            {
                spriteBatch.DrawString(_font, _text, new Vector2(Bounds.X, Bounds.Y), _textColor);
            }
        }
    }
}
