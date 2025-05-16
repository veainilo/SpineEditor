using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpineEditor.Events;

namespace SpineEditor.UI.UISystem
{
    /// <summary>
    /// 完整的时间轴控件，包含网格、时间刻度、事件轨道和播放头
    /// </summary>
    public class TimelineControlNew : UIElement
    {
        // 子控件
        private GridPanel _gridPanel;
        private TimeScalePanel _timeScalePanel;
        private EventTrackPanel _eventTrackPanel;
        private PlayheadPanel _playheadPanel;

        // 属性
        public float Duration { get; private set; } = 1.0f;
        public float CurrentTime
        {
            get => _playheadPanel.CurrentTime;
            set => _playheadPanel.CurrentTime = value;
        }
        public FrameEvent SelectedEvent => _eventTrackPanel.SelectedEvent;

        // 事件
        public event EventHandler<FrameEvent> OnEventSelected;
        public event EventHandler<float> OnTimeChanged;

        public TimelineControlNew(GraphicsDevice graphicsDevice, SpriteFont font)
        {
            // 初始化纹理
            TextureManager.Initialize(graphicsDevice);

            // 创建子控件
            _gridPanel = new GridPanel
            {
                ZIndex = 0, // 最底层
                BackgroundColor = new Color(30, 30, 35)
            };

            _timeScalePanel = new TimeScalePanel
            {
                ZIndex = 2, // 在网格之上
                Font = font,
                BackgroundColor = Color.Transparent
            };

            _eventTrackPanel = new EventTrackPanel
            {
                ZIndex = 3, // 在时间刻度之上
                Font = font,
                BackgroundColor = new Color(40, 40, 50),
                EventMarker = CreateEventMarker(graphicsDevice)
            };

            _playheadPanel = new PlayheadPanel
            {
                ZIndex = 4, // 最上层
                PlayheadMarker = CreatePlayheadMarker(graphicsDevice),
                BackgroundColor = Color.Transparent
            };

            // 添加子控件
            AddChild(_gridPanel);
            AddChild(_timeScalePanel);
            AddChild(_eventTrackPanel);
            AddChild(_playheadPanel);

            // 订阅事件
            _eventTrackPanel.OnEventSelected += (sender, evt) => OnEventSelected?.Invoke(this, evt);
            _playheadPanel.OnTimeChanged += (sender, time) => OnTimeChanged?.Invoke(this, time);
            _timeScalePanel.OnZoomScrollChanged += (sender, args) => {
                // 同步缩放和滚动到其他面板
                _eventTrackPanel.Zoom = args.Zoom;
                _eventTrackPanel.ScrollPosition = args.ScrollPosition;
                _playheadPanel.Zoom = args.Zoom;
                _playheadPanel.ScrollPosition = args.ScrollPosition;
            };
        }

        // 设置时间轴边界
        public void SetBounds(Rectangle bounds)
        {
            Bounds = bounds;

            // 设置子控件边界
            _gridPanel.Bounds = bounds;

            // 时间刻度面板在顶部
            _timeScalePanel.Bounds = new Rectangle(bounds.X, bounds.Y, bounds.Width, 40);

            // 事件轨道面板在中间
            _eventTrackPanel.Bounds = new Rectangle(bounds.X, bounds.Y + 40, bounds.Width, 30);

            // 播放头面板覆盖整个时间轴
            _playheadPanel.Bounds = bounds;
        }

        // 设置动画时长
        public void SetDuration(float duration)
        {
            Duration = duration;
            _timeScalePanel.Duration = duration;
            _eventTrackPanel.Duration = duration;
            _playheadPanel.Duration = duration;
        }

        // 添加事件
        public void AddEvent(FrameEvent evt)
        {
            _eventTrackPanel.Events.Add(evt);
            _eventTrackPanel.Events.Sort((a, b) => a.Time.CompareTo(b.Time));
        }

        // 移除事件
        public void RemoveEvent(FrameEvent evt)
        {
            _eventTrackPanel.Events.Remove(evt);
        }

        // 获取所有事件
        public List<FrameEvent> GetEvents()
        {
            return _eventTrackPanel.Events;
        }

        // 创建事件标记纹理
        private Texture2D CreateEventMarker(GraphicsDevice graphicsDevice)
        {
            Texture2D texture = new Texture2D(graphicsDevice, 16, 20);
            Color[] data = new Color[16 * 20];

            // 创建菱形标记
            for (int y = 0; y < 20; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    int centerX = 8;
                    int centerY = 10;
                    int distance = Math.Abs(x - centerX) + Math.Abs(y - centerY);

                    if (distance <= 7)
                    {
                        if (distance == 7)
                            data[y * 16 + x] = new Color(255, 180, 0);
                        else if (distance == 6)
                            data[y * 16 + x] = new Color(255, 200, 0);
                        else
                            data[y * 16 + x] = new Color(255, 220, 0, 200);
                    }
                    else
                    {
                        data[y * 16 + x] = Color.Transparent;
                    }
                }
            }

            texture.SetData(data);
            return texture;
        }

        // 创建播放头标记纹理
        private Texture2D CreatePlayheadMarker(GraphicsDevice graphicsDevice)
        {
            Texture2D texture = new Texture2D(graphicsDevice, 15, 20);
            Color[] data = new Color[15 * 20];

            // 创建三角形播放头
            for (int y = 0; y < 20; y++)
            {
                for (int x = 0; x < 15; x++)
                {
                    if (y == 0 && x >= 5 && x <= 9)
                    {
                        data[y * 15 + x] = new Color(0, 200, 255);
                    }
                    else if (x == 7 && y <= 15)
                    {
                        data[y * 15 + x] = new Color(0, 200, 255);
                    }
                    else if (y >= 15 && y < 20)
                    {
                        int distanceFromCenter = Math.Abs(x - 7);
                        if (distanceFromCenter <= (y - 15))
                        {
                            if (distanceFromCenter == (y - 15) || y == 19)
                                data[y * 15 + x] = new Color(0, 200, 255);
                            else
                                data[y * 15 + x] = new Color(0, 180, 255, 200);
                        }
                        else
                        {
                            data[y * 15 + x] = Color.Transparent;
                        }
                    }
                    else
                    {
                        data[y * 15 + x] = Color.Transparent;
                    }
                }
            }

            texture.SetData(data);
            return texture;
        }

        /// <summary>
        /// 处理鼠标输入，确保事件被正确吞噬
        /// </summary>
        protected override bool OnMouseInput(MouseState mouseState, MouseState prevMouseState)
        {
            // 检查是否是滚轮事件
            if (mouseState.ScrollWheelValue != prevMouseState.ScrollWheelValue)
            {
                // 处理滚轮事件
                float zoomDelta = (mouseState.ScrollWheelValue - prevMouseState.ScrollWheelValue) / 120.0f * 0.1f;

                // 获取当前鼠标位置对应的时间
                float mouseTimePosition = _timeScalePanel.TimeFromX(mouseState.X);

                // 更新缩放值
                _timeScalePanel.Zoom = MathHelper.Clamp(_timeScalePanel.Zoom + zoomDelta, 0.1f, 10.0f);

                // 调整滚动位置，使鼠标下的时间点保持不变
                float newMouseX = _timeScalePanel.XFromTime(mouseTimePosition);
                _timeScalePanel.ScrollPosition += mouseState.X - newMouseX;

                // 同步缩放和滚动到其他面板
                _eventTrackPanel.Zoom = _timeScalePanel.Zoom;
                _eventTrackPanel.ScrollPosition = _timeScalePanel.ScrollPosition;
                _playheadPanel.Zoom = _timeScalePanel.Zoom;
                _playheadPanel.ScrollPosition = _timeScalePanel.ScrollPosition;

                // 直接返回true，吞噬所有滚轮事件，防止传递到下层
                return true;
            }

            // 检查是否是鼠标左键点击事件
            if (mouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Released)
            {
                // 在这里处理点击事件
                // 例如，可以根据点击位置设置当前时间
                if (Bounds.Contains(mouseState.Position))
                {
                    // 计算点击位置对应的时间
                    float clickTime = _timeScalePanel.TimeFromX(mouseState.X);

                    // 设置当前时间
                    _playheadPanel.CurrentTime = clickTime;

                    // 触发时间变化事件
                    OnTimeChanged?.Invoke(this, clickTime);

                    // 吞噬事件，不让下层元素处理
                    return true;
                }
            }

            // 检查是否是鼠标拖动事件
            if (mouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Pressed)
            {
                // 在这里处理拖动事件
                if (Bounds.Contains(mouseState.Position))
                {
                    // 计算拖动位置对应的时间
                    float dragTime = _timeScalePanel.TimeFromX(mouseState.X);

                    // 设置当前时间
                    _playheadPanel.CurrentTime = dragTime;

                    // 触发时间变化事件
                    OnTimeChanged?.Invoke(this, dragTime);

                    // 吞噬事件，不让下层元素处理
                    return true;
                }
            }

            // 对于其他事件，如果鼠标在时间轴范围内，也吞噬事件
            if (Bounds.Contains(mouseState.Position))
            {
                return true;
            }

            // 对于其他情况，不吞噬事件
            return false;
        }
    }
}
