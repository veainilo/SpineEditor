using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpineEditor.Events;
using SpineEditor.UI.UISystem;
using System;
using System.Collections.Generic;

namespace SpineEditor.UI.GUILayoutComponents
{
    /// <summary>
    /// 基于GUILayout系统的时间轴控件
    /// </summary>
    public class TimelineControlGUI : GUILayoutPanel
    {
        // 时间轴属性
        private float _duration = 1.0f;
        private float _currentTime = 0.0f;
        private float _zoom = 1.0f;
        private float _scrollPosition = 0.0f;
        private List<FrameEvent> _events = new List<FrameEvent>();
        private FrameEvent _selectedEvent = null;

        // 纹理
        private Texture2D _eventMarker;
        private Texture2D _playheadMarker;
        private Texture2D _gridTexture;

        // 鼠标状态
        private MouseState _prevMouseState;
        private bool _isDraggingPlayhead = false;
        private bool _isDraggingEvent = false;
        private float _dragOffset = 0.0f;

        // 事件
        public event EventHandler<FrameEvent> EventSelected;
        public event EventHandler<float> TimeChanged;

        /// <summary>
        /// 获取或设置当前时间
        /// </summary>
        public float CurrentTime
        {
            get => _currentTime;
            set
            {
                if (_currentTime != value)
                {
                    _currentTime = MathHelper.Clamp(value, 0, _duration);
                    TimeChanged?.Invoke(this, _currentTime);
                }
            }
        }

        /// <summary>
        /// 获取选中的事件
        /// </summary>
        public FrameEvent SelectedEvent => _selectedEvent;

        /// <summary>
        /// 获取所有事件
        /// </summary>
        public List<FrameEvent> Events => _events;

        /// <summary>
        /// 创建时间轴控件
        /// </summary>
        /// <param name="title">面板标题</param>
        /// <param name="bounds">面板边界</param>
        /// <param name="graphicsDevice">图形设备</param>
        /// <param name="font">字体</param>
        public TimelineControlGUI(string title, Rectangle bounds, GraphicsDevice graphicsDevice, SpriteFont font)
            : base(title, bounds, graphicsDevice, font)
        {
            // 创建事件标记纹理
            _eventMarker = CreateEventMarker(graphicsDevice);

            // 创建播放头标记纹理
            _playheadMarker = CreatePlayheadMarker(graphicsDevice);

            // 创建网格纹理
            _gridTexture = CreateGridTexture(graphicsDevice);

            // 设置面板背景色
            BackgroundColor = new Color(30, 30, 35);
        }

        /// <summary>
        /// 设置动画时长
        /// </summary>
        /// <param name="duration">动画时长</param>
        public void SetDuration(float duration)
        {
            _duration = Math.Max(0.1f, duration);
        }

        /// <summary>
        /// 添加事件
        /// </summary>
        /// <param name="name">事件名称</param>
        /// <param name="time">事件时间</param>
        /// <returns>添加的事件</returns>
        public FrameEvent AddEvent(string name, float time)
        {
            FrameEvent newEvent = new FrameEvent(name, time, 0, 0, "");
            _events.Add(newEvent);
            _events.Sort((a, b) => a.Time.CompareTo(b.Time));
            return newEvent;
        }

        /// <summary>
        /// 移除事件
        /// </summary>
        /// <param name="index">事件索引</param>
        public void RemoveEvent(int index)
        {
            if (index >= 0 && index < _events.Count)
            {
                if (_selectedEvent == _events[index])
                {
                    _selectedEvent = null;
                }
                _events.RemoveAt(index);
            }
        }

        /// <summary>
        /// 绘制GUI内容
        /// </summary>
        protected override void DrawGUI()
        {
            // 主面板
            UISystem.GUILayout.BeginVertical();

            // 时间刻度区域
            DrawTimeScale();

            // 事件轨道区域
            DrawEventTrack();

            UISystem.GUILayout.EndVertical();
        }

        /// <summary>
        /// 处理鼠标输入
        /// </summary>
        /// <param name="mouseState">当前鼠标状态</param>
        /// <param name="prevMouseState">上一帧鼠标状态</param>
        /// <returns>是否处理了输入</returns>
        protected override bool OnMouseInput(MouseState mouseState, MouseState prevMouseState)
        {
            // 检查是否在面板范围内
            if (!_bounds.Contains(mouseState.Position))
                return false;

            // 处理鼠标滚轮缩放
            if (mouseState.ScrollWheelValue != prevMouseState.ScrollWheelValue)
            {
                float zoomDelta = (mouseState.ScrollWheelValue - prevMouseState.ScrollWheelValue) / 120.0f * 0.1f;
                float mouseTimePosition = TimeFromX(mouseState.X);

                // 更新缩放值
                _zoom = MathHelper.Clamp(_zoom + zoomDelta, 0.1f, 10.0f);

                // 调整滚动位置，使鼠标下的时间点保持不变
                float newMouseX = XFromTime(mouseTimePosition);
                _scrollPosition += mouseState.X - newMouseX;

                // 确保滚动位置不会超出范围
                _scrollPosition = MathHelper.Clamp(_scrollPosition, 0, Math.Max(0, _bounds.Width * _zoom - _bounds.Width));

                return true;
            }

            // 处理鼠标左键点击和拖动
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                // 计算点击位置对应的时间
                float clickTime = TimeFromX(mouseState.X);

                // 检查是否点击了播放头或者正在拖动播放头
                float playheadX = XFromTime(_currentTime);
                bool clickedPlayhead = Math.Abs(mouseState.X - playheadX) < 10;

                // 检查是否点击了时间轴的上半部分（用于拖动播放头）
                bool clickedTimelineTop = mouseState.Y < _bounds.Y + 40;

                if (clickedPlayhead || _isDraggingPlayhead || clickedTimelineTop)
                {
                    // 设置为拖动播放头模式
                    _isDraggingPlayhead = true;

                    // 更新当前时间
                    CurrentTime = MathHelper.Clamp(clickTime, 0, _duration);

                    return true;
                }

                // 检查是否点击了事件
                foreach (var evt in _events)
                {
                    float eventX = XFromTime(evt.Time);
                    if (Math.Abs(mouseState.X - eventX) < 10 && mouseState.Y > _bounds.Y + 40)
                    {
                        // 如果是第一次点击事件
                        if (prevMouseState.LeftButton == ButtonState.Released)
                        {
                            // 选中事件
                            _selectedEvent = evt;
                            EventSelected?.Invoke(this, evt);

                            // 设置拖动偏移量
                            _dragOffset = mouseState.X - eventX;
                            _isDraggingEvent = true;
                        }
                        else if (_isDraggingEvent && _selectedEvent == evt)
                        {
                            // 更新事件时间
                            float newTime = TimeFromX(mouseState.X - _dragOffset);
                            evt.Time = MathHelper.Clamp(newTime, 0, _duration);

                            // 重新排序事件
                            _events.Sort((a, b) => a.Time.CompareTo(b.Time));
                        }

                        return true;
                    }
                }

                // 如果没有点击事件，但点击了事件轨道区域，则添加新事件
                if (mouseState.Y > _bounds.Y + 40 && prevMouseState.LeftButton == ButtonState.Released)
                {
                    // 添加新事件
                    FrameEvent newEvent = AddEvent("New Event", clickTime);

                    // 选中新事件
                    _selectedEvent = newEvent;
                    EventSelected?.Invoke(this, newEvent);

                    return true;
                }
            }
            else
            {
                // 鼠标释放，重置拖动状态
                _isDraggingPlayhead = false;
                _isDraggingEvent = false;
            }

            return false;
        }

        /// <summary>
        /// 绘制时间刻度
        /// </summary>
        private void DrawTimeScale()
        {
            UISystem.GUILayout.BeginVertical(UISystem.GUILayout.Height(40));

            // 注意：实际的绘制在DrawPanelFrame方法中完成
            // 这里只是设置布局

            UISystem.GUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制事件轨道
        /// </summary>
        private void DrawEventTrack()
        {
            UISystem.GUILayout.BeginVertical(UISystem.GUILayout.Height(160));

            // 注意：实际的绘制在DrawPanelFrame方法中完成
            // 这里只是设置布局

            UISystem.GUILayout.EndVertical();
        }

        /// <summary>
        /// 将时间转换为X坐标
        /// </summary>
        /// <param name="time">时间</param>
        /// <returns>X坐标</returns>
        private float XFromTime(float time)
        {
            return _bounds.X + time * 100.0f * _zoom - _scrollPosition;
        }

        /// <summary>
        /// 将X坐标转换为时间
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <returns>时间</returns>
        private float TimeFromX(float x)
        {
            return (x - _bounds.X + _scrollPosition) / (100.0f * _zoom);
        }

        /// <summary>
        /// 绘制面板框架
        /// </summary>
        /// <param name="spriteBatch">精灵批处理</param>
        protected override void DrawPanelFrame(SpriteBatch spriteBatch)
        {
            base.DrawPanelFrame(spriteBatch);

            // 绘制网格
            DrawGrid(spriteBatch);

            // 绘制时间刻度
            DrawTimeMarkers(spriteBatch);

            // 绘制事件
            DrawEvents(spriteBatch);

            // 绘制播放头
            DrawPlayhead(spriteBatch);
        }

        /// <summary>
        /// 绘制网格
        /// </summary>
        private void DrawGrid(SpriteBatch spriteBatch)
        {
            // 绘制背景
            spriteBatch.Draw(_gridTexture, new Rectangle(_bounds.X, _bounds.Y + 40, _bounds.Width, _bounds.Height - 40), new Color(20, 20, 25));

            // 绘制垂直网格线
            float pixelsPerSecond = 100.0f * _zoom;
            float startTime = _scrollPosition / pixelsPerSecond;
            float endTime = (_scrollPosition + _bounds.Width) / pixelsPerSecond;

            // 计算主要时间刻度间隔（1秒、0.5秒、0.25秒等）
            float timeInterval = 1.0f;
            if (_zoom > 2.0f) timeInterval = 0.5f;
            if (_zoom > 4.0f) timeInterval = 0.25f;
            if (_zoom > 8.0f) timeInterval = 0.1f;

            // 计算起始时间（向下取整到最近的时间间隔）
            float firstTime = (float)Math.Floor(startTime / timeInterval) * timeInterval;

            // 绘制垂直网格线
            for (float time = firstTime; time <= endTime; time += timeInterval)
            {
                float x = XFromTime(time);
                if (x >= _bounds.X && x <= _bounds.X + _bounds.Width)
                {
                    // 主要时间刻度线使用更明显的颜色
                    Color lineColor;
                    int lineWidth = 1;

                    if (Math.Abs(time % 1.0f) < 0.01f)
                    {
                        // 整秒线
                        lineColor = new Color(100, 100, 140, 150);
                        lineWidth = 2;
                    }
                    else if (Math.Abs(time % 0.5f) < 0.01f)
                    {
                        // 半秒线
                        lineColor = new Color(80, 80, 120, 120);
                    }
                    else
                    {
                        // 其他刻度线
                        lineColor = new Color(60, 60, 80, 80);
                    }

                    // 绘制垂直线
                    spriteBatch.Draw(_gridTexture, new Rectangle((int)x - lineWidth/2, _bounds.Y + 40, lineWidth, _bounds.Height - 40), lineColor);
                }
            }

            // 绘制水平网格线
            for (int y = _bounds.Y + 40; y <= _bounds.Y + _bounds.Height; y += 40)
            {
                spriteBatch.Draw(_gridTexture, new Rectangle(_bounds.X, y, _bounds.Width, 1), new Color(60, 60, 80, 80));
            }

            // 绘制当前时间线
            float currentTimeX = XFromTime(_currentTime);
            if (currentTimeX >= _bounds.X && currentTimeX <= _bounds.X + _bounds.Width)
            {
                spriteBatch.Draw(_gridTexture, new Rectangle((int)currentTimeX, _bounds.Y + 40, 1, _bounds.Height - 40), new Color(0, 200, 255, 100));
            }
        }

        /// <summary>
        /// 绘制时间刻度
        /// </summary>
        private void DrawTimeMarkers(SpriteBatch spriteBatch)
        {
            // 绘制时间刻度背景
            spriteBatch.Draw(_gridTexture, new Rectangle(_bounds.X, _bounds.Y, _bounds.Width, 40), new Color(40, 40, 50));

            // 计算像素/秒比例
            float pixelsPerSecond = 100.0f * _zoom;
            float startTime = _scrollPosition / pixelsPerSecond;
            float endTime = (_scrollPosition + _bounds.Width) / pixelsPerSecond;

            // 计算主要时间刻度间隔（1秒、0.5秒、0.25秒等）
            float timeInterval = 1.0f;
            if (_zoom > 2.0f) timeInterval = 0.5f;
            if (_zoom > 4.0f) timeInterval = 0.25f;
            if (_zoom > 8.0f) timeInterval = 0.1f;

            // 计算起始时间（向下取整到最近的时间间隔）
            float firstTime = (float)Math.Floor(startTime / timeInterval) * timeInterval;

            // 绘制时间刻度
            for (float time = firstTime; time <= endTime; time += timeInterval)
            {
                float x = XFromTime(time);
                if (x >= _bounds.X && x <= _bounds.X + _bounds.Width)
                {
                    // 主要时间刻度线使用更明显的颜色
                    Color textColor = Math.Abs(time % 1.0f) < 0.01f
                        ? Color.White      // 整秒文本
                        : Color.LightGray; // 其他刻度文本

                    // 只在整秒和半秒处显示文本
                    if (Math.Abs(time % 0.5f) < 0.01f)
                    {
                        string timeText = time.ToString("0.0##");
                        Vector2 textSize = _font.MeasureString(timeText);
                        spriteBatch.DrawString(_font, timeText, new Vector2(x - textSize.X / 2, _bounds.Y + 10), textColor);
                    }

                    // 绘制刻度线
                    int tickHeight = Math.Abs(time % 1.0f) < 0.01f ? 10 : 5;
                    spriteBatch.Draw(_gridTexture, new Rectangle((int)x, _bounds.Y + 30, 1, tickHeight), textColor);
                }
            }
        }

        /// <summary>
        /// 绘制事件
        /// </summary>
        private void DrawEvents(SpriteBatch spriteBatch)
        {
            // 先绘制所有非选中的事件
            foreach (var evt in _events)
            {
                if (evt != _selectedEvent)
                {
                    DrawSingleEvent(spriteBatch, evt, false);
                }
            }

            // 最后绘制选中的事件，确保它显示在最上层
            if (_selectedEvent != null)
            {
                DrawSingleEvent(spriteBatch, _selectedEvent, true);
            }
        }

        /// <summary>
        /// 绘制单个事件
        /// </summary>
        private void DrawSingleEvent(SpriteBatch spriteBatch, FrameEvent evt, bool isSelected)
        {
            float x = XFromTime(evt.Time);
            if (x >= _bounds.X - 8 && x <= _bounds.X + _bounds.Width + 8)
            {
                // 选中的事件使用不同的颜色和大小
                Color eventColor = isSelected ? Color.Yellow : new Color(200, 200, 100);
                float scale = isSelected ? 1.2f : 1.0f;

                // 绘制事件标记背景（增强可见性）
                if (isSelected)
                {
                    // 绘制选中事件的背景光晕
                    spriteBatch.Draw(_gridTexture,
                        new Rectangle((int)x - 12, _bounds.Y + 56, 24, 24),
                        new Color(255, 255, 0, 50));
                }

                // 绘制事件标记
                spriteBatch.Draw(_eventMarker,
                    new Vector2(x - 8 * scale, _bounds.Y + 60),
                    null,
                    eventColor,
                    0f,
                    Vector2.Zero,
                    scale,
                    SpriteEffects.None,
                    0f);

                // 绘制事件名称 - 确保不同事件的文本不会重叠
                Vector2 textSize = _font.MeasureString(evt.Name);

                // 检查是否有足够的空间显示文本
                bool hasEnoughSpace = true;
                if (!isSelected) // 选中的事件总是显示名称
                {
                    foreach (var otherEvt in _events)
                    {
                        if (otherEvt != evt)
                        {
                            float otherX = XFromTime(otherEvt.Time);
                            // 如果两个事件太近，则不显示名称
                            if (Math.Abs(x - otherX) < textSize.X * 0.75f)
                            {
                                hasEnoughSpace = false;
                                break;
                            }
                        }
                    }
                }

                // 只有在有足够空间时才显示事件名称，或者是选中的事件
                if (hasEnoughSpace || isSelected)
                {
                    // 如果是选中的事件，绘制文本背景
                    if (isSelected)
                    {
                        Rectangle textBg = new Rectangle(
                            (int)(x - textSize.X / 2 - 5),
                            (int)(_bounds.Y + 78),
                            (int)(textSize.X + 10),
                            (int)(textSize.Y + 4));
                        spriteBatch.Draw(_gridTexture, textBg, new Color(0, 0, 0, 150));
                    }

                    // 绘制事件名称
                    spriteBatch.DrawString(_font,
                        evt.Name,
                        new Vector2(x - textSize.X / 2, _bounds.Y + 80),
                        isSelected ? Color.White : eventColor);
                }

                // 如果是选中的事件，绘制垂直指示线
                if (isSelected)
                {
                    spriteBatch.Draw(_gridTexture,
                        new Rectangle((int)x, _bounds.Y + 40, 1, _bounds.Height - 40),
                        new Color(255, 255, 0, 100));
                }
            }
        }

        /// <summary>
        /// 绘制播放头
        /// </summary>
        private void DrawPlayhead(SpriteBatch spriteBatch)
        {
            float x = XFromTime(_currentTime);
            if (x >= _bounds.X - 8 && x <= _bounds.X + _bounds.Width + 8)
            {
                // 绘制播放头标记
                spriteBatch.Draw(_playheadMarker, new Vector2(x - 8, _bounds.Y), Color.Cyan);

                // 绘制播放头线 - 使用更明显的颜色和宽度
                spriteBatch.Draw(_gridTexture, new Rectangle((int)x - 1, _bounds.Y, 3, _bounds.Height), new Color(0, 200, 255, 200));

                // 绘制播放头阴影效果
                spriteBatch.Draw(_gridTexture, new Rectangle((int)x - 5, _bounds.Y, 10, 5), new Color(0, 150, 200, 100));

                // 绘制当前时间 - 使用更明显的字体和背景
                string timeText = _currentTime.ToString("0.000");
                Vector2 textSize = _font.MeasureString(timeText);

                // 绘制文本背景
                Rectangle textBg = new Rectangle(
                    (int)(x - textSize.X / 2 - 5),
                    (int)(_bounds.Y + _bounds.Height - 25),
                    (int)(textSize.X + 10),
                    (int)(textSize.Y + 5));
                spriteBatch.Draw(_gridTexture, textBg, new Color(0, 100, 150, 200));

                // 绘制文本
                spriteBatch.DrawString(_font, timeText,
                    new Vector2(x - textSize.X / 2, _bounds.Y + _bounds.Height - 22),
                    Color.White);
            }
        }

        /// <summary>
        /// 创建事件标记纹理
        /// </summary>
        private Texture2D CreateEventMarker(GraphicsDevice graphicsDevice)
        {
            // 创建一个菱形标记
            Texture2D texture = new Texture2D(graphicsDevice, 16, 16);
            Color[] data = new Color[16 * 16];
            for (int i = 0; i < data.Length; i++)
                data[i] = Color.Transparent;

            // 绘制菱形
            for (int y = 0; y < 16; y++)
            {
                int width = 16 - Math.Abs(y - 8) * 2;
                int start = (16 - width) / 2;
                for (int x = start; x < start + width; x++)
                {
                    data[y * 16 + x] = Color.Yellow;
                }
            }

            texture.SetData(data);
            return texture;
        }

        /// <summary>
        /// 创建播放头标记纹理
        /// </summary>
        private Texture2D CreatePlayheadMarker(GraphicsDevice graphicsDevice)
        {
            // 创建一个三角形标记
            Texture2D texture = new Texture2D(graphicsDevice, 16, 16);
            Color[] data = new Color[16 * 16];
            for (int i = 0; i < data.Length; i++)
                data[i] = Color.Transparent;

            // 绘制三角形
            for (int y = 0; y < 8; y++)
            {
                int width = (y + 1) * 2;
                int start = (16 - width) / 2;
                for (int x = start; x < start + width; x++)
                {
                    data[y * 16 + x] = Color.Cyan;
                }
            }

            texture.SetData(data);
            return texture;
        }

        /// <summary>
        /// 创建网格纹理
        /// </summary>
        private Texture2D CreateGridTexture(GraphicsDevice graphicsDevice)
        {
            // 创建一个1x1的白色像素
            Texture2D texture = new Texture2D(graphicsDevice, 1, 1);
            texture.SetData(new[] { Color.White });
            return texture;
        }
    }
}
