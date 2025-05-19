using Microsoft.Xna.Framework;

namespace SpineEditor.UI.UISystem
{
    /// <summary>
    /// 布局面板基类，提供自动布局功能的基础
    /// </summary>
    public abstract class LayoutPanel : Panel
    {
        // 内边距
        public int PaddingLeft { get; set; } = 5;
        public int PaddingRight { get; set; } = 5;
        public int PaddingTop { get; set; } = 5;
        public int PaddingBottom { get; set; } = 5;
        
        // 元素间距
        public int Spacing { get; set; } = 5;
        
        // 是否自动调整大小以适应内容
        public bool AutoSize { get; set; } = false;
        
        // 当子元素或属性变化时，重新排列子元素
        protected abstract void ArrangeChildren();
        
        // 重写添加子元素方法
        public override void AddChild(UIElement child)
        {
            base.AddChild(child);
            ArrangeChildren();
        }
        
        // 重写移除子元素方法
        public override void RemoveChild(UIElement child)
        {
            base.RemoveChild(child);
            ArrangeChildren();
        }
        
        // 重写更新方法
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);
            // 可以在这里检测大小变化并重新排列子元素
        }
        
        // 设置边界时重新排列子元素
        public void SetBounds(Rectangle bounds)
        {
            Bounds = bounds;
            ArrangeChildren();
        }
    }
}
