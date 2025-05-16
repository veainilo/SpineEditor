using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpineEditor.UI
{
    /// <summary>
    /// 左侧面板控件，包含信息显示、操作按钮和动画列表
    /// </summary>
    public class LeftPanel
    {
        private GraphicsDevice _graphicsDevice;
        private SpriteFont _font;
        private Rectangle _bounds;
        private Texture2D _texture;
        private bool _visible = true;
        private bool _isHovered = false;
        private MouseState _prevMouseState;

        // 面板内的控件
        private Button _playPauseButton;
        private Button _resetButton;
        private TextBox _speedTextBox;
        private AnimationListBox _animationList;

        // 信息文本
        private string _currentTime = "0.000";
        private string _totalTime = "0.000";
        private string _currentAnimation = "";
        private int _eventCount = 0;
        private float _scale = 1.0f;

        // 面板区域
        private Rectangle _infoArea;
        private Rectangle _controlArea;
        private Rectangle _animationArea;

        // 事件
        public event EventHandler PlayPauseClicked;
        public event EventHandler ResetClicked;
        public event EventHandler<string> SpeedChanged;
        public event EventHandler<string> AnimationSelected;

        /// <summary>
        /// 获取或设置控件是否可见
        /// </summary>
        public bool Visible
        {
            get => _visible;
            set => _visible = value;
        }

        /// <summary>
        /// 获取动画列表控件
        /// </summary>
        public AnimationListBox AnimationList => _animationList;

        /// <summary>
        /// 获取面板的边界
        /// </summary>
        public Rectangle Bounds => _bounds;

        /// <summary>
        /// 创建左侧面板
        /// </summary>
        /// <param name="graphicsDevice">图形设备</param>
        /// <param name="font">字体</param>
        /// <param name="bounds">边界</param>
        public LeftPanel(GraphicsDevice graphicsDevice, SpriteFont font, Rectangle bounds)
        {
            _graphicsDevice = graphicsDevice;
            _font = font;
            _bounds = bounds;

            // 创建纹理
            _texture = new Texture2D(graphicsDevice, 1, 1);
            _texture.SetData(new[] { Color.White });

            // 计算各区域的位置和大小
            int padding = 10;
            int sectionSpacing = 15;
            int infoHeight = 120; // 增加信息区域高度
            int controlHeight = 140;

            // 信息区域 - 顶部
            _infoArea = new Rectangle(
                bounds.X + padding,
                bounds.Y + padding + 20, // 增加顶部间距，避免文字重叠
                bounds.Width - padding * 2,
                infoHeight);

            // 控制区域 - 中间
            _controlArea = new Rectangle(
                bounds.X + padding,
                _infoArea.Y + _infoArea.Height + sectionSpacing,
                bounds.Width - padding * 2,
                controlHeight);

            // 动画列表区域 - 底部，占据剩余空间
            _animationArea = new Rectangle(
                bounds.X + padding,
                _controlArea.Y + _controlArea.Height + sectionSpacing,
                bounds.Width - padding * 2,
                bounds.Height - _infoArea.Height - controlHeight - sectionSpacing * 3 - padding * 2 - 20);

            // 创建控件
            InitializeControls();

            _prevMouseState = Mouse.GetState();
        }

        /// <summary>
        /// 初始化控件
        /// </summary>
        private void InitializeControls()
        {
            int buttonWidth = 100; // 增加按钮宽度
            int buttonHeight = 30;
            int buttonSpacing = 15;
            int labelWidth = 50; // 标签宽度
            int inputWidth = buttonWidth - labelWidth; // 输入框宽度

            // 控制区域内部布局
            int startX = _controlArea.X + (_controlArea.Width - buttonWidth) / 2;
            int startY = _controlArea.Y + 25; // 减少顶部间距

            // 创建播放/暂停按钮
            _playPauseButton = new Button(_graphicsDevice, "Pause",
                new Rectangle(startX, startY, buttonWidth, buttonHeight));
            startY += buttonHeight + buttonSpacing;

            // 创建重置按钮
            _resetButton = new Button(_graphicsDevice, "Reset",
                new Rectangle(startX, startY, buttonWidth, buttonHeight));
            startY += buttonHeight + buttonSpacing;

            // 创建速度文本框 - 左侧标签，右侧输入框
            _speedTextBox = new TextBox(_graphicsDevice, "Speed", "1.0",
                new Rectangle(startX, startY, buttonWidth, buttonHeight));

            // 创建动画列表
            _animationList = new AnimationListBox(_graphicsDevice, _font,
                new List<string>(), _animationArea);

            // 设置事件处理
            _playPauseButton.Click += (sender, e) => PlayPauseClicked?.Invoke(this, EventArgs.Empty);
            _resetButton.Click += (sender, e) => ResetClicked?.Invoke(this, EventArgs.Empty);
            _speedTextBox.TextChanged += (sender, e) => SpeedChanged?.Invoke(this, _speedTextBox.Text);
            _animationList.SelectedIndexChanged += (sender, e) =>
            {
                if (_animationList.SelectedItem != null)
                {
                    AnimationSelected?.Invoke(this, _animationList.SelectedItem);
                }
            };
        }

        /// <summary>
        /// 更新面板状态
        /// </summary>
        /// <param name="gameTime">游戏时间</param>
        public void Update(GameTime gameTime)
        {
            if (!_visible)
                return;

            MouseState mouseState = Mouse.GetState();
            _isHovered = _bounds.Contains(mouseState.Position);

            // 更新控件
            _playPauseButton.Update();
            _resetButton.Update();
            _speedTextBox.Update(gameTime);
            _animationList.Update();

            _prevMouseState = mouseState;
        }

        /// <summary>
        /// 设置播放/暂停按钮文本
        /// </summary>
        /// <param name="isPlaying">是否正在播放</param>
        public void SetPlayPauseButtonText(bool isPlaying)
        {
            _playPauseButton.Text = isPlaying ? "Pause" : "Play";
        }

        /// <summary>
        /// 更新信息
        /// </summary>
        /// <param name="currentTime">当前时间</param>
        /// <param name="totalTime">总时间</param>
        /// <param name="currentAnimation">当前动画</param>
        /// <param name="eventCount">事件数量</param>
        /// <param name="scale">缩放比例</param>
        public void UpdateInfo(float currentTime, float totalTime, string currentAnimation, int eventCount, float scale)
        {
            _currentTime = currentTime.ToString("F3");
            _totalTime = totalTime.ToString("F3");
            _currentAnimation = currentAnimation;
            _eventCount = eventCount;
            _scale = scale;
        }

        /// <summary>
        /// 设置动画列表
        /// </summary>
        /// <param name="animations">动画列表</param>
        public void SetAnimations(IEnumerable<string> animations)
        {
            _animationList.Items = new List<string>(animations);
        }

        /// <summary>
        /// 绘制面板
        /// </summary>
        /// <param name="spriteBatch">精灵批处理</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (!_visible)
                return;

            // 绘制面板背景
            Color backgroundColor = new Color(40, 40, 40, 230);
            spriteBatch.Draw(_texture, _bounds, backgroundColor);

            // 绘制面板边框
            DrawBorder(spriteBatch, _bounds, new Color(100, 100, 100), 2);

            // 绘制信息区域
            Color infoBackgroundColor = new Color(50, 50, 50);
            spriteBatch.Draw(_texture, _infoArea, infoBackgroundColor);
            DrawBorder(spriteBatch, _infoArea, new Color(80, 80, 80), 1);

            // 绘制信息区域标题
            spriteBatch.DrawString(_font, "信息", new Vector2(_infoArea.X + 10, _infoArea.Y - 20), Color.White);

            // 绘制信息文本
            float lineHeight = _font.MeasureString("A").Y + 8; // 增加行间距

            // 第一行 - 当前时间
            Vector2 labelPos = new Vector2(_infoArea.X + 10, _infoArea.Y + 15);
            Vector2 valuePos = new Vector2(_infoArea.X + 90, _infoArea.Y + 15); // 值的位置右对齐

            spriteBatch.DrawString(_font, "当前时间:", labelPos, Color.LightGray);
            spriteBatch.DrawString(_font, $"{_currentTime} / {_totalTime}", valuePos, Color.White);

            // 第二行 - 当前动画
            labelPos.Y += lineHeight;
            valuePos.Y += lineHeight;
            spriteBatch.DrawString(_font, "当前动画:", labelPos, Color.LightGray);
            spriteBatch.DrawString(_font, _currentAnimation, valuePos, Color.White);

            // 第三行 - 事件数量
            labelPos.Y += lineHeight;
            valuePos.Y += lineHeight;
            spriteBatch.DrawString(_font, "事件数量:", labelPos, Color.LightGray);
            spriteBatch.DrawString(_font, _eventCount.ToString(), valuePos, Color.White);

            // 第四行 - 缩放比例
            labelPos.Y += lineHeight;
            valuePos.Y += lineHeight;
            spriteBatch.DrawString(_font, "缩放比例:", labelPos, Color.LightGray);
            spriteBatch.DrawString(_font, _scale.ToString("F2"), valuePos, Color.White);

            // 绘制控制区域
            Color controlBackgroundColor = new Color(50, 50, 50);
            spriteBatch.Draw(_texture, _controlArea, controlBackgroundColor);
            DrawBorder(spriteBatch, _controlArea, new Color(80, 80, 80), 1);

            // 绘制控制区域标题
            spriteBatch.DrawString(_font, "控制", new Vector2(_controlArea.X + 10, _controlArea.Y - 20), Color.White);

            // 绘制控件 - 确保控件不会重叠
            int controlSpacing = 15; // 控件之间的间距
            int controlY = _controlArea.Y + 15;

            // 调整按钮位置
            _playPauseButton.Bounds = new Rectangle(
                _controlArea.X + (_controlArea.Width - _playPauseButton.Bounds.Width) / 2,
                controlY,
                _playPauseButton.Bounds.Width,
                _playPauseButton.Bounds.Height);
            _playPauseButton.Draw(spriteBatch, _font);

            controlY += _playPauseButton.Bounds.Height + controlSpacing;

            _resetButton.Bounds = new Rectangle(
                _controlArea.X + (_controlArea.Width - _resetButton.Bounds.Width) / 2,
                controlY,
                _resetButton.Bounds.Width,
                _resetButton.Bounds.Height);
            _resetButton.Draw(spriteBatch, _font);

            controlY += _resetButton.Bounds.Height + controlSpacing;

            _speedTextBox.Bounds = new Rectangle(
                _controlArea.X + (_controlArea.Width - _speedTextBox.Bounds.Width) / 2,
                controlY,
                _speedTextBox.Bounds.Width,
                _speedTextBox.Bounds.Height);
            _speedTextBox.Draw(spriteBatch, _font);

            // 绘制动画列表区域标题
            spriteBatch.DrawString(_font, "动画列表", new Vector2(_animationArea.X + 10, _animationArea.Y - 20), Color.White);

            // 绘制动画列表
            _animationList.Draw(spriteBatch);
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
