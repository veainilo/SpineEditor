using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpineEditor.Events;

namespace SpineEditor.UI.UISystem
{
    /// <summary>
    /// 事件轨道控件，显示和管理事件标记
    /// </summary>
    public class EventTrackPanel : Panel
    {
        public List<FrameEvent> Events { get; } = new List<FrameEvent>();
        public FrameEvent SelectedEvent { get; private set; }
        public float Duration { get; set; } = 1.0f;
        public float Zoom { get; set; } = 1.0f;
        public float ScrollPosition { get; set; } = 0.0f;
        public SpriteFont Font { get; set; }
        public Texture2D EventMarker { get; set; }

        // 事件选中委托
        public event EventHandler<FrameEvent> OnEventSelected;

        // 是否启用上下文菜单
        private bool _contextMenuEnabled = true;

        /// <summary>
        /// 禁用上下文菜单
        /// </summary>
        public void DisableContextMenu()
        {
            _contextMenuEnabled = false;
        }

        // 拖动相关
        private bool _isDragging = false;
        private float _dragOffset = 0;

        protected override void OnDraw(SpriteBatch spriteBatch)
        {
            // 绘制背景
            base.OnDraw(spriteBatch);

            // 绘制事件标记
            foreach (var evt in Events)
            {
                float x = XFromTime(evt.Time);
                if (x >= Bounds.X - 10 && x <= Bounds.X + Bounds.Width + 10)
                {
                    // 根据事件类型选择颜色
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
                    if (evt == SelectedEvent)
                    {
                        color = Color.Lerp(color, Color.White, 0.5f);
                    }

                    // 绘制事件标记
                    spriteBatch.Draw(EventMarker, new Vector2(x - 8, Bounds.Y + 5), color);

                    // 绘制事件名称
                    Vector2 textSize = Font.MeasureString(evt.Name);

                    // 绘制文本背景
                    if (evt == SelectedEvent)
                    {
                        Rectangle textBgRect = new Rectangle(
                            (int)(x - textSize.X / 2 - 2),
                            Bounds.Y + 30,
                            (int)textSize.X + 4,
                            (int)textSize.Y + 2
                        );
                        spriteBatch.Draw(TextureManager.Pixel, textBgRect, new Color(40, 40, 50, 200));
                    }

                    // 绘制事件名称
                    spriteBatch.DrawString(
                        Font,
                        evt.Name,
                        new Vector2(x - textSize.X / 2, Bounds.Y + 31),
                        evt == SelectedEvent ? Color.White : new Color(200, 200, 200)
                    );
                }
            }
        }

        // 将时间转换为X坐标
        private float XFromTime(float time)
        {
            return Bounds.X + (time / Duration) * Bounds.Width * Zoom - ScrollPosition;
        }

        // 将X坐标转换为时间
        private float TimeFromX(float x)
        {
            return ((x + ScrollPosition - Bounds.X) / (Bounds.Width * Zoom)) * Duration;
        }

        // 处理鼠标事件
        protected override bool OnMouseInput(MouseState mouseState, MouseState prevMouseState)
        {
            // 处理点击事件
            if (prevMouseState.LeftButton == ButtonState.Released && mouseState.LeftButton == ButtonState.Pressed)
            {
                // 检查是否点击了事件标记
                foreach (var evt in Events)
                {
                    float eventX = XFromTime(evt.Time);
                    if (Math.Abs(mouseState.X - eventX) < 10)
                    {
                        SelectedEvent = evt;
                        _isDragging = true;
                        _dragOffset = mouseState.X - eventX;
                        OnEventSelected?.Invoke(this, SelectedEvent);
                        return true; // 吞噬事件
                    }
                }
            }

            // 处理右键菜单（如果启用）
            if (_contextMenuEnabled && prevMouseState.RightButton == ButtonState.Released && mouseState.RightButton == ButtonState.Pressed)
            {
                // 在这里可以添加右键菜单的处理逻辑
                // 由于我们已经禁用了右键菜单，这部分代码不会执行
                return true; // 吞噬事件
            }

            // 处理拖动事件
            if (_isDragging && mouseState.LeftButton == ButtonState.Pressed && SelectedEvent != null)
            {
                float newTime = TimeFromX(mouseState.X - _dragOffset);
                SelectedEvent.Time = MathHelper.Clamp(newTime, 0, Duration);

                // 重新排序事件
                Events.Sort((a, b) => a.Time.CompareTo(b.Time));

                return true; // 吞噬事件
            }
            else if (mouseState.LeftButton == ButtonState.Released)
            {
                _isDragging = false;
            }

            return false;
        }
    }
}
