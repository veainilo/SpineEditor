using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpineEditor.UI
{
    /// <summary>
    /// 按钮控件
    /// </summary>
    public class Button
    {
        private Rectangle _bounds;
        private string _text;
        private bool _isHovered;
        private bool _isPressed;
        private Texture2D _texture;
        private MouseState _prevMouseState;

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
        /// 获取或设置按钮边界
        /// </summary>
        public Rectangle Bounds
        {
            get => _bounds;
            set => _bounds = value;
        }

        /// <summary>
        /// 创建按钮
        /// </summary>
        /// <param name="graphicsDevice">图形设备</param>
        /// <param name="text">按钮文本</param>
        /// <param name="bounds">按钮边界</param>
        public Button(GraphicsDevice graphicsDevice, string text, Rectangle bounds)
        {
            _text = text;
            _bounds = bounds;
            _isHovered = false;
            _isPressed = false;

            // 创建按钮纹理
            _texture = new Texture2D(graphicsDevice, 1, 1);
            _texture.SetData(new[] { Color.White });

            _prevMouseState = Mouse.GetState();
        }

        /// <summary>
        /// 更新按钮状态
        /// </summary>
        public void Update()
        {
            MouseState mouseState = Mouse.GetState();

            // 检查鼠标是否悬停在按钮上
            _isHovered = _bounds.Contains(mouseState.Position);

            // 检查按钮是否被按下
            if (_isHovered)
            {
                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    _isPressed = true;
                }
                else if (_prevMouseState.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released)
                {
                    if (_isPressed)
                    {
                        // 触发点击事件
                        Click?.Invoke(this, EventArgs.Empty);
                    }
                    _isPressed = false;
                }
            }
            else
            {
                _isPressed = false;
            }

            _prevMouseState = mouseState;
        }

        /// <summary>
        /// 绘制按钮
        /// </summary>
        /// <param name="spriteBatch">精灵批处理</param>
        /// <param name="font">字体</param>
        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            // 根据按钮状态选择颜色 - 使用更鲜明的颜色
            Color color;
            if (_isPressed)
                color = new Color(80, 80, 120); // 按下时使用深蓝色
            else if (_isHovered)
                color = new Color(100, 100, 180); // 悬停时使用蓝色
            else
                color = new Color(120, 120, 200); // 正常状态使用浅蓝色

            // 绘制按钮背景
            spriteBatch.Draw(_texture, _bounds, color);

            // 绘制按钮边框 - 使用更明显的边框
            Color borderColor = _isHovered ? Color.Yellow : new Color(180, 180, 255);
            DrawBorder(spriteBatch, _bounds, borderColor, 2); // 使用更粗的边框

            // 绘制按钮文本 - 使用更明显的文本
            if (!string.IsNullOrEmpty(_text))
            {
                Vector2 textSize = font.MeasureString(_text);
                Vector2 textPosition = new Vector2(
                    _bounds.X + (_bounds.Width - textSize.X) / 2,
                    _bounds.Y + (_bounds.Height - textSize.Y) / 2
                );

                // 绘制文本阴影以增强可读性
                spriteBatch.DrawString(font, _text, new Vector2(textPosition.X + 1, textPosition.Y + 1), new Color(0, 0, 0, 150));

                // 使用白色绘制文本
                spriteBatch.DrawString(font, _text, textPosition, Color.White);
            }
        }

        /// <summary>
        /// 绘制边框
        /// </summary>
        /// <param name="spriteBatch">精灵批处理</param>
        /// <param name="rectangle">矩形</param>
        /// <param name="color">颜色</param>
        /// <param name="thickness">厚度</param>
        private void DrawBorder(SpriteBatch spriteBatch, Rectangle rectangle, Color color, int thickness)
        {
            // 上边框
            spriteBatch.Draw(_texture, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, thickness), color);
            // 下边框
            spriteBatch.Draw(_texture, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height - thickness, rectangle.Width, thickness), color);
            // 左边框
            spriteBatch.Draw(_texture, new Rectangle(rectangle.X, rectangle.Y, thickness, rectangle.Height), color);
            // 右边框
            spriteBatch.Draw(_texture, new Rectangle(rectangle.X + rectangle.Width - thickness, rectangle.Y, thickness, rectangle.Height), color);
        }
    }

    /// <summary>
    /// 文本框控件
    /// </summary>
    public class TextBox
    {
        private Rectangle _bounds;
        private string _text;
        private string _label;
        private bool _isSelected;
        private bool _isHovered;
        private Texture2D _texture;
        private MouseState _prevMouseState;
        private KeyboardState _prevKeyboardState;
        private float _cursorBlinkTime;
        private bool _showCursor;
        private bool _visible = true;

        /// <summary>
        /// 获取或设置控件是否可见
        /// </summary>
        public bool Visible
        {
            get => _visible;
            set => _visible = value;
        }

        /// <summary>
        /// 获取或设置文本
        /// </summary>
        public string Text
        {
            get => _text;
            set => _text = value;
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
        /// 获取或设置边界
        /// </summary>
        public Rectangle Bounds
        {
            get => _bounds;
            set => _bounds = value;
        }

        /// <summary>
        /// 文本变更事件
        /// </summary>
        public event EventHandler TextChanged;

        /// <summary>
        /// 创建文本框
        /// </summary>
        /// <param name="graphicsDevice">图形设备</param>
        /// <param name="label">标签</param>
        /// <param name="text">文本</param>
        /// <param name="bounds">边界</param>
        public TextBox(GraphicsDevice graphicsDevice, string label, string text, Rectangle bounds)
        {
            _label = label;
            _text = text;
            _bounds = bounds;
            _isSelected = false;
            _isHovered = false;
            _showCursor = false;
            _cursorBlinkTime = 0;

            // 创建纹理
            _texture = new Texture2D(graphicsDevice, 1, 1);
            _texture.SetData(new[] { Color.White });

            _prevMouseState = Mouse.GetState();
            _prevKeyboardState = Keyboard.GetState();
        }

        /// <summary>
        /// 更新文本框状态
        /// </summary>
        /// <param name="gameTime">游戏时间</param>
        public void Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();
            KeyboardState keyboardState = Keyboard.GetState();

            // 检查鼠标是否悬停在文本框上
            _isHovered = _bounds.Contains(mouseState.Position);

            // 检查文本框是否被选中
            if (_isHovered && mouseState.LeftButton == ButtonState.Pressed && _prevMouseState.LeftButton == ButtonState.Released)
            {
                _isSelected = true;
            }
            else if (!_isHovered && mouseState.LeftButton == ButtonState.Pressed && _prevMouseState.LeftButton == ButtonState.Released)
            {
                _isSelected = false;
            }

            // 处理键盘输入
            if (_isSelected)
            {
                // 更新光标闪烁
                _cursorBlinkTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_cursorBlinkTime >= 0.5f)
                {
                    _cursorBlinkTime = 0;
                    _showCursor = !_showCursor;
                }

                // 处理退格键
                if (keyboardState.IsKeyDown(Keys.Back) && !_prevKeyboardState.IsKeyDown(Keys.Back) && _text.Length > 0)
                {
                    _text = _text.Substring(0, _text.Length - 1);
                    TextChanged?.Invoke(this, EventArgs.Empty);
                }

                // 处理字符输入
                foreach (Keys key in keyboardState.GetPressedKeys())
                {
                    if (!_prevKeyboardState.IsKeyDown(key))
                    {
                        char? c = KeyToChar(key, keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift));
                        if (c.HasValue)
                        {
                            _text += c.Value;
                            TextChanged?.Invoke(this, EventArgs.Empty);
                        }
                    }
                }
            }

            _prevMouseState = mouseState;
            _prevKeyboardState = keyboardState;
        }

        /// <summary>
        /// 绘制文本框
        /// </summary>
        /// <param name="spriteBatch">精灵批处理</param>
        /// <param name="font">字体</param>
        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            // 如果控件不可见，则不绘制
            if (!_visible)
                return;

            // 绘制标签 - 使用更明显的颜色
            if (!string.IsNullOrEmpty(_label))
            {
                // 绘制标签背景以增强可读性
                Vector2 labelSize = font.MeasureString(_label);
                spriteBatch.Draw(_texture, new Rectangle(_bounds.X, _bounds.Y - 22, (int)labelSize.X + 6, 22), new Color(60, 60, 60));

                // 使用亮黄色绘制标签文本
                spriteBatch.DrawString(font, _label, new Vector2(_bounds.X + 3, _bounds.Y - 20), new Color(255, 255, 0));
            }

            // 绘制文本框背景 - 使用更亮的颜色
            Color backgroundColor = _isSelected ? new Color(70, 70, 70) : (_isHovered ? new Color(60, 60, 60) : new Color(50, 50, 50));
            spriteBatch.Draw(_texture, _bounds, backgroundColor);

            // 绘制文本框边框 - 使用更明显的颜色和更粗的边框
            Color borderColor = _isSelected ? Color.Yellow : (_isHovered ? new Color(200, 200, 200) : new Color(150, 150, 150));
            DrawBorder(spriteBatch, _bounds, borderColor, 2);

            // 绘制文本 - 使用更明显的颜色
            if (!string.IsNullOrEmpty(_text))
            {
                // 绘制文本阴影以增强可读性
                spriteBatch.DrawString(font, _text, new Vector2(_bounds.X + 6, _bounds.Y + (_bounds.Height - font.MeasureString(_text).Y) / 2 + 1), new Color(0, 0, 0, 150));

                // 使用亮色绘制文本
                spriteBatch.DrawString(font, _text, new Vector2(_bounds.X + 5, _bounds.Y + (_bounds.Height - font.MeasureString(_text).Y) / 2), new Color(220, 220, 220));
            }

            // 绘制光标 - 使用更明显的光标
            if (_isSelected && _showCursor)
            {
                float cursorX = _bounds.X + 5;
                if (!string.IsNullOrEmpty(_text))
                {
                    cursorX += font.MeasureString(_text).X;
                }
                // 使用更宽、更亮的光标
                spriteBatch.Draw(_texture, new Rectangle((int)cursorX, _bounds.Y + 5, 2, _bounds.Height - 10), Color.Yellow);
            }
        }

        /// <summary>
        /// 将键盘按键转换为字符
        /// </summary>
        /// <param name="key">按键</param>
        /// <param name="shift">是否按下 Shift 键</param>
        /// <returns>字符</returns>
        private char? KeyToChar(Keys key, bool shift)
        {
            // 处理中文输入法
            if (key == Keys.ProcessKey)
                return null;
            // 字母
            if (key >= Keys.A && key <= Keys.Z)
            {
                return shift ? (char)('A' + (key - Keys.A)) : (char)('a' + (key - Keys.A));
            }

            // 数字
            if (key >= Keys.D0 && key <= Keys.D9 && !shift)
            {
                return (char)('0' + (key - Keys.D0));
            }

            // 数字键盘
            if (key >= Keys.NumPad0 && key <= Keys.NumPad9)
            {
                return (char)('0' + (key - Keys.NumPad0));
            }

            // 特殊字符
            switch (key)
            {
                case Keys.Space: return ' ';
                case Keys.OemPeriod: return shift ? '>' : '.';
                case Keys.OemComma: return shift ? '<' : ',';
                case Keys.OemMinus: return shift ? '_' : '-';
                case Keys.OemPlus: return shift ? '+' : '=';
                case Keys.OemQuestion: return shift ? '?' : '/';
                case Keys.OemSemicolon: return shift ? ':' : ';';
                case Keys.OemQuotes: return shift ? '"' : '\'';
                case Keys.OemOpenBrackets: return shift ? '{' : '[';
                case Keys.OemCloseBrackets: return shift ? '}' : ']';
                case Keys.OemBackslash: return shift ? '|' : '\\';
                case Keys.OemTilde: return shift ? '~' : '`';
                case Keys.D1: return shift ? '!' : '1';
                case Keys.D2: return shift ? '@' : '2';
                case Keys.D3: return shift ? '#' : '3';
                case Keys.D4: return shift ? '$' : '4';
                case Keys.D5: return shift ? '%' : '5';
                case Keys.D6: return shift ? '^' : '6';
                case Keys.D7: return shift ? '&' : '7';
                case Keys.D8: return shift ? '*' : '8';
                case Keys.D9: return shift ? '(' : '9';
                case Keys.D0: return shift ? ')' : '0';
                default: return null;
            }
        }

        /// <summary>
        /// 绘制边框
        /// </summary>
        /// <param name="spriteBatch">精灵批处理</param>
        /// <param name="rectangle">矩形</param>
        /// <param name="color">颜色</param>
        /// <param name="thickness">厚度</param>
        private void DrawBorder(SpriteBatch spriteBatch, Rectangle rectangle, Color color, int thickness)
        {
            // 上边框
            spriteBatch.Draw(_texture, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, thickness), color);
            // 下边框
            spriteBatch.Draw(_texture, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height - thickness, rectangle.Width, thickness), color);
            // 左边框
            spriteBatch.Draw(_texture, new Rectangle(rectangle.X, rectangle.Y, thickness, rectangle.Height), color);
            // 右边框
            spriteBatch.Draw(_texture, new Rectangle(rectangle.X + rectangle.Width - thickness, rectangle.Y, thickness, rectangle.Height), color);
        }
    }
}
