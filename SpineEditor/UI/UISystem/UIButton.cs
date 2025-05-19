using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpineEditor.UI.UISystem
{
    /// <summary>
    /// UI按钮控件，可以集成到布局系统中
    /// </summary>
    public class UIButton : UIElement
    {
        private string _text;
        private SpriteFont _font;
        private bool _isHovered;
        private bool _isPressed;
        private Color _backgroundColor = new Color(60, 60, 70);
        private Color _hoverColor = new Color(80, 80, 100);
        private Color _pressedColor = new Color(40, 40, 50);
        private Color _textColor = Color.White;
        
        /// <summary>
        /// 点击事件
        /// </summary>
        public event EventHandler Click;
        
        /// <summary>
        /// 获取或设置按钮文本
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
        /// 创建UI按钮
        /// </summary>
        /// <param name="text">按钮文本</param>
        /// <param name="font">字体</param>
        public UIButton(string text, SpriteFont font)
        {
            _text = text;
            _font = font;
            Bounds = new Rectangle(0, 0, 100, 30);
        }
        
        protected override bool OnMouseInput(MouseState mouseState, MouseState prevMouseState)
        {
            _isHovered = true;
            
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                _isPressed = true;
            }
            else if (prevMouseState.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released)
            {
                if (_isPressed)
                {
                    // 触发点击事件
                    Click?.Invoke(this, EventArgs.Empty);
                }
                _isPressed = false;
            }
            
            return true; // 消费事件
        }
        
        protected override void OnUpdate(GameTime gameTime)
        {
            // 如果鼠标不在按钮上，重置状态
            if (!_isHovered)
            {
                _isPressed = false;
            }
            
            _isHovered = false; // 在下一帧重新检测
        }
        
        protected override void OnDraw(SpriteBatch spriteBatch)
        {
            // 选择背景颜色
            Color backgroundColor;
            if (_isPressed)
                backgroundColor = _pressedColor;
            else if (_isHovered)
                backgroundColor = _hoverColor;
            else
                backgroundColor = _backgroundColor;
            
            // 绘制背景
            spriteBatch.Draw(TextureManager.Pixel, Bounds, backgroundColor);
            
            // 绘制边框
            Color borderColor = _isHovered ? Color.Yellow : new Color(100, 100, 120);
            DrawBorder(spriteBatch, Bounds, borderColor, 1);
            
            // 绘制文本
            if (!string.IsNullOrEmpty(_text) && _font != null)
            {
                Vector2 textSize = _font.MeasureString(_text);
                Vector2 textPosition = new Vector2(
                    Bounds.X + (Bounds.Width - textSize.X) / 2,
                    Bounds.Y + (Bounds.Height - textSize.Y) / 2
                );
                
                spriteBatch.DrawString(_font, _text, textPosition, _textColor);
            }
        }
        
        private void DrawBorder(SpriteBatch spriteBatch, Rectangle rectangle, Color color, int thickness)
        {
            // 上边框
            spriteBatch.Draw(TextureManager.Pixel, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, thickness), color);
            // 下边框
            spriteBatch.Draw(TextureManager.Pixel, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height - thickness, rectangle.Width, thickness), color);
            // 左边框
            spriteBatch.Draw(TextureManager.Pixel, new Rectangle(rectangle.X, rectangle.Y, thickness, rectangle.Height), color);
            // 右边框
            spriteBatch.Draw(TextureManager.Pixel, new Rectangle(rectangle.X + rectangle.Width - thickness, rectangle.Y, thickness, rectangle.Height), color);
        }
    }
}
