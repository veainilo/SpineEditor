using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpineEditor.UI.UISystem
{
    /// <summary>
    /// UI管理器，负责管理UI元素树和事件分发
    /// </summary>
    public class UIManager
    {
        // 根元素
        private UIElement _root;

        // 鼠标状态
        private MouseState _prevMouseState;

        // 图形设备
        public GraphicsDevice GraphicsDevice { get; private set; }

        public UIManager()
        {
            // 创建根元素
            _root = new RootElement();
            _prevMouseState = Mouse.GetState();
        }

        public UIManager(GraphicsDevice graphicsDevice) : this()
        {
            GraphicsDevice = graphicsDevice;
        }

        // 添加UI元素到根元素
        public void AddElement(UIElement element)
        {
            _root.AddChild(element);
        }

        // 移除UI元素
        public void RemoveElement(UIElement element)
        {
            _root.RemoveChild(element);
        }

        // 更新所有UI元素
        public void Update(GameTime gameTime)
        {
            // 处理输入
            MouseState mouseState = Mouse.GetState();

            // 处理鼠标输入
            bool handled = _root.HandleMouseInput(mouseState, _prevMouseState);

            // 保存当前鼠标状态
            _prevMouseState = mouseState;

            // 更新UI元素
            _root.Update(gameTime);
        }

        // 绘制所有UI元素
        public void Draw(SpriteBatch spriteBatch)
        {
            _root.Draw(spriteBatch);
        }

        // 根元素类（不可见，仅用于组织结构）
        private class RootElement : UIElement
        {
            public RootElement()
            {
                // 设置根元素覆盖整个屏幕
                Bounds = new Rectangle(0, 0, int.MaxValue, int.MaxValue);
            }
        }
    }
}
