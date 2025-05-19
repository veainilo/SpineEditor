using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpineEditor.UI
{
    /// <summary>
    /// 提供临时消息提示的Toast控件
    /// </summary>
    public class Toast
    {
        private string _message;
        private float _displayTime;
        private float _fadeInTime;
        private float _fadeOutTime;
        private float _currentTime;
        private bool _isVisible;
        private Vector2 _position;
        private Color _backgroundColor;
        private Color _textColor;
        private SpriteFont _font;
        private Texture2D _texture;
        private GraphicsDevice _graphicsDevice;
        private int _padding = 10;
        private float _alpha = 0f;

        /// <summary>
        /// 获取Toast是否可见
        /// </summary>
        public bool IsVisible => _isVisible;

        /// <summary>
        /// 创建一个新的Toast控件
        /// </summary>
        /// <param name="graphicsDevice">图形设备</param>
        /// <param name="font">字体</param>
        public Toast(GraphicsDevice graphicsDevice, SpriteFont font)
        {
            _graphicsDevice = graphicsDevice;
            _font = font;
            _isVisible = false;
            _backgroundColor = new Color(40, 40, 40, 200);
            _textColor = Color.White;

            // 创建1x1白色纹理用于绘制背景
            _texture = new Texture2D(graphicsDevice, 1, 1);
            _texture.SetData(new[] { Color.White });
        }

        /// <summary>
        /// 显示一条消息
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="displayTime">显示时间（秒）</param>
        /// <param name="fadeInTime">淡入时间（秒）</param>
        /// <param name="fadeOutTime">淡出时间（秒）</param>
        public void Show(string message, float displayTime = 2.0f, float fadeInTime = 0.3f, float fadeOutTime = 0.5f)
        {
            _message = message;
            _displayTime = displayTime;
            _fadeInTime = fadeInTime;
            _fadeOutTime = fadeOutTime;
            _currentTime = 0;
            _isVisible = true;
            _alpha = 0f;
        }

        /// <summary>
        /// 更新Toast状态
        /// </summary>
        /// <param name="gameTime">游戏时间</param>
        public void Update(GameTime gameTime)
        {
            if (!_isVisible)
                return;

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _currentTime += deltaTime;

            // 计算alpha值
            if (_currentTime < _fadeInTime)
            {
                // 淡入阶段
                _alpha = _currentTime / _fadeInTime;
            }
            else if (_currentTime < _fadeInTime + _displayTime)
            {
                // 显示阶段
                _alpha = 1.0f;
            }
            else if (_currentTime < _fadeInTime + _displayTime + _fadeOutTime)
            {
                // 淡出阶段
                _alpha = 1.0f - (_currentTime - _fadeInTime - _displayTime) / _fadeOutTime;
            }
            else
            {
                // 结束显示
                _isVisible = false;
                _alpha = 0f;
            }
        }

        /// <summary>
        /// 绘制Toast
        /// </summary>
        /// <param name="spriteBatch">精灵批处理</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (!_isVisible || _alpha <= 0)
                return;

            // 计算消息尺寸
            Vector2 textSize = _font.MeasureString(_message);
            
            // 计算Toast位置（屏幕底部居中）
            _position = new Vector2(
                _graphicsDevice.Viewport.Width / 2 - (textSize.X + _padding * 2) / 2,
                _graphicsDevice.Viewport.Height - textSize.Y - _padding * 2 - 50); // 距离底部50像素

            // 计算Toast矩形
            Rectangle toastRect = new Rectangle(
                (int)_position.X,
                (int)_position.Y,
                (int)(textSize.X + _padding * 2),
                (int)(textSize.Y + _padding * 2));

            // 绘制背景
            Color bgColor = _backgroundColor * _alpha;
            spriteBatch.Draw(_texture, toastRect, bgColor);

            // 绘制边框
            DrawBorder(spriteBatch, toastRect, new Color(100, 100, 100) * _alpha, 1);

            // 绘制文本
            Color textColor = _textColor * _alpha;
            spriteBatch.DrawString(_font, _message, 
                new Vector2(_position.X + _padding, _position.Y + _padding), 
                textColor);
        }

        /// <summary>
        /// 绘制边框
        /// </summary>
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
