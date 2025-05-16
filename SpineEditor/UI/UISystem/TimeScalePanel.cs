using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpineEditor.Utils;

namespace SpineEditor.UI.UISystem
{
    /// <summary>
    /// 时间刻度控件，显示时间刻度和主要网格线
    /// </summary>
    public class TimeScalePanel : Panel
    {
        public float Duration { get; set; } = 1.0f;
        public float Zoom { get; set; } = 1.0f;
        public float ScrollPosition { get; set; } = 0.0f;
        public SpriteFont Font { get; set; }
        
        // 缩放和滚动变化事件
        public event EventHandler<ZoomScrollEventArgs> OnZoomScrollChanged;
        
        protected override void OnDraw(SpriteBatch spriteBatch)
        {
            // 绘制背景 - 使用透明背景，不覆盖下层网格
            BackgroundColor = Color.Transparent;
            base.OnDraw(spriteBatch);
            
            // 绘制时间刻度和网格线
            float timeStep = GetTimeStep();
            for (float t = 0; t <= Duration; t += timeStep)
            {
                float x = XFromTime(t);
                if (x >= Bounds.X && x <= Bounds.X + Bounds.Width)
                {
                    bool isMainTick = Math.Abs(t - Math.Round(t, 0)) < 0.001f; // 整数时间点
                    
                    if (isMainTick)
                    {
                        // 主刻度线
                        DrawingUtils.DrawVerticalLine(spriteBatch, (int)x, Bounds.Y + 20, 20, new Color(150, 150, 170));
                        
                        // 垂直网格线
                        DrawingUtils.DrawVerticalLine(spriteBatch, (int)x, Bounds.Y + 40, Bounds.Height - 40, new Color(80, 80, 100, 80));
                        
                        // 时间文本
                        string timeText = t.ToString("0.00");
                        Vector2 textSize = Font.MeasureString(timeText);
                        spriteBatch.DrawString(Font, timeText, new Vector2(x - textSize.X / 2, Bounds.Y + 2), new Color(200, 200, 220));
                    }
                    else
                    {
                        // 次刻度线
                        DrawingUtils.DrawVerticalLine(spriteBatch, (int)x, Bounds.Y + 20, 10, new Color(100, 100, 120));
                    }
                }
            }
        }
        
        // 将时间转换为X坐标
        public float XFromTime(float time)
        {
            return Bounds.X + (time / Duration) * Bounds.Width * Zoom - ScrollPosition;
        }
        
        // 将X坐标转换为时间
        public float TimeFromX(float x)
        {
            return ((x + ScrollPosition - Bounds.X) / (Bounds.Width * Zoom)) * Duration;
        }
        
        // 获取时间刻度步长
        private float GetTimeStep()
        {
            float baseDuration = Math.Max(1.0f, Duration);
            float targetStepCount = 20.0f * Zoom;
            float rawStep = baseDuration / targetStepCount;
            
            float magnitude = (float)Math.Pow(10, Math.Floor(Math.Log10(rawStep)));
            float normalizedStep = rawStep / magnitude;
            
            if (normalizedStep < 0.2f) return 0.1f * magnitude;
            if (normalizedStep < 0.5f) return 0.2f * magnitude;
            if (normalizedStep < 1.0f) return 0.5f * magnitude;
            if (normalizedStep < 2.0f) return 1.0f * magnitude;
            if (normalizedStep < 5.0f) return 2.0f * magnitude;
            return 5.0f * magnitude;
        }
        
        // 处理鼠标滚轮缩放
        protected override bool OnMouseInput(MouseState mouseState, MouseState prevMouseState)
        {
            if (mouseState.ScrollWheelValue != prevMouseState.ScrollWheelValue)
            {
                float zoomDelta = (mouseState.ScrollWheelValue - prevMouseState.ScrollWheelValue) / 120.0f * 0.1f;
                float mouseTimePosition = TimeFromX(mouseState.X);
                
                float oldZoom = Zoom;
                Zoom = MathHelper.Clamp(Zoom + zoomDelta, 0.1f, 10.0f);
                
                float newMouseX = XFromTime(mouseTimePosition);
                ScrollPosition += mouseState.X - newMouseX;
                
                // 触发缩放和滚动变化事件
                OnZoomScrollChanged?.Invoke(this, new ZoomScrollEventArgs(Zoom, ScrollPosition));
                
                return true; // 吞噬事件，不让下层元素处理
            }
            
            return false;
        }
    }
    
    // 缩放和滚动事件参数
    public class ZoomScrollEventArgs : EventArgs
    {
        public float Zoom { get; }
        public float ScrollPosition { get; }
        
        public ZoomScrollEventArgs(float zoom, float scrollPosition)
        {
            Zoom = zoom;
            ScrollPosition = scrollPosition;
        }
    }
}
