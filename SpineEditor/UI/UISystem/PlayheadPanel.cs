using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpineEditor.Utils;

namespace SpineEditor.UI.UISystem
{
    /// <summary>
    /// 播放头控件，显示当前播放位置
    /// </summary>
    public class PlayheadPanel : Panel
    {
        public float CurrentTime { get; set; } = 0.0f;
        public float Duration { get; set; } = 1.0f;
        public float Zoom { get; set; } = 1.0f;
        public float ScrollPosition { get; set; } = 0.0f;
        public Texture2D PlayheadMarker { get; set; }
        
        // 拖动相关
        private bool _isDragging = false;
        
        // 当前时间变化事件
        public event EventHandler<float> OnTimeChanged;
        
        protected override void OnDraw(SpriteBatch spriteBatch)
        {
            // 使用透明背景，不覆盖下层元素
            BackgroundColor = Color.Transparent;
            base.OnDraw(spriteBatch);
            
            // 绘制播放头
            float x = XFromTime(CurrentTime);
            if (x >= Bounds.X - 5 && x <= Bounds.X + Bounds.Width + 5)
            {
                // 绘制播放头标记
                spriteBatch.Draw(PlayheadMarker, new Vector2(x - 7, Bounds.Y), Color.White);
                
                // 绘制播放头线条
                for (int i = 0; i < Bounds.Height; i++)
                {
                    float alpha = 1.0f - (i / (float)Bounds.Height) * 0.7f;
                    DrawingUtils.DrawRectangle(
                        spriteBatch,
                        (int)x - 1,
                        Bounds.Y + i,
                        2,
                        1,
                        new Color(0, 200, 255, (int)(255 * alpha))
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
            // 检查是否点击了播放头
            float playheadX = XFromTime(CurrentTime);
            bool clickedPlayhead = Math.Abs(mouseState.X - playheadX) < 10;
            
            // 如果点击了播放头或者正在拖动
            if ((clickedPlayhead && mouseState.LeftButton == ButtonState.Pressed) || _isDragging)
            {
                _isDragging = mouseState.LeftButton == ButtonState.Pressed;
                
                // 更新当前时间
                float newTime = MathHelper.Clamp(TimeFromX(mouseState.X), 0, Duration);
                if (newTime != CurrentTime)
                {
                    CurrentTime = newTime;
                    OnTimeChanged?.Invoke(this, CurrentTime);
                }
                
                return true; // 吞噬事件
            }
            
            return false;
        }
    }
}
