using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpineEditor.Events;
using SpineEditor.Utils;

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

        // 固定网格相关
        private const int GRID_CELL_WIDTH = 50;  // 固定网格单元格宽度
        private const int GRID_CELL_HEIGHT = 30; // 固定网格单元格高度
        private bool _showFixedGrid = true;      // 是否显示固定网格

        /// <summary>
        /// 获取选中的事件
        /// </summary>
        public FrameEvent SelectedEvent => _selectedEvent;

        /// <summary>
        /// 是否启用上下文菜单
        /// </summary>
        private bool _contextMenuEnabled = true;

        /// <summary>
        /// 事件选中委托
        /// </summary>
        public event EventHandler<FrameEvent> OnEventSelected;

        /// <summary>
        /// 禁用上下文菜单
        /// </summary>
        public void DisableContextMenu()
        {
            _contextMenuEnabled = false;
        }

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

            // 初始化绘制工具类
            DrawingUtils.Initialize(graphicsDevice);

            // 创建背景纹理 - 使用更专业的深色背景
            _background = new Texture2D(graphicsDevice, 1, 1);
            _background.SetData(new[] { new Color(30, 30, 35) }); // 深蓝灰色背景

            // 使用绘制工具类的像素纹理
            _pixel = new Texture2D(graphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });

            // 创建事件标记纹理 - 使用更专业的菱形标记
            _eventMarker = new Texture2D(graphicsDevice, 16, 20);
            Color[] eventMarkerData = new Color[16 * 20];

            // 创建菱形标记
            for (int y = 0; y < 20; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    // 菱形形状
                    int centerX = 8;
                    int centerY = 10;
                    int distance = Math.Abs(x - centerX) + Math.Abs(y - centerY);

                    if (distance <= 7) // 菱形内部
                    {
                        if (distance == 7) // 菱形边缘
                            eventMarkerData[y * 16 + x] = new Color(255, 180, 0); // 橙色边框
                        else if (distance == 6) // 内边缘
                            eventMarkerData[y * 16 + x] = new Color(255, 200, 0); // 浅橙色
                        else // 内部填充
                            eventMarkerData[y * 16 + x] = new Color(255, 220, 0, 200); // 半透明黄色
                    }
                    else
                    {
                        eventMarkerData[y * 16 + x] = Color.Transparent;
                    }
                }
            }
            _eventMarker.SetData(eventMarkerData);

            // 创建播放头标记纹理 - 使用更专业的播放头
            _playheadMarker = new Texture2D(graphicsDevice, 15, 20);
            Color[] playheadMarkerData = new Color[15 * 20];

            // 创建三角形播放头
            for (int y = 0; y < 20; y++)
            {
                for (int x = 0; x < 15; x++)
                {
                    // 顶部线条
                    if (y == 0 && x >= 5 && x <= 9)
                    {
                        playheadMarkerData[y * 15 + x] = new Color(0, 200, 255); // 青色
                    }
                    // 垂直线条
                    else if (x == 7 && y <= 15)
                    {
                        playheadMarkerData[y * 15 + x] = new Color(0, 200, 255); // 青色
                    }
                    // 三角形
                    else if (y >= 15 && y < 20)
                    {
                        int distanceFromCenter = Math.Abs(x - 7);
                        if (distanceFromCenter <= (y - 15))
                        {
                            if (distanceFromCenter == (y - 15) || y == 19) // 边缘
                                playheadMarkerData[y * 15 + x] = new Color(0, 200, 255); // 青色边框
                            else // 内部填充
                                playheadMarkerData[y * 15 + x] = new Color(0, 180, 255, 200); // 半透明青色
                        }
                        else
                        {
                            playheadMarkerData[y * 15 + x] = Color.Transparent;
                        }
                    }
                    else
                    {
                        playheadMarkerData[y * 15 + x] = Color.Transparent;
                    }
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
                _selectedEvent = new FrameEvent("New Event", _contextMenuTime, 0, 0, "");
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

                    // 保存旧的缩放值，用于计算固定网格偏移量的调整
                    float oldZoom = _zoom;

                    // 更新缩放值
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

                // 处理右键点击，显示上下文菜单（如果启用）
                if (_contextMenuEnabled && _prevMouseState.RightButton == ButtonState.Released && mouseState.RightButton == ButtonState.Pressed)
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
            DrawingUtils.DrawTexture(spriteBatch, _background, _bounds, Color.White);

            // 绘制固定网格（不受缩放影响）- 放在最底层
            if (_showFixedGrid)
            {
                // 绘制垂直网格线 - 使用更淡的颜色，确保不干扰交互
                for (int x = _bounds.X; x <= _bounds.X + _bounds.Width; x += GRID_CELL_WIDTH)
                {
                    DrawingUtils.DrawVerticalLine(spriteBatch, x, _bounds.Y, _bounds.Height, new Color(50, 50, 60, 30));
                }

                // 绘制水平网格线 - 使用更淡的颜色，确保不干扰交互
                for (int y = _bounds.Y; y <= _bounds.Y + _bounds.Height; y += GRID_CELL_HEIGHT)
                {
                    DrawingUtils.DrawHorizontalLine(spriteBatch, _bounds.X, y, _bounds.Width, new Color(50, 50, 60, 30));
                }
            }

            // 绘制水平分隔线（在网格之上）
            DrawingUtils.DrawHorizontalLine(spriteBatch, _bounds.X, _bounds.Y + 20, _bounds.Width, new Color(60, 60, 70), 1);
            DrawingUtils.DrawHorizontalLine(spriteBatch, _bounds.X, _bounds.Y + 40, _bounds.Width, new Color(60, 60, 70), 1);
            DrawingUtils.DrawHorizontalLine(spriteBatch, _bounds.X, _bounds.Y + _bounds.Height - 1, _bounds.Width, new Color(60, 60, 70), 1);

            // 绘制时间刻度和动态网格线（受缩放影响）
            float timeStep = GetTimeStep();
            for (float t = 0; t <= _duration; t += timeStep)
            {
                float x = XFromTime(t);
                if (x >= _bounds.X && x <= _bounds.X + _bounds.Width)
                {
                    bool isMainTick = Math.Abs(t - Math.Round(t, 0)) < 0.001f; // 整数时间点

                    // 绘制刻度线 - 主刻度线更长更明显
                    if (isMainTick)
                    {
                        // 主刻度线
                        DrawingUtils.DrawVerticalLine(spriteBatch, (int)x, _bounds.Y + 20, 20, new Color(150, 150, 170));

                        // 垂直动态网格线 - 半透明（只在主刻度线处绘制）
                        DrawingUtils.DrawVerticalLine(spriteBatch, (int)x, _bounds.Y + 40, _bounds.Height - 40, new Color(80, 80, 100, 80));

                        // 绘制时间文本 - 只在主刻度线上显示
                        string timeText = t.ToString("0.00");
                        Vector2 textSize = _font.MeasureString(timeText);
                        spriteBatch.DrawString(_font, timeText, new Vector2(x - textSize.X / 2, _bounds.Y + 2), new Color(200, 200, 220));
                    }
                    else
                    {
                        // 次刻度线
                        DrawingUtils.DrawVerticalLine(spriteBatch, (int)x, _bounds.Y + 20, 10, new Color(100, 100, 120));
                    }
                }
            }

            // 绘制固定时间标记（不受缩放影响）- 放在动态网格之上，但在事件轨道之下
            if (_showFixedGrid)
            {
                // 每秒绘制一个固定时间标记
                for (float t = 0; t <= _duration; t += 1.0f)
                {
                    // 计算固定位置（不受缩放影响）
                    float fixedX = _bounds.X + (t / _duration) * _bounds.Width;

                    if (fixedX >= _bounds.X && fixedX <= _bounds.X + _bounds.Width)
                    {
                        // 绘制固定时间标记（小圆点）- 使用更淡的颜色，确保不干扰交互
                        DrawingUtils.DrawRectangle(
                            spriteBatch,
                            (int)fixedX - 1,
                            _bounds.Y + 40,
                            3,
                            3,
                            new Color(100, 100, 255, 50)
                        );
                    }
                }
            }

            // 绘制事件轨道背景
            Rectangle eventTrackRect = new Rectangle(_bounds.X, _bounds.Y + 40, _bounds.Width, 30);
            DrawingUtils.DrawRectangle(spriteBatch, eventTrackRect, new Color(40, 40, 50));

            // 绘制事件标记
            foreach (var evt in _eventEditor.Events)
            {
                float x = XFromTime(evt.Time);
                if (x >= _bounds.X - 10 && x <= _bounds.X + _bounds.Width + 10)
                {
                    // 根据事件类型选择不同的颜色
                    Color color;
                    switch (evt.EventType)
                    {
                        case EventType.Attack:
                            color = new Color(255, 100, 100); // 红色
                            break;
                        case EventType.Effect:
                            color = new Color(100, 255, 100); // 绿色
                            break;
                        case EventType.Sound:
                            color = new Color(100, 100, 255); // 蓝色
                            break;
                        default:
                            color = new Color(255, 220, 0);   // 黄色
                            break;
                    }

                    // 如果是选中的事件，使用更亮的颜色
                    if (evt == _selectedEvent)
                    {
                        color = Color.Lerp(color, Color.White, 0.5f);
                    }

                    // 绘制事件标记
                    DrawingUtils.DrawTexture(spriteBatch, _eventMarker, new Vector2(x - 8, _bounds.Y + 45), color);

                    // 绘制事件名称
                    Vector2 textSize = _font.MeasureString(evt.Name);

                    // 绘制文本背景以增强可读性
                    if (evt == _selectedEvent)
                    {
                        Rectangle textBgRect = new Rectangle(
                            (int)(x - textSize.X / 2 - 2),
                            _bounds.Y + 70,
                            (int)textSize.X + 4,
                            (int)textSize.Y + 2
                        );
                        DrawingUtils.DrawRectangle(spriteBatch, textBgRect, new Color(40, 40, 50, 200));
                    }

                    // 绘制事件名称文本
                    spriteBatch.DrawString(
                        _font,
                        evt.Name,
                        new Vector2(x - textSize.X / 2, _bounds.Y + 71),
                        evt == _selectedEvent ? Color.White : new Color(200, 200, 200)
                    );
                }
            }

            // 绘制当前时间文本
            string currentTimeText = _eventEditor.CurrentTime.ToString("0.000") + " s";
            Vector2 currentTimeSize = _font.MeasureString(currentTimeText);
            spriteBatch.DrawString(
                _font,
                currentTimeText,
                new Vector2(_bounds.X + _bounds.Width - currentTimeSize.X - 10, _bounds.Y + 2),
                new Color(0, 200, 255)
            );

            // 绘制当前播放位置
            float currentX = XFromTime(_eventEditor.CurrentTime);
            if (currentX >= _bounds.X - 5 && currentX <= _bounds.X + _bounds.Width + 5)
            {
                // 绘制播放头标记
                DrawingUtils.DrawTexture(spriteBatch, _playheadMarker, new Vector2(currentX - 7, _bounds.Y), Color.White);

                // 绘制播放头线条 - 使用渐变效果
                for (int i = 0; i < _bounds.Height - 20; i++)
                {
                    float alpha = 1.0f - (i / (float)(_bounds.Height - 20)) * 0.7f;
                    DrawingUtils.DrawRectangle(
                        spriteBatch,
                        (int)currentX - 1,
                        _bounds.Y + 20 + i,
                        2,
                        1,
                        new Color(0, 200, 255, (int)(255 * alpha))
                    );
                }
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
        /// 将时间转换为 X 坐标（考虑缩放和滚动）
        /// </summary>
        /// <param name="time">时间</param>
        /// <returns>X 坐标</returns>
        private float XFromTime(float time)
        {
            // 计算时间在总时长中的比例，然后乘以宽度和缩放，再减去滚动偏移
            return _bounds.X + (time / _duration) * _bounds.Width * _zoom - _scrollPosition;
        }

        /// <summary>
        /// 将 X 坐标转换为时间（考虑缩放和滚动）
        /// </summary>
        /// <param name="x">X 坐标</param>
        /// <returns>时间</returns>
        private float TimeFromX(float x)
        {
            // 将 X 坐标转换回时间（考虑滚动和缩放）
            return ((x + _scrollPosition - _bounds.X) / (_bounds.Width * _zoom)) * _duration;
        }

        /// <summary>
        /// 获取时间刻度步长
        /// </summary>
        /// <returns>时间步长</returns>
        private float GetTimeStep()
        {
            // 根据缩放级别和动画时长调整时间步长
            float baseDuration = Math.Max(1.0f, _duration);

            // 计算合适的步长，使得时间轴上的刻度数量适中
            float targetStepCount = 20.0f * _zoom; // 目标刻度数量随缩放增加
            float rawStep = baseDuration / targetStepCount;

            // 将步长规范化为易读的值：0.1, 0.2, 0.5, 1.0, 2.0, 5.0 等
            float magnitude = (float)Math.Pow(10, Math.Floor(Math.Log10(rawStep)));
            float normalizedStep = rawStep / magnitude;

            if (normalizedStep < 0.2f) return 0.1f * magnitude;
            if (normalizedStep < 0.5f) return 0.2f * magnitude;
            if (normalizedStep < 1.0f) return 0.5f * magnitude;
            if (normalizedStep < 2.0f) return 1.0f * magnitude;
            if (normalizedStep < 5.0f) return 2.0f * magnitude;
            return 5.0f * magnitude;
        }
    }
}
