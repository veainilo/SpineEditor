using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpineEditor.UI.UISystem
{
    /// <summary>
    /// UI元素基类，所有UI控件的基础
    /// </summary>
    public abstract class UIElement
    {
        // 基本属性
        public Rectangle Bounds { get; set; }
        public bool IsVisible { get; set; } = true;
        public bool IsEnabled { get; set; } = true;
        public int ZIndex { get; set; } = 0;

        // 树状结构
        public UIElement Parent { get; private set; }
        public List<UIElement> Children { get; } = new List<UIElement>();

        // 事件处理
        public bool HandleMouseInput(MouseState mouseState, MouseState prevMouseState)
        {
            if (!IsVisible || !IsEnabled)
                return false;

            // 检查鼠标是否在元素范围内
            bool isMouseOver = Bounds.Contains(mouseState.Position);

            // 首先让子元素处理事件（从后向前，即从最上层开始）
            for (int i = Children.Count - 1; i >= 0; i--)
            {
                if (Children[i].HandleMouseInput(mouseState, prevMouseState))
                    return true; // 子元素已处理事件，事件被吞噬
            }

            // 如果子元素没有处理事件，则自己处理
            if (isMouseOver)
            {
                // 处理鼠标事件
                bool handled = OnMouseInput(mouseState, prevMouseState);
                return handled; // 如果返回true，表示事件被吞噬
            }

            return false;
        }

        // 子类重写此方法以处理鼠标事件
        protected virtual bool OnMouseInput(MouseState mouseState, MouseState prevMouseState)
        {
            return false; // 默认不处理
        }

        // 添加子元素
        public virtual void AddChild(UIElement child)
        {
            if (child.Parent != null)
                child.Parent.Children.Remove(child);

            child.Parent = this;
            Children.Add(child);

            // 按Z-Index排序
            Children.Sort((a, b) => a.ZIndex.CompareTo(b.ZIndex));
        }

        // 移除子元素
        public virtual void RemoveChild(UIElement child)
        {
            if (Children.Contains(child))
            {
                child.Parent = null;
                Children.Remove(child);
            }
        }

        // 绘制
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (!IsVisible)
                return;

            // 绘制自身
            OnDraw(spriteBatch);

            // 绘制子元素（按Z-Index顺序）
            foreach (var child in Children)
            {
                child.Draw(spriteBatch);
            }
        }

        // 子类重写此方法以实现自身的绘制
        protected virtual void OnDraw(SpriteBatch spriteBatch) { }

        // 更新
        public virtual void Update(GameTime gameTime)
        {
            if (!IsEnabled)
                return;

            // 更新自身
            OnUpdate(gameTime);

            // 更新子元素
            foreach (var child in Children)
            {
                child.Update(gameTime);
            }
        }

        // 子类重写此方法以实现自身的更新逻辑
        protected virtual void OnUpdate(GameTime gameTime) { }
    }
}
