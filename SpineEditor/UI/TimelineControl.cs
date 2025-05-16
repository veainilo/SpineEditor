using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpineEditor.UI
{
    /// <summary>
    /// 时间轴控件，用于显示和编辑帧事件
    /// </summary>
    public class TimelineControl
    {
        private SpineEventEditor _eventEditor;
        private float _duration;
        private float _zoom = 1.0f;
        private float _scrollPosition = 0;
        private Rectangle _bounds;
        private Texture2D _background;
        private Texture2D _eventMarker;
        private Texture2D _playheadMarker;
        private Texture2D _pixel;
        private SpriteFont _font;
        private FrameEvent _selectedEvent;
        private MouseState _prevMouseState;
        private bool _isDraggingPlayhead = false;
        private bool _isDraggingEvent = false;
        private float _dragOffset = 0;
        private ContextMenu _contextMenu;
        private float _contextMenuTime;

        /// <summary>
        /// 获取选中的事件
        /// </summary>
        public FrameEvent SelectedEvent => _selectedEvent;

        /// <summary>
        /// 事件选中委托
        /// </summary>
        public event EventHandler<FrameEvent> OnEventSelected;

        /// <summary>
        /// 创建时间轴控件
        /// </summary>
        /// <param name="eventEditor">Spine 事件编辑器</param>
        /// <param name="graphicsDevice">图形设备</param>
        /// <param name="font">字体</param>
        public TimelineControl(SpineEventEditor eventEditor, GraphicsDevice graphicsDevice, SpriteFont font)
        {
            _eventEditor = eventEditor;
            _font = font;

            // 创建纹理
            _background = new Texture2D(graphicsDevice, 1, 1);
            _background.SetData(new[] { new Color(50, 50, 50) });

            _pixel = new Texture2D(graphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });

            // 创建事件标记纹理
            _eventMarker = new Texture2D(graphicsDevice, 10, 20);
            Color[] eventMarkerData = new Color[10 * 20];
            for (int y = 0; y < 20; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    if (y < 10 && (x == 0 || x == 9 || y == 0 || y == 9))
                        eventMarkerData[y * 10 + x] = Color.Red;
                    else if (y >= 10 && x >= 2 && x <= 7)
                        eventMarkerData[y * 10 + x] = Color.Red;
                    else
                        eventMarkerData[y * 10 + x] = Color.Transparent;
                }
            }
            _eventMarker.SetData(eventMarkerData);

            // 创建播放头标记纹理
            _playheadMarker = new Texture2D(graphicsDevice, 9, 15);
            Color[] playheadMarkerData = new Color[9 * 15];
            for (int y = 0; y < 15; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    if (y < 9 && x == 4)
                        playheadMarkerData[y * 9 + x] = Color.Yellow;
                    else if (y >= 9 && x >= 0 && x <= 8 && y - 9 <= 8 - x && y - 9 <= x)
                        playheadMarkerData[y * 9 + x] = Color.Yellow;
                    else
                        playheadMarkerData[y * 9 + x] = Color.Transparent;
                }
            }
            _playheadMarker.SetData(playheadMarkerData);

            // 创建上下文菜单
            _contextMenu = new ContextMenu(graphicsDevice, font);
            MenuItem addEventItem = _contextMenu.AddItem("Add Event");
            MenuItem deleteEventItem = _contextMenu.AddItem("Delete Event");

            // 设置菜单项点击事件
            addEventItem.Click += (sender, e) => {
                // 在上下文菜单位置添加新事件
                _selectedEvent = new FrameEvent("New Event", _contextMenuTime);
                _eventEditor.AddEvent(_selectedEvent.Name, _selectedEvent.Time);
                OnEventSelected?.Invoke(this, _selectedEvent);
            };

            deleteEventItem.Click += (sender, e) => {
                // 删除选中的事件
                if (_selectedEvent != null)
                {
                    int index = _eventEditor.Events.IndexOf(_selectedEvent);
                    if (index >= 0)
                    {
                        _eventEditor.RemoveEvent(index);
                        _selectedEvent = null;
                        OnEventSelected?.Invoke(this, null);
                    }
                }
            };

            _prevMouseState = Mouse.GetState();
        }

        /// <summary>
        /// 更新时间轴控件
        /// </summary>
        /// <param name="gameTime">游戏时间</param>
        public void Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();
            KeyboardState keyboardState = Keyboard.GetState();

            // 处理鼠标滚轮缩放
            if (_bounds.Contains(mouseState.Position))
            {
                if (mouseState.ScrollWheelValue != _prevMouseState.ScrollWheelValue)
                {
                    float zoomDelta = (mouseState.ScrollWheelValue - _prevMouseState.ScrollWheelValue) / 120.0f * 0.1f;
                    float mouseTimePosition = TimeFromX(mouseState.X);

                    _zoom = MathHelper.Clamp(_zoom + zoomDelta, 0.1f, 10.0f);

                    // 调整滚动位置，使鼠标下的时间点保持不变
                    float newMouseX = XFromTime(mouseTimePosition);
                    _scrollPosition += mouseState.X - newMouseX;
                }
            }

            // 处理键盘滚动
            if (keyboardState.IsKeyDown(Keys.Left))
                _scrollPosition -= 5;
            if (keyboardState.IsKeyDown(Keys.Right))
                _scrollPosition += 5;

            // 确保滚动位置不会超出范围
            _scrollPosition = MathHelper.Clamp(_scrollPosition, 0, Math.Max(0, _bounds.Width * _zoom - _bounds.Width));

            // 处理鼠标拖动播放头和时间轴点击
            if (_bounds.Contains(mouseState.Position))
            {
                float clickTime = TimeFromX(mouseState.X);

                // 如果鼠标左键按下
                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    // 检查是否点击了播放头或者正在拖动播放头
                    float playheadX = XFromTime(_eventEditor.CurrentTime);

                    // 增加播放头点击区域，使其更容易点击
                    bool clickedPlayhead = Math.Abs(mouseState.X - playheadX) < 10;

                    // 检查是否点击了时间轴的上半部分（用于拖动播放头）
                    bool clickedTimelineTop = mouseState.Y < _bounds.Y + _bounds.Height / 2;

                    if (clickedPlayhead || _isDraggingPlayhead || clickedTimelineTop)
                    {
                        // 设置为拖动播放头模式
                        _isDraggingPlayhead = true;

                        // 更新当前时间
                        _eventEditor.CurrentTime = MathHelper.Clamp(clickTime, 0, _duration);
                    }
                    // 检查是否点击了事件标记
                    else if (_prevMouseState.LeftButton == ButtonState.Released)
                    {
                        bool eventClicked = false;
                        foreach (var evt in _eventEditor.Events)
                        {
                            float eventX = XFromTime(evt.Time);
                            // 增加事件标记点击区域
                            if (Math.Abs(mouseState.X - eventX) < 10)
                            {
                                _selectedEvent = evt;
                                _isDraggingEvent = true;
                                _dragOffset = mouseState.X - eventX;
                                OnEventSelected?.Invoke(this, _selectedEvent);
                                eventClicked = true;
                                break;
                            }
                        }
                    }
                }

                // 如果鼠标单击（按下后释放），直接设置当前时间
                if (_prevMouseState.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released)
                {
                    // 如果不是在拖动事件，则设置当前时间
                    if (!_isDraggingEvent)
                    {
                        _eventEditor.CurrentTime = MathHelper.Clamp(clickTime, 0, _duration);
                    }
                }

                // 处理右键点击，显示上下文菜单
                if (_prevMouseState.RightButton == ButtonState.Released && mouseState.RightButton == ButtonState.Pressed)
                {
                    // 保存右键点击位置的时间
                    _contextMenuTime = clickTime;

                    // 显示上下文菜单
                    _contextMenu.Show(new Vector2(mouseState.X, mouseState.Y));
                }
            }

            // 更新上下文菜单
            _contextMenu.Update();

            // 如果鼠标释放，重置拖动状态
            if (mouseState.LeftButton == ButtonState.Released)
            {
                _isDraggingPlayhead = false;
            }

            // 处理事件拖动
            if (_isDraggingEvent && mouseState.LeftButton == ButtonState.Pressed && _selectedEvent != null)
            {
                float newTime = TimeFromX(mouseState.X - _dragOffset);
                _selectedEvent.Time = MathHelper.Clamp(newTime, 0, _duration);

                // 重新排序事件
                _eventEditor.Events.Sort((a, b) => a.Time.CompareTo(b.Time));
            }
            else
            {
                _isDraggingEvent = false;
            }

            // 处理删除事件
            if (_selectedEvent != null && keyboardState.IsKeyDown(Keys.Delete) && !keyboardState.IsKeyDown(Keys.LeftControl))
            {
                int index = _eventEditor.Events.IndexOf(_selectedEvent);
                if (index >= 0)
                {
                    _eventEditor.RemoveEvent(index);
                    _selectedEvent = null;
                    OnEventSelected?.Invoke(this, null);
                }
            }

            _prevMouseState = mouseState;
        }

        /// <summary>
        /// 绘制时间轴控件
        /// </summary>
        /// <param name="spriteBatch">精灵批处理</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            // 绘制背景
            spriteBatch.Draw(_background, _bounds, Color.White);

            // 绘制时间刻度
            float timeStep = GetTimeStep();
            for (float t = 0; t <= _duration; t += timeStep)
            {
                float x = XFromTime(t);
                if (x >= _bounds.X && x <= _bounds.X + _bounds.Width)
                {
                    // 绘制刻度线
                    spriteBatch.Draw(_pixel, new Rectangle((int)x, _bounds.Y + 20, 1, 10), Color.Gray);

                    // 绘制时间文本
                    string timeText = t.ToString("0.00");
                    Vector2 textSize = _font.MeasureString(timeText);
                    spriteBatch.DrawString(_font, timeText, new Vector2(x - textSize.X / 2, _bounds.Y + 2), Color.White);
                }
            }

            // 绘制事件标记
            foreach (var evt in _eventEditor.Events)
            {
                float x = XFromTime(evt.Time);
                if (x >= _bounds.X - 10 && x <= _bounds.X + _bounds.Width + 10)
                {
                    Color color = (evt == _selectedEvent) ? Color.Yellow : Color.White;
                    spriteBatch.Draw(_eventMarker, new Vector2(x - 5, _bounds.Y + 30), color);

                    // 绘制事件名称
                    Vector2 textSize = _font.MeasureString(evt.Name);
                    spriteBatch.DrawString(_font, evt.Name, new Vector2(x - textSize.X / 2, _bounds.Y + 55), color);
                }
            }

            // 绘制当前播放位置
            float currentX = XFromTime(_eventEditor.CurrentTime);
            if (currentX >= _bounds.X - 5 && currentX <= _bounds.X + _bounds.Width + 5)
            {
                spriteBatch.Draw(_playheadMarker, new Vector2(currentX - 4, _bounds.Y + 5), Color.White);
                spriteBatch.Draw(_pixel, new Rectangle((int)currentX, _bounds.Y + 20, 1, _bounds.Height - 20), Color.Yellow);
            }

            // 绘制上下文菜单
            _contextMenu.Draw(spriteBatch);
        }

        /// <summary>
        /// 设置时间轴的边界
        /// </summary>
        /// <param name="bounds">边界矩形</param>
        public void SetBounds(Rectangle bounds)
        {
            _bounds = bounds;
        }

        /// <summary>
        /// 设置动画时长
        /// </summary>
        /// <param name="duration">时长（秒）</param>
        public void SetDuration(float duration)
        {
            _duration = duration;
        }

        /// <summary>
        /// 将时间转换为 X 坐标
        /// </summary>
        /// <param name="time">时间</param>
        /// <returns>X 坐标</returns>
        private float XFromTime(float time)
        {
            return _bounds.X + (time / _duration) * _bounds.Width * _zoom - _scrollPosition;
        }

        /// <summary>
        /// 将 X 坐标转换为时间
        /// </summary>
        /// <param name="x">X 坐标</param>
        /// <returns>时间</returns>
        private float TimeFromX(float x)
        {
            return ((x + _scrollPosition - _bounds.X) / (_bounds.Width * _zoom)) * _duration;
        }

        /// <summary>
        /// 获取时间刻度步长
        /// </summary>
        /// <returns>时间步长</returns>
        private float GetTimeStep()
        {
            // 根据缩放级别调整时间步长
            if (_zoom <= 0.2f) return 5.0f;
            if (_zoom <= 0.5f) return 2.0f;
            if (_zoom <= 1.0f) return 1.0f;
            if (_zoom <= 2.0f) return 0.5f;
            if (_zoom <= 5.0f) return 0.2f;
            return 0.1f;
        }
    }
}
