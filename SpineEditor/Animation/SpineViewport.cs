using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace SpineEditor.Animation
{
    /// <summary>
    /// Spine 视口类，用于管理动画的缩放、拖拽和网格线显示
    /// </summary>
    public class SpineViewport
    {
        private SpineAnimationPlayer _player;
        private GraphicsDevice _graphicsDevice;
        private Texture2D _pixel;
        private SpriteFont _font;

        private bool _isDragging = false;
        private Vector2 _dragStart;
        private Vector2 _positionStart;
        private MouseState _prevMouseState;

        private bool _showGrid = true;
        private int _gridSize = 50;
        private Color _gridColor = new Color(100, 100, 100, 100);
        private Color _gridAxisColor = new Color(150, 150, 150, 150);

        private float _minScale = 0.1f;
        private float _maxScale = 10.0f;
        private float _scaleStep = 0.1f;

        // 已移动到下方

        /// <summary>
        /// 获取或设置网格大小
        /// </summary>
        public int GridSize
        {
            get => _gridSize;
            set => _gridSize = Math.Max(10, value);
        }

        /// <summary>
        /// 创建 Spine 视口
        /// </summary>
        /// <param name="player">Spine 动画播放器</param>
        /// <param name="graphicsDevice">图形设备</param>
        /// <param name="font">字体</param>
        public SpineViewport(SpineAnimationPlayer player, GraphicsDevice graphicsDevice, SpriteFont font)
        {
            _player = player;
            _graphicsDevice = graphicsDevice;
            _font = font;

            // 创建网格纹理
            _pixel = new Texture2D(graphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });

            _prevMouseState = Mouse.GetState();
        }

        /// <summary>
        /// 更新视口
        /// </summary>
        /// <param name="gameTime">游戏时间</param>
        public void Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();
            KeyboardState keyboardState = Keyboard.GetState();

            // 处理鼠标滚轮缩放
            if (mouseState.ScrollWheelValue != _prevMouseState.ScrollWheelValue)
            {
                float zoomDelta = (mouseState.ScrollWheelValue - _prevMouseState.ScrollWheelValue) / 120.0f * _scaleStep;
                float newScale = MathHelper.Clamp(_player.Scale + zoomDelta, _minScale, _maxScale);

                // 计算缩放前鼠标位置相对于动画中心的偏移
                Vector2 mousePos = new Vector2(mouseState.X, mouseState.Y);
                Vector2 centerOffset = mousePos - _player.Position;

                // 计算缩放后的新位置，保持鼠标指向的点不变
                Vector2 newPosition = mousePos - centerOffset * (newScale / _player.Scale);

                // 应用新的缩放和位置
                _player.Scale = newScale;
                _player.Position = newPosition;
            }

            // 处理鼠标拖拽
            if (mouseState.MiddleButton == ButtonState.Pressed ||
                (mouseState.LeftButton == ButtonState.Pressed && keyboardState.IsKeyDown(Keys.Space)))
            {
                if (!_isDragging)
                {
                    // 开始拖拽
                    _isDragging = true;
                    _dragStart = new Vector2(mouseState.X, mouseState.Y);
                    _positionStart = _player.Position;
                }
                else
                {
                    // 继续拖拽
                    Vector2 dragDelta = new Vector2(mouseState.X, mouseState.Y) - _dragStart;
                    _player.Position = _positionStart + dragDelta;
                }
            }
            else
            {
                _isDragging = false;
            }

            // 处理键盘快捷键
            if (keyboardState.IsKeyDown(Keys.G) && !_prevMouseState.LeftButton.HasFlag(ButtonState.Pressed) &&
                !_prevMouseState.RightButton.HasFlag(ButtonState.Pressed) && !_prevMouseState.MiddleButton.HasFlag(ButtonState.Pressed))
            {
                // 切换网格显示
                _showGrid = !_showGrid;
            }

            if (keyboardState.IsKeyDown(Keys.Home))
            {
                // 重置视图
                _player.Position = new Vector2(_graphicsDevice.Viewport.Width / 2, _graphicsDevice.Viewport.Height / 2);
                _player.Scale = 1.0f;
            }

            _prevMouseState = mouseState;
        }

        /// <summary>
        /// 获取或设置是否显示网格
        /// </summary>
        public bool ShowGrid
        {
            get => _showGrid;
            set => _showGrid = value;
        }

        /// <summary>
        /// 绘制视口信息（不包括网格）
        /// </summary>
        /// <param name="spriteBatch">精灵批处理</param>
        public void DrawInfo(SpriteBatch spriteBatch)
        {
            // 绘制缩放信息
            string scaleText = $"Scale: {_player.Scale:0.00}";
            spriteBatch.DrawString(_font, scaleText, new Vector2(10, 110), Color.White);
        }

        /// <summary>
        /// 绘制网格
        /// </summary>
        /// <param name="spriteBatch">精灵批处理</param>
        public void DrawGrid(SpriteBatch spriteBatch)
        {
            int viewWidth = _graphicsDevice.Viewport.Width;
            int viewHeight = _graphicsDevice.Viewport.Height;

            // 计算网格原点（动画位置）
            Vector2 origin = _player.Position;

            // 计算网格线的间距（考虑缩放）
            float scaledGridSize = _gridSize * _player.Scale;

            // 计算网格线的起始和结束位置
            int startX = (int)(origin.X % scaledGridSize);
            int startY = (int)(origin.Y % scaledGridSize);

            // 绘制垂直网格线
            for (float x = startX; x < viewWidth; x += scaledGridSize)
            {
                Color lineColor = Math.Abs(x - origin.X) < 1 ? _gridAxisColor : _gridColor;
                spriteBatch.Draw(_pixel, new Rectangle((int)x, 0, 1, viewHeight), lineColor);
            }

            // 绘制水平网格线
            for (float y = startY; y < viewHeight; y += scaledGridSize)
            {
                Color lineColor = Math.Abs(y - origin.Y) < 1 ? _gridAxisColor : _gridColor;
                spriteBatch.Draw(_pixel, new Rectangle(0, (int)y, viewWidth, 1), lineColor);
            }

            // 绘制原点标记
            int markerSize = 10;
            spriteBatch.Draw(_pixel, new Rectangle((int)origin.X - markerSize / 2, (int)origin.Y - markerSize / 2, markerSize, markerSize), new Color(255, 0, 0, 150));
        }
    }
}
