using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpineEditor.Events;
using SpineEditor.UI.UISystem;
using System;

namespace SpineEditor.UI.GUILayoutComponents
{
    /// <summary>
    /// 基于GUILayout系统的Spine视口
    /// </summary>
    public class SpineViewportGUI : GUILayoutPanel
    {
        // Spine事件编辑器
        private SpineEventEditor _eventEditor;

        // 视口状态
        private bool _isDragging = false;
        private Vector2 _dragStart;
        private Vector2 _dragOrigin;
        private float _zoomLevel = 1.0f;
        private bool _enableScrollWheel = true;

        // 鼠标状态
        private MouseState _prevMouseState;

        /// <summary>
        /// 创建Spine视口
        /// </summary>
        /// <param name="eventEditor">Spine事件编辑器</param>
        /// <param name="title">面板标题</param>
        /// <param name="bounds">面板边界</param>
        /// <param name="graphicsDevice">图形设备</param>
        /// <param name="font">字体</param>
        public SpineViewportGUI(SpineEventEditor eventEditor, string title, Rectangle bounds, GraphicsDevice graphicsDevice, SpriteFont font)
            : base(title, bounds, graphicsDevice, font)
        {
            _eventEditor = eventEditor;
            _showTitle = false; // 不显示标题栏
        }

        /// <summary>
        /// 更新视口
        /// </summary>
        /// <param name="gameTime">游戏时间</param>
        /// <param name="enableScrollWheel">是否启用滚轮缩放</param>
        public void Update(GameTime gameTime, bool enableScrollWheel)
        {
            _enableScrollWheel = enableScrollWheel;
            base.Update(gameTime);
        }

        /// <summary>
        /// 处理鼠标输入
        /// </summary>
        /// <param name="mousePosition">鼠标位置</param>
        /// <param name="leftButtonPressed">左键是否按下</param>
        /// <param name="leftButtonJustPressed">左键是否刚刚按下</param>
        /// <param name="leftButtonJustReleased">左键是否刚刚释放</param>
        /// <returns>是否处理了输入</returns>
        public bool HandleMouseInput(Point mousePosition, bool leftButtonPressed, bool leftButtonJustPressed, bool leftButtonJustReleased)
        {
            // 检查是否在视口内
            if (!_bounds.Contains(mousePosition))
                return false;

            MouseState mouseState = Mouse.GetState();

            // 处理鼠标滚轮缩放
            if (_enableScrollWheel && mouseState.ScrollWheelValue != _prevMouseState.ScrollWheelValue)
            {
                float zoomDelta = (mouseState.ScrollWheelValue - _prevMouseState.ScrollWheelValue) / 1200.0f;
                _zoomLevel = MathHelper.Clamp(_zoomLevel + zoomDelta, 0.1f, 10.0f);
                _eventEditor.Scale = _zoomLevel;
                _prevMouseState = mouseState;
                return true;
            }

            // 处理鼠标拖动
            if (leftButtonJustPressed)
            {
                _isDragging = true;
                _dragStart = new Vector2(mousePosition.X, mousePosition.Y);
                _dragOrigin = _eventEditor.Position;
                return true;
            }

            if (leftButtonPressed && _isDragging)
            {
                Vector2 delta = new Vector2(mousePosition.X, mousePosition.Y) - _dragStart;
                _eventEditor.Position = _dragOrigin + delta;
                return true;
            }

            if (leftButtonJustReleased && _isDragging)
            {
                _isDragging = false;
                return true;
            }

            _prevMouseState = mouseState;
            return false;
        }

        /// <summary>
        /// 绘制GUI内容
        /// </summary>
        protected override void DrawGUI()
        {
            // 视口不需要绘制GUI内容，因为Spine动画是直接在SpriteBatch上绘制的
            // 这里只需要提供一个空的实现
        }

        /// <summary>
        /// 绘制面板框架
        /// </summary>
        /// <param name="spriteBatch">精灵批处理</param>
        protected override void DrawPanelFrame(SpriteBatch spriteBatch)
        {
            // 绘制视口边框
            DrawBorder(spriteBatch, _bounds, Color.Gray, 1);

            // 绘制Spine动画
            if (_eventEditor != null)
            {
                // 保存当前视口状态
                Rectangle originalViewport = spriteBatch.GraphicsDevice.Viewport.Bounds;

                // 保存当前位置
                Vector2 originalPosition = _eventEditor.Position;

                try
                {
                    // 设置新的视口，限制绘制区域
                    spriteBatch.End();

                    // 不需要修改Spine动画的位置，因为我们在SpineEventEditorGameGUI中已经设置了位置
                    // 我们只需要设置视口，让Spine动画显示在视口中心

                    // 设置视口
                    spriteBatch.GraphicsDevice.Viewport = new Viewport(_bounds);

                    // 绘制Spine动画
                    _eventEditor.Draw();
                }
                finally
                {
                    // 恢复原始位置
                    _eventEditor.Position = originalPosition;

                    // 恢复原始视口
                    spriteBatch.GraphicsDevice.Viewport = new Viewport(originalViewport);
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                }

                // 绘制中心点标记 - 使用视口中心
                Vector2 centerPos = new Vector2(
                    _bounds.X + _bounds.Width / 2,
                    _bounds.Y + _bounds.Height / 2
                );

                // 绘制十字标记
                spriteBatch.Draw(TextureManager.Pixel, new Rectangle((int)centerPos.X - 10, (int)centerPos.Y, 20, 1), Color.Yellow);
                spriteBatch.Draw(TextureManager.Pixel, new Rectangle((int)centerPos.X, (int)centerPos.Y - 10, 1, 20), Color.Yellow);
            }
        }
    }
}
