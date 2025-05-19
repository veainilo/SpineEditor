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

        // 攻击形状渲染器
        private AttackShapeRenderer _attackShapeRenderer;

        // 保存事件相关
        private bool _isSavingEvents = false;
        private string _currentFilePath = "events.json";

        // UI 元素
        private LeftPanel _leftPanel;

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

            // 订阅文件拖放事件
            Window.FileDrop += Window_FileDrop;
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

                // 创建攻击形状渲染器
                _attackShapeRenderer = new AttackShapeRenderer(GraphicsDevice);
                Console.WriteLine("创建攻击形状渲染器成功");

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

            // 创建左侧面板
            int leftPanelWidth = 280; // 增加面板宽度
            _leftPanel = new LeftPanel(GraphicsDevice, _font,
                new Rectangle(0, 0, leftPanelWidth, GraphicsDevice.Viewport.Height - 200));

            // 设置初始动画列表
            _leftPanel.SetAnimations(_eventEditor.AnimationNames);

            // 设置初始选中的动画
            if (_eventEditor.AnimationNames.Length > 0)
            {
                _leftPanel.AnimationList.SelectedIndex = 0;
            }

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

            // 设置事件选中处理
            _timelineControl.OnEventSelected += (sender, evt) => {
                _propertyPanel.SetSelectedEvent(evt);
            };

            // 禁用时间轴上的右键菜单，改为使用属性面板上的按钮
            _timelineControl.DisableContextMenu();

            // 设置时间变化处理
            _timelineControl.OnTimeChanged += (sender, time) => {
                _eventEditor.CurrentTime = time;
            };

            // 设置事件触发处理
            _eventEditor.OnEventTriggered += (sender, evt) => {
                Console.WriteLine($"事件触发: {evt.Name}, 时间: {evt.Time}, 整数值: {evt.IntValue}, 浮点值: {evt.FloatValue}, 字符串值: {evt.StringValue}");
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
                    _timelineControl.GetEvents().Clear();
                    foreach (var evt in _eventEditor.Events)
                    {
                        _timelineControl.AddEvent(evt);
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

            // 检测Ctrl+S快捷键保存帧事件
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.LeftControl) && keyboardState.IsKeyDown(Keys.S))
            {
                // 防止连续触发，使用简单的防抖
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
            bool isMouseOverTimeline = _timelineControl.Bounds.Contains(mouseState.Position);

            // 检查鼠标是否在左侧面板区域内
            bool isMouseOverLeftPanel = false;
            if (_leftPanel != null)
            {
                // 检查左侧面板区域
                isMouseOverLeftPanel = _leftPanel.Bounds.Contains(mouseState.Position);
            }

            // 检查鼠标是否在右侧属性面板区域内
            bool isMouseOverPropertyPanel = false;
            if (_propertyPanel != null)
            {
                isMouseOverPropertyPanel = _propertyPanel.Bounds.Contains(mouseState.Position);
            }

            // 更新 UI 管理器
            _uiManager.Update(gameTime);

            // 更新左侧面板
            _leftPanel.Update(gameTime);

            // 更新视口控件，只有当鼠标不在时间轴控件范围内且不在左侧面板区域内时才处理滚轮事件
            _viewport.Update(gameTime, !isMouseOverTimeline && !isMouseOverLeftPanel && !isMouseOverPropertyPanel);

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

            // 绘制攻击形状（如果有选中的攻击事件）
            DrawSelectedAttackShape();

            // 绘制 UI 管理器（包含新的时间轴控件）
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, null);
            _uiManager.Draw(_spriteBatch);
            _spriteBatch.End();

            // 绘制左侧面板
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, null);
            _leftPanel.Draw(_spriteBatch);
            _spriteBatch.End();

            // 绘制视口信息
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, null);
            _viewport.DrawInfo(_spriteBatch);
            _spriteBatch.End();

            // 绘制属性编辑面板（最后绘制，确保在最上层）
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, null);
            _propertyPanel.Draw(_spriteBatch);
            _spriteBatch.End();

            // 更新左侧面板信息
            _leftPanel.UpdateInfo(
                _eventEditor.CurrentTime,
                _eventEditor.AnimationDuration,
                _eventEditor.CurrentAnimation,
                _eventEditor.Events.Count,
                _eventEditor.Scale
            );

            base.Draw(gameTime);
        }

        /// <summary>
        /// 处理文件拖放事件
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件参数</param>
        private void Window_FileDrop(object sender, FileDropEventArgs e)
        {
            // 检查拖放的文件是否是Spine文件
            string atlasFile = null;
            string skelFile = null;

            foreach (string file in e.Files)
            {
                string extension = Path.GetExtension(file).ToLower();
                if (extension == ".atlas")
                {
                    atlasFile = file;
                }
                else if (extension == ".skel" || extension == ".json")
                {
                    skelFile = file;
                }
            }

            // 如果只找到了skel/json文件，尝试自动查找对应的atlas文件
            if (atlasFile == null && skelFile != null)
            {
                string directory = Path.GetDirectoryName(skelFile);
                string fileNameWithoutExt = Path.GetFileNameWithoutExtension(skelFile);

                // 尝试在同一目录下查找同名的atlas文件
                string possibleAtlasFile = Path.Combine(directory, fileNameWithoutExt + ".atlas");
                if (File.Exists(possibleAtlasFile))
                {
                    atlasFile = possibleAtlasFile;
                    Console.WriteLine($"自动找到atlas文件: {atlasFile}");
                }
                else
                {
                    // 尝试查找目录中的任何atlas文件
                    string[] atlasFiles = Directory.GetFiles(directory, "*.atlas");
                    if (atlasFiles.Length > 0)
                    {
                        atlasFile = atlasFiles[0];
                        Console.WriteLine($"在目录中找到atlas文件: {atlasFile}");
                    }
                }
            }

            // 如果找到了.atlas和.skel/.json文件，则加载它们
            if (atlasFile != null && skelFile != null)
            {
                // 设置当前文件路径为skel文件所在目录下的同名json文件
                string directory = Path.GetDirectoryName(skelFile);
                string fileNameWithoutExt = Path.GetFileNameWithoutExtension(skelFile);
                _currentFilePath = Path.Combine(directory, fileNameWithoutExt + "_events.json");

                LoadSpineAnimation(atlasFile, skelFile);
            }
        }

        /// <summary>
        /// 加载Spine动画
        /// </summary>
        /// <param name="atlasPath">Atlas文件路径</param>
        /// <param name="skelPath">Skeleton文件路径</param>
        private void LoadSpineAnimation(string atlasPath, string skelPath)
        {
            // 如果已经加载了动画，先清除现有的动画
            if (_eventEditor != null)
            {
                // 加载新的Spine动画
                bool success = _eventEditor.LoadAnimation(
                    atlasPath,
                    skelPath,
                    0.5f,
                    new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2)
                );

                if (success)
                {
                    Console.WriteLine($"成功加载Spine动画: {atlasPath}, {skelPath}");

                    // 更新动画列表
                    _leftPanel.SetAnimations(_eventEditor.AnimationNames);

                    // 设置初始选中的动画
                    if (_eventEditor.AnimationNames.Length > 0)
                    {
                        _leftPanel.AnimationList.SelectedIndex = 0;
                    }

                    // 获取动画时长
                    if (_eventEditor.AnimationState != null && _eventEditor.AnimationNames.Length > 0)
                    {
                        string animName = _eventEditor.AnimationNames[0];
                        _eventEditor.PlayAnimation(animName, true);
                        _eventEditor.IsPlaying = false; // 初始暂停
                        float duration = _eventEditor.AnimationDuration;
                        _timelineControl.SetDuration(duration);

                        // 尝试加载事件文件
                        if (!string.IsNullOrEmpty(_currentFilePath) && File.Exists(_currentFilePath))
                        {
                            if (_eventEditor.LoadEventsFromJson(_currentFilePath))
                            {
                                Console.WriteLine($"自动加载事件文件成功: {_currentFilePath}");
                            }
                        }
                        else
                        {
                            // 尝试查找与skel文件同目录下的事件文件
                            string directory = Path.GetDirectoryName(skelPath);
                            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(skelPath);
                            string possibleEventFile = Path.Combine(directory, fileNameWithoutExt + "_events.json");

                            if (File.Exists(possibleEventFile))
                            {
                                _currentFilePath = possibleEventFile;
                                if (_eventEditor.LoadEventsFromJson(_currentFilePath))
                                {
                                    Console.WriteLine($"自动加载事件文件成功: {_currentFilePath}");
                                }
                            }
                        }

                        // 更新时间轴上的事件
                        _timelineControl.GetEvents().Clear();
                        foreach (var evt in _eventEditor.Events)
                        {
                            _timelineControl.AddEvent(evt);
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"加载Spine动画失败: {atlasPath}, {skelPath}");
                }
            }
        }

        /// <summary>
        /// 保存事件数据
        /// </summary>
        private void SaveEvents()
        {
            if (_eventEditor != null && !string.IsNullOrEmpty(_currentFilePath))
            {
                string animationName = _eventEditor.CurrentAnimation;
                if (!string.IsNullOrEmpty(animationName))
                {
                    _eventEditor.SaveEventsToJson(_currentFilePath, animationName);
                    Console.WriteLine($"已保存事件数据到: {_currentFilePath}");

                    // 显示保存成功提示
                    // TODO: 添加UI提示
                }
            }
        }

        /// <summary>
        /// 绘制选中的攻击形状
        /// </summary>
        private void DrawSelectedAttackShape()
        {
            // 获取当前选中的事件
            FrameEvent selectedEvent = _propertyPanel.SelectedEvent;

            // 检查是否有选中的事件，且是攻击类型
            if (selectedEvent != null && selectedEvent.EventType == EventType.Attack && selectedEvent.Attack != null)
            {
                // 获取攻击形状
                AttackShape shape = selectedEvent.Attack.Shape;
                if (shape != null)
                {
                    // 获取当前动画的位置和缩放
                    // 注意：Position是Spine动画的原点位置，攻击形状坐标是相对于此原点的
                    Vector2 position = _eventEditor.Position;
                    float scale = _eventEditor.Scale;

                    // 设置拖拽处理器的当前形状
                    _attackShapeRenderer.DragHandler.CurrentShape = shape;

                    // 绘制攻击形状
                    // 使用半透明红色，使形状更加明显
                    Color shapeColor = new Color(255, 0, 0, 128);
                    _attackShapeRenderer.DrawAttackShape(shape, position, scale, shapeColor, true);

                    // 更新拖拽处理器
                    _attackShapeRenderer.DragHandler.Update(position, scale);
                }
            }
            else
            {
                // 如果没有选中攻击形状，清除拖拽处理器的当前形状
                _attackShapeRenderer.DragHandler.CurrentShape = null;
            }
        }
    }
}
