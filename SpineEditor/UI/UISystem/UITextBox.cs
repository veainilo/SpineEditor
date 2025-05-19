using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpineEditor.UI.UISystem
{
    /// <summary>
    /// UI文本框控件，可以集成到布局系统中
    /// </summary>
    public class UITextBox : UIElement
    {
        private string _text = "";
        private string _label = "";
        private SpriteFont _font;
        private bool _isSelected;
        private float _cursorBlinkTime;
        private bool _showCursor;
        private KeyboardState _prevKeyboardState;
        
        /// <summary>
        /// 文本变更事件
        /// </summary>
        public event EventHandler TextChanged;
        
        /// <summary>
        /// 获取或设置文本
        /// </summary>
        public string Text
        {
            get => _text;
            set
            {
                if (_text != value)
                {
                    _text = value;
                    TextChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }
        
        /// <summary>
        /// 获取或设置标签
        /// </summary>
        public string Label
        {
            get => _label;
            set => _label = value;
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
        /// 创建UI文本框
        /// </summary>
        /// <param name="label">标签</param>
        /// <param name="text">文本</param>
        /// <param name="font">字体</param>
        public UITextBox(string label, string text, SpriteFont font)
        {
            _label = label;
            _text = text;
            _font = font;
            Bounds = new Rectangle(0, 0, 200, 30);
            _prevKeyboardState = Keyboard.GetState();
        }
        
        protected override bool OnMouseInput(MouseState mouseState, MouseState prevMouseState)
        {
            if (mouseState.LeftButton == ButtonState.Released && prevMouseState.LeftButton == ButtonState.Pressed)
            {
                _isSelected = true;
                return true;
            }
            
            return false;
        }
        
        protected override void OnUpdate(GameTime gameTime)
        {
            // 如果选中，处理键盘输入
            if (_isSelected)
            {
                // 更新光标闪烁
                _cursorBlinkTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_cursorBlinkTime > 0.5f)
                {
                    _cursorBlinkTime = 0;
                    _showCursor = !_showCursor;
                }
                
                // 处理键盘输入
                KeyboardState keyboardState = Keyboard.GetState();
                
                // 处理退格键
                if (keyboardState.IsKeyDown(Keys.Back) && !_prevKeyboardState.IsKeyDown(Keys.Back) && _text.Length > 0)
                {
                    Text = _text.Substring(0, _text.Length - 1);
                }
                
                // 处理回车键
                if (keyboardState.IsKeyDown(Keys.Enter) && !_prevKeyboardState.IsKeyDown(Keys.Enter))
                {
                    _isSelected = false;
                }
                
                // 处理字符输入
                foreach (Keys key in keyboardState.GetPressedKeys())
                {
                    if (!_prevKeyboardState.IsKeyDown(key))
                    {
                        char? c = KeyToChar(key, keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift));
                        if (c.HasValue)
                        {
                            Text += c.Value;
                        }
                    }
                }
                
                _prevKeyboardState = keyboardState;
            }
            else
            {
                _showCursor = false;
                _cursorBlinkTime = 0;
            }
        }
        
        protected override void OnDraw(SpriteBatch spriteBatch)
        {
            // 绘制标签
            if (!string.IsNullOrEmpty(_label) && _font != null)
            {
                Vector2 labelSize = _font.MeasureString(_label);
                spriteBatch.DrawString(_font, _label, new Vector2(Bounds.X, Bounds.Y - labelSize.Y - 2), Color.White);
            }
            
            // 绘制文本框背景
            Color backgroundColor = _isSelected ? new Color(60, 60, 70) : new Color(40, 40, 50);
            spriteBatch.Draw(TextureManager.Pixel, Bounds, backgroundColor);
            
            // 绘制边框
            Color borderColor = _isSelected ? Color.Yellow : new Color(100, 100, 120);
            DrawBorder(spriteBatch, Bounds, borderColor, 1);
            
            // 绘制文本
            if (_font != null)
            {
                // 计算文本位置
                Vector2 textPosition = new Vector2(Bounds.X + 5, Bounds.Y + (Bounds.Height - _font.MeasureString("A").Y) / 2);
                
                // 绘制文本
                spriteBatch.DrawString(_font, _text, textPosition, Color.White);
                
                // 如果选中且显示光标，绘制光标
                if (_isSelected && _showCursor)
                {
                    Vector2 cursorPosition = textPosition;
                    if (!string.IsNullOrEmpty(_text))
                    {
                        cursorPosition.X += _font.MeasureString(_text).X;
                    }
                    
                    spriteBatch.Draw(TextureManager.Pixel, new Rectangle((int)cursorPosition.X, (int)cursorPosition.Y, 1, (int)_font.MeasureString("A").Y), Color.White);
                }
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
        
        private char? KeyToChar(Keys key, bool shift)
        {
            // 数字键
            if (key >= Keys.D0 && key <= Keys.D9 && !shift)
            {
                return (char)('0' + (key - Keys.D0));
            }
            
            // 字母键
            if (key >= Keys.A && key <= Keys.Z)
            {
                return shift ? (char)('A' + (key - Keys.A)) : (char)('a' + (key - Keys.A));
            }
            
            // 空格键
            if (key == Keys.Space)
            {
                return ' ';
            }
            
            // 其他特殊字符
            switch (key)
            {
                case Keys.OemPeriod: return shift ? '>' : '.';
                case Keys.OemComma: return shift ? '<' : ',';
                case Keys.OemMinus: return shift ? '_' : '-';
                case Keys.OemPlus: return shift ? '+' : '=';
                default: return null;
            }
        }
    }
}
