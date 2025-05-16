using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpineEditor.UI;
using SpineEditor.Animation;
using SpineEditor.UI.UISystem;

namespace SpineEditor.Events
{
    /// <summary>
    /// 使用新UI系统的Spine帧事件编辑器游戏类
    /// </summary>
    public class SpineEventEditorGameNew : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private SpineEventEditor _eventEditor;
        private SpineViewport _viewport;
        private TimelineControlNew _timelineControl;
        private EventPropertyPanel _propertyPanel;
        private SpriteFont _font;
        private UIManager _uiManager;

        // UI 元素
        private Button _loadButton;
        private Button _saveButton;
        private Button _playButton;
        private Button _pauseButton;
        private Button _resetButton;
        private TextBox _speedTextBox;
        private DropdownList _animationDropdown;

        private string _currentFilePath = "events.json";

        /// <summary>
        /// 创建 Spine 帧事件编辑器游戏
        /// </summary>
        public SpineEventEditorGameNew()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // 设置窗口大小
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
        }

        /// <summary>
        /// 初始化游戏
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// 加载游戏内容
        /// </summary>
        protected override void LoadContent()
        {
            try
            {
                Console.WriteLine("开始加载游戏内容...");

                _spriteBatch = new SpriteBatch(GraphicsDevice);
                Console.WriteLine("创建 SpriteBatch 成功");

                // 创建UI管理器
                _uiManager = new UIManager();
                Console.WriteLine("创建 UI管理器 成功");

                // 加载字体
                _font = Content.Load<SpriteFont>("Font");
                Console.WriteLine("加载字体成功");

                // 创建 Spine 事件编辑器
                _eventEditor = new SpineEventEditor(GraphicsDevice);
                Console.WriteLine("创建 Spine事件编辑器 成功");

                // 加载 Spine 动画
                string atlasPath = Path.Combine(Content.RootDirectory, "spine", "tianshen.atlas");
                string skelPath = Path.Combine(Content.RootDirectory, "spine", "tianshen.skel");
                Console.WriteLine($"尝试加载Spine动画: {atlasPath}, {skelPath}");

                bool success = _eventEditor.LoadAnimation(
                    atlasPath,
                    skelPath,
                    0.5f,
                    new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2)
                );
                Console.WriteLine($"加载Spine动画结果: {success}");

                if (!success)
                {
                    Console.WriteLine("加载 Spine 动画失败");
                    Exit();
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"加载游戏内容时出错: {ex.Message}");
                Console.WriteLine($"堆栈跟踪: {ex.StackTrace}");
                throw;
            }

            // 创建视口控件
            _viewport = new SpineViewport(_eventEditor, GraphicsDevice, _font);

            // 创建新的时间轴控件
            _timelineControl = new TimelineControlNew(GraphicsDevice, _font);
            _timelineControl.SetBounds(new Rectangle(0, 500, GraphicsDevice.Viewport.Width, 200));

            // 添加到UI管理器
            _uiManager.AddElement(_timelineControl);

            // 获取动画时长
            if (_eventEditor.AnimationState != null && _eventEditor.AnimationNames.Length > 0)
            {
                string animName = _eventEditor.AnimationNames[0];
                _eventEditor.PlayAnimation(animName, true);
                _eventEditor.IsPlaying = false; // 初始暂停
                float duration = _eventEditor.AnimationDuration;
                _timelineControl.SetDuration(duration);
            }

            // 创建属性编辑面板
            _propertyPanel = new EventPropertyPanel(_eventEditor, GraphicsDevice, _font);
            _propertyPanel.SetBounds(new Rectangle(GraphicsDevice.Viewport.Width - 300, 0, 300, GraphicsDevice.Viewport.Height - 200));

            // 创建UI按钮
            _loadButton = new Button(GraphicsDevice, "Load", new Rectangle(10, 10, 80, 30));
            _saveButton = new Button(GraphicsDevice, "Save", new Rectangle(100, 10, 80, 30));
            _playButton = new Button(GraphicsDevice, "Play", new Rectangle(190, 10, 80, 30));
            _pauseButton = new Button(GraphicsDevice, "Pause", new Rectangle(280, 10, 80, 30));
            _resetButton = new Button(GraphicsDevice, "Reset", new Rectangle(370, 10, 80, 30));

            // 创建速度文本框
            _speedTextBox = new TextBox(GraphicsDevice, "Speed", "1.0", new Rectangle(460, 10, 80, 30));

            // 创建动画下拉列表
            List<string> animationNames = new List<string>(_eventEditor.AnimationNames);
            _animationDropdown = new DropdownList(GraphicsDevice, _font, "Animation", animationNames, new Rectangle(560, 10, 200, 30));

            // 设置初始选中的动画
            if (animationNames.Count > 0)
            {
                _animationDropdown.SelectedIndex = 0;
            }

            // 设置按钮事件
            _loadButton.Click += (sender, e) => {
                // 加载事件数据
                if (_eventEditor.LoadEventsFromJson(_currentFilePath))
                {
                    Console.WriteLine($"从 {_currentFilePath} 加载事件数据成功");

                    // 更新时间轴上的事件
                    _timelineControl.GetEvents().Clear();
                    foreach (var evt in _eventEditor.Events)
                    {
                        _timelineControl.AddEvent(evt);
                    }
                }
                else
                {
                    Console.WriteLine($"从 {_currentFilePath} 加载事件数据失败");
                }
            };

            _saveButton.Click += (sender, e) => {
                // 保存事件数据
                _eventEditor.SaveEventsToJson(_currentFilePath, _eventEditor.CurrentAnimation);
                Console.WriteLine($"事件数据已保存到 {_currentFilePath}");
            };

            _playButton.Click += (sender, e) => {
                _eventEditor.IsPlaying = true;
            };

            _pauseButton.Click += (sender, e) => {
                _eventEditor.IsPlaying = false;
            };

            _resetButton.Click += (sender, e) => {
                _eventEditor.CurrentTime = 0;
                _timelineControl.CurrentTime = 0;
            };

            _speedTextBox.TextChanged += (sender, e) => {
                if (float.TryParse(_speedTextBox.Text, out float speed))
                {
                    _eventEditor.PlaybackSpeed = MathHelper.Clamp(speed, 0.1f, 10.0f);
                }
            };

            // 设置事件选中处理
            _timelineControl.OnEventSelected += (sender, evt) => {
                _propertyPanel.SetSelectedEvent(evt);
            };

            // 设置时间变化处理
            _timelineControl.OnTimeChanged += (sender, time) => {
                _eventEditor.CurrentTime = time;
            };

            // 设置事件触发处理
            _eventEditor.OnEventTriggered += (sender, evt) => {
                Console.WriteLine($"事件触发: {evt.Name}, 时间: {evt.Time}, 整数值: {evt.IntValue}, 浮点值: {evt.FloatValue}, 字符串值: {evt.StringValue}");
            };

            // 设置动画下拉列表事件
            _animationDropdown.SelectedIndexChanged += (sender, e) => {
                if (_animationDropdown.SelectedItem != null)
                {
                    // 切换动画
                    bool success = _eventEditor.SwitchAnimation(_animationDropdown.SelectedItem, true);

                    if (success)
                    {
                        // 更新时间轴的持续时间
                        _timelineControl.SetDuration(_eventEditor.AnimationDuration);

                        // 更新时间轴上的事件
                        _timelineControl.GetEvents().Clear();
                        foreach (var evt in _eventEditor.Events)
                        {
                            _timelineControl.AddEvent(evt);
                        }
                    }
                }
            };
        }

        /// <summary>
        /// 更新游戏
        /// </summary>
        /// <param name="gameTime">游戏时间</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // 更新 UI 管理器
            _uiManager.Update(gameTime);

            // 更新 UI 按钮
            _loadButton.Update();
            _saveButton.Update();
            _playButton.Update();
            _pauseButton.Update();
            _resetButton.Update();
            _speedTextBox.Update(gameTime);
            _animationDropdown.Update();

            // 更新视口控件
            _viewport.Update(gameTime);

            // 更新属性编辑面板
            _propertyPanel.Update(gameTime);

            // 更新 Spine 动画
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;
            _eventEditor.Update(deltaTime);

            // 同步时间轴的当前时间
            _timelineControl.CurrentTime = _eventEditor.CurrentTime;

            base.Update(gameTime);
        }

        /// <summary>
        /// 绘制游戏
        /// </summary>
        /// <param name="gameTime">游戏时间</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // 绘制背景网格
            _spriteBatch.Begin();
            if (_viewport.ShowGrid)
            {
                _viewport.DrawGrid(_spriteBatch);
            }
            _spriteBatch.End();

            // 绘制 Spine 动画
            _eventEditor.Draw();

            // 绘制 UI 管理器（包含新的时间轴控件）
            _spriteBatch.Begin();
            _uiManager.Draw(_spriteBatch);
            _spriteBatch.End();

            // 绘制 UI 按钮和其他控件
            _spriteBatch.Begin();
            _loadButton.Draw(_spriteBatch, _font);
            _saveButton.Draw(_spriteBatch, _font);
            _playButton.Draw(_spriteBatch, _font);
            _pauseButton.Draw(_spriteBatch, _font);
            _resetButton.Draw(_spriteBatch, _font);
            _speedTextBox.Draw(_spriteBatch, _font);
            _animationDropdown.Draw(_spriteBatch);

            // 绘制视口信息
            _viewport.DrawInfo(_spriteBatch);

            // 绘制属性编辑面板
            _propertyPanel.Draw(_spriteBatch);

            _spriteBatch.End();

            // 绘制当前信息
            _spriteBatch.Begin();
            string infoText = $"Current Time: {_eventEditor.CurrentTime:F3} / {_eventEditor.AnimationDuration:F3}\n" +
                             $"Current Animation: {_eventEditor.CurrentAnimation}\n" +
                             $"Event Count: {_eventEditor.Events.Count}\n" +
                             $"Scale: {_eventEditor.Scale:F2}";
            _spriteBatch.DrawString(_font, infoText, new Vector2(10, 50), Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
