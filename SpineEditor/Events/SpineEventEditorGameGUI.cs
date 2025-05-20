using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpineEditor.UI;
using SpineEditor.UI.GUILayoutComponents;
using SpineEditor.UI.UISystem;
using System;
using System.IO;
using System.Text.Json;

namespace SpineEditor.Events
{
    /// <summary>
    /// 使用GUILayout系统的Spine帧事件编辑器游戏类
    /// </summary>
    public class SpineEventEditorGameGUI : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private SpineEventEditor _eventEditor;
        private TimelineControlGUI _timelineControl;
        private SpriteFont _font;
        private UIManager _uiManager;

        // 攻击形状渲染器
        private AttackShapeRenderer _attackShapeRenderer;

        // 保存事件相关
        private bool _isSavingEvents = false;
        private string _currentFilePath = "events.json";
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        // GUILayout面板
        private LeftPanelGUI _leftPanel;
        private EventPropertyPanelGUI _propertyPanel;
        private SpineViewportGUI _viewport;

        // Toast提示
        private Toast _toast;

        public SpineEventEditorGameGUI()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // 设置窗口大小
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;

            // 允许调整窗口大小
            Window.AllowUserResizing = true;

            // 监听窗口大小变化事件
            Window.ClientSizeChanged += Window_ClientSizeChanged;
        }

        /// <summary>
        /// 窗口大小变化事件处理
        /// </summary>
        private void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            // 更新UI布局
            UpdateUILayout();
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
                _uiManager = new UIManager(GraphicsDevice);
                Console.WriteLine("创建 UI管理器 成功");

                // 加载字体
                _font = Content.Load<SpriteFont>("Font");
                Console.WriteLine("加载字体成功");

                // 创建攻击形状渲染器
                _attackShapeRenderer = new AttackShapeRenderer(GraphicsDevice);
                Console.WriteLine("创建攻击形状渲染器成功");

                // 创建Toast提示
                _toast = new Toast(GraphicsDevice, _font);
                Console.WriteLine("创建Toast提示成功");

                // 创建 Spine 事件编辑器
                _eventEditor = new SpineEventEditor(GraphicsDevice);
                Console.WriteLine("创建 Spine事件编辑器 成功");

                // 初始化GUILayout系统
                GUILayout.Initialize(_uiManager, _font);
                Console.WriteLine("初始化 GUILayout系统 成功");

                // 创建左侧面板
                int leftPanelWidth = 280;
                _leftPanel = new LeftPanelGUI("左侧面板",
                    new Rectangle(0, 0, leftPanelWidth, GraphicsDevice.Viewport.Height - 200),
                    GraphicsDevice, _font);
                Console.WriteLine("创建 左侧面板 成功");

                // 创建属性编辑面板
                _propertyPanel = new EventPropertyPanelGUI(_eventEditor, "属性面板",
                    new Rectangle(GraphicsDevice.Viewport.Width - 300, 0, 300, GraphicsDevice.Viewport.Height - 200),
                    GraphicsDevice, _font);
                Console.WriteLine("创建 属性面板 成功");

                // 创建视口
                _viewport = new SpineViewportGUI(_eventEditor, "视口",
                    new Rectangle(leftPanelWidth, 0,
                        GraphicsDevice.Viewport.Width - leftPanelWidth - 300,
                        GraphicsDevice.Viewport.Height - 200),
                    GraphicsDevice, _font);
                _viewport.BackgroundColor = new Color(50, 50, 60); // 设置视口背景色
                Console.WriteLine("创建 视口 成功");

                // 创建新的时间轴控件
                _timelineControl = new TimelineControlGUI("时间轴",
                    new Rectangle(0, GraphicsDevice.Viewport.Height - 200, GraphicsDevice.Viewport.Width, 200),
                    GraphicsDevice, _font);
                Console.WriteLine("创建 时间轴控件 成功");

                // 设置事件处理
                SetupEventHandlers();

                // 加载默认动画
                LoadDefaultAnimation();

                // 初始化完成后立即更新UI布局，确保所有元素位置正确
                UpdateUILayout();

                Console.WriteLine("加载游戏内容完成");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"加载游戏内容时出错: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }

        /// <summary>
        /// 设置事件处理
        /// </summary>
        private void SetupEventHandlers()
        {
            // 设置时间轴事件选择事件
            _timelineControl.EventSelected += (sender, evt) => {
                _propertyPanel.SetSelectedEvent(evt);
            };

            // 设置时间轴时间变化事件
            _timelineControl.TimeChanged += (sender, time) => {
                _eventEditor.CurrentTime = time;
            };

            // 设置播放/暂停按钮事件
            _leftPanel.PlayPauseClicked += (sender, e) => {
                if (_eventEditor.IsPlaying)
                {
                    // 如果正在播放，则暂停
                    _eventEditor.IsPlaying = false;
                    _leftPanel.SetPlayPauseButtonText(false);
                }
                else
                {
                    // 如果已暂停，则播放
                    _eventEditor.IsPlaying = true;
                    _leftPanel.SetPlayPauseButtonText(true);
                }
            };

            // 设置重置按钮事件
            _leftPanel.ResetClicked += (sender, e) => {
                _eventEditor.CurrentTime = 0;
                _timelineControl.CurrentTime = 0;
            };

            // 设置保存按钮事件
            _leftPanel.SaveClicked += (sender, e) => {
                SaveEvents();
            };

            // 设置速度变更事件
            _leftPanel.SpeedChanged += (sender, speedText) => {
                if (float.TryParse(speedText, out float speed))
                {
                    _eventEditor.PlaybackSpeed = MathHelper.Clamp(speed, 0.1f, 10.0f);
                }
            };

            // 设置动画选择事件
            _leftPanel.AnimationSelected += (sender, animationName) => {
                // 切换动画
                bool success = _eventEditor.SwitchAnimation(animationName, true);

                if (success)
                {
                    // 更新时间轴的持续时间
                    _timelineControl.SetDuration(_eventEditor.AnimationDuration);

                    // 更新时间轴上的事件
                    _timelineControl.Events.Clear();
                    foreach (var evt in _eventEditor.Events)
                    {
                        _timelineControl.AddEvent(evt.Name, evt.Time);
                    }
                }
            };

            // 设置属性面板事件
            _propertyPanel.EventAdded += (sender, evt) => {
                _timelineControl.AddEvent(evt.Name, evt.Time);
            };

            _propertyPanel.EventDeleted += (sender, evt) => {
                int index = _timelineControl.Events.IndexOf(evt);
                if (index >= 0)
                {
                    _timelineControl.RemoveEvent(index);
                }
            };
        }

        /// <summary>
        /// 加载默认动画
        /// </summary>
        private void LoadDefaultAnimation()
        {
            try
            {
                // 加载默认的Spine动画数据
                string contentDir = Path.Combine(Directory.GetCurrentDirectory(), "Content");
                string atlasPath = Path.Combine(contentDir, "spineboy.atlas");
                string skelPath = Path.Combine(contentDir, "spineboy.skel");

                // 如果文件不存在，尝试使用相对路径
                if (!File.Exists(atlasPath) || !File.Exists(skelPath))
                {
                    atlasPath = "Content/spineboy.atlas";
                    skelPath = "Content/spineboy.skel";
                }

                // 检查文件是否存在
                if (!File.Exists(atlasPath) || !File.Exists(skelPath))
                {
                    Console.WriteLine($"默认动画文件不存在: {atlasPath} 或 {skelPath}");
                    _toast.Show("默认动画文件不存在，请拖放Spine动画文件到窗口", 5.0f);
                    return;
                }

                Console.WriteLine($"加载默认动画: {atlasPath}, {skelPath}");

                // 加载动画
                bool success = _eventEditor.LoadAnimation(atlasPath, skelPath, 0.5f);
                if (!success)
                {
                    Console.WriteLine("加载默认动画失败");
                    _toast.Show("加载默认动画失败", 3.0f);
                    return;
                }

                Console.WriteLine("加载默认动画成功");

                // 获取动画时长
                if (_eventEditor.AnimationState != null && _eventEditor.AnimationNames.Length > 0)
                {
                    string animName = _eventEditor.AnimationNames[0];
                    _eventEditor.PlayAnimation(animName, true);
                    _eventEditor.IsPlaying = false; // 初始暂停
                    float duration = _eventEditor.AnimationDuration;
                    _timelineControl.SetDuration(duration);

                    Console.WriteLine($"播放动画: {animName}, 持续时间: {duration}秒");
                }

                // 设置初始动画列表
                _leftPanel.SetAnimations(_eventEditor.AnimationNames);

                // 设置当前文件路径
                _currentFilePath = Path.ChangeExtension(skelPath, ".events.json");

                // 尝试加载事件数据
                string eventFilePath = Path.Combine(Path.GetDirectoryName(skelPath),
                    Path.GetFileNameWithoutExtension(skelPath) + "_events.json");
                if (File.Exists(eventFilePath))
                {
                    bool loadSuccess = _eventEditor.LoadEventsFromJson(eventFilePath);
                    Console.WriteLine($"加载事件数据{(loadSuccess ? "成功" : "失败")}: {eventFilePath}");

                    // 更新时间轴上的事件
                    _timelineControl.Events.Clear();
                    foreach (var evt in _eventEditor.Events)
                    {
                        _timelineControl.AddEvent(evt.Name, evt.Time);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"加载默认动画时出错: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                _toast.Show($"加载默认动画时出错: {ex.Message}", 5.0f);
            }
        }

        /// <summary>
        /// 更新游戏
        /// </summary>
        /// <param name="gameTime">游戏时间</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // 检查是否按下Ctrl+S保存事件
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.LeftControl) && keyboardState.IsKeyDown(Keys.S))
            {
                if (!_isSavingEvents)
                {
                    _isSavingEvents = true;
                    SaveEvents();
                }
            }
            else
            {
                _isSavingEvents = false;
            }

            // 获取鼠标状态
            MouseState mouseState = Mouse.GetState();

            // 检查鼠标是否在时间轴控件范围内
            bool isMouseOverTimeline = new Rectangle(0, GraphicsDevice.Viewport.Height - 200,
                GraphicsDevice.Viewport.Width, 200).Contains(mouseState.Position);

            // 检查鼠标是否在左侧面板区域内
            bool isMouseOverLeftPanel = new Rectangle(0, 0, 280, GraphicsDevice.Viewport.Height - 200).Contains(mouseState.Position);

            // 检查鼠标是否在属性面板区域内
            bool isMouseOverPropertyPanel = new Rectangle(GraphicsDevice.Viewport.Width - 300, 0, 300, GraphicsDevice.Viewport.Height - 200).Contains(mouseState.Position);

            // 更新 UI 管理器
            _uiManager.Update(gameTime);

            // 更新左侧面板
            _leftPanel.Update(gameTime);

            // 更新属性编辑面板
            _propertyPanel.Update(gameTime);

            // 更新视口控件，只有当鼠标不在时间轴控件范围内且不在左侧面板区域内且不在属性面板区域内时才处理滚轮事件
            _viewport.Update(gameTime, !isMouseOverTimeline && !isMouseOverLeftPanel && !isMouseOverPropertyPanel);

            // 更新时间轴控件
            _timelineControl.Update(gameTime);

            // 更新 Spine 动画
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;
            _eventEditor.Update(deltaTime);

            // 同步时间轴的当前时间
            _timelineControl.CurrentTime = _eventEditor.CurrentTime;

            // 更新Toast提示
            _toast.Update(gameTime);

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
            DrawGrid();

            // 绘制 Spine 动画 - 注意：这里不直接调用Draw方法，而是在视口控件中绘制
            // _eventEditor.Draw();

            // 绘制攻击形状 - 在视口内绘制
            var selectedEvent = _timelineControl.SelectedEvent;
            if (selectedEvent != null && selectedEvent.EventType == EventType.Attack)
            {
                // 保存当前视口状态
                Rectangle originalViewport = GraphicsDevice.Viewport.Bounds;

                try
                {
                    // 设置新的视口，限制绘制区域
                    GraphicsDevice.Viewport = new Viewport(_viewport.Bounds);

                    // 使用半透明红色，使形状更加明显
                    _attackShapeRenderer.DrawAttackShape(selectedEvent.Attack.Shape, _eventEditor.Position, _eventEditor.Scale, Color.Red * 0.5f, true);
                }
                finally
                {
                    // 恢复原始视口
                    GraphicsDevice.Viewport = new Viewport(originalViewport);
                }
            }

            // 绘制UI管理器
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            _uiManager.Draw(_spriteBatch);
            _spriteBatch.End();

            // 绘制左侧面板
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            _leftPanel.Draw(_spriteBatch);
            _spriteBatch.End();

            // 绘制属性编辑面板
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            _propertyPanel.Draw(_spriteBatch);
            _spriteBatch.End();

            // 绘制视口控件
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            _viewport.Draw(_spriteBatch);
            _spriteBatch.End();

            // 绘制时间轴控件
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            _timelineControl.Draw(_spriteBatch);
            _spriteBatch.End();

            // 更新左侧面板信息
            _leftPanel.UpdateInfo(
                _eventEditor.CurrentTime,
                _eventEditor.AnimationDuration,
                _eventEditor.CurrentAnimation,
                _eventEditor.Events.Count,
                _eventEditor.Scale
            );

            // 绘制Toast提示
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            _toast.Draw(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// 更新UI布局以适应窗口大小
        /// </summary>
        private void UpdateUILayout()
        {
            // 更新时间轴控件的位置和大小
            _timelineControl.SetBounds(new Rectangle(0, GraphicsDevice.Viewport.Height - 200,
                GraphicsDevice.Viewport.Width, 200));

            // 更新左侧面板的位置和大小
            int leftPanelWidth = 280;
            _leftPanel.SetBounds(new Rectangle(0, 0, leftPanelWidth, GraphicsDevice.Viewport.Height - 200));

            // 更新属性面板的位置和大小
            _propertyPanel.SetBounds(new Rectangle(GraphicsDevice.Viewport.Width - 300, 0,
                300, GraphicsDevice.Viewport.Height - 200));

            // 更新视口的位置和大小
            Rectangle viewportBounds = new Rectangle(leftPanelWidth, 0,
                GraphicsDevice.Viewport.Width - leftPanelWidth - 300,
                GraphicsDevice.Viewport.Height - 200);
            _viewport.SetBounds(viewportBounds);

            // 更新Spine动画的位置 - 设置为屏幕中心，而不是视口中心
            // 这样在SpineViewportGUI中绘制时，动画会显示在视口中心
            _eventEditor.Position = new Vector2(
                GraphicsDevice.Viewport.Width / 2,
                GraphicsDevice.Viewport.Height / 2);
        }

        /// <summary>
        /// 绘制背景网格
        /// </summary>
        private void DrawGrid()
        {
            // 获取视口边界
            Rectangle viewportBounds = _viewport.Bounds;

            // 创建SpriteBatch
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            // 绘制背景
            _spriteBatch.Draw(TextureManager.Pixel, viewportBounds, new Color(30, 30, 40));

            // 网格大小
            int gridSize = 50;

            // 绘制水平网格线
            for (int y = viewportBounds.Y; y <= viewportBounds.Y + viewportBounds.Height; y += gridSize)
            {
                _spriteBatch.Draw(TextureManager.Pixel,
                    new Rectangle(viewportBounds.X, y, viewportBounds.Width, 1),
                    new Color(50, 50, 60, 100));
            }

            // 绘制垂直网格线
            for (int x = viewportBounds.X; x <= viewportBounds.X + viewportBounds.Width; x += gridSize)
            {
                _spriteBatch.Draw(TextureManager.Pixel,
                    new Rectangle(x, viewportBounds.Y, 1, viewportBounds.Height),
                    new Color(50, 50, 60, 100));
            }

            // 绘制中心十字线
            int centerX = viewportBounds.X + viewportBounds.Width / 2;
            int centerY = viewportBounds.Y + viewportBounds.Height / 2;

            _spriteBatch.Draw(TextureManager.Pixel,
                new Rectangle(centerX, viewportBounds.Y, 1, viewportBounds.Height),
                new Color(100, 100, 120, 150));

            _spriteBatch.Draw(TextureManager.Pixel,
                new Rectangle(viewportBounds.X, centerY, viewportBounds.Width, 1),
                new Color(100, 100, 120, 150));

            _spriteBatch.End();
        }

        /// <summary>
        /// 保存事件
        /// </summary>
        private void SaveEvents()
        {
            try
            {
                // 获取事件文件路径
                string eventsFilePath = _currentFilePath;
                if (string.IsNullOrEmpty(Path.GetDirectoryName(eventsFilePath)))
                {
                    // 如果没有目录，则保存到当前目录
                    eventsFilePath = Path.Combine(Directory.GetCurrentDirectory(), eventsFilePath);
                }

                // 序列化事件
                string json = JsonSerializer.Serialize(_eventEditor.Events, _jsonOptions);

                // 保存到文件
                File.WriteAllText(eventsFilePath, json);

                // 显示保存成功提示
                _toast.Show($"事件已保存到 {eventsFilePath}", 3.0f);
            }
            catch (Exception ex)
            {
                // 显示保存失败提示
                _toast.Show($"保存事件失败: {ex.Message}", 3.0f);
            }
        }
    }
}
