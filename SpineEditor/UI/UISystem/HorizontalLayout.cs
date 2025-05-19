using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace SpineEditor.UI.UISystem
{
    /// <summary>
    /// 水平布局面板，自动水平排列子元素
    /// </summary>
    public class HorizontalLayout : LayoutPanel
    {
        // 垂直对齐方式
        public enum VerticalAlignment
        {
            Top,
            Center,
            Bottom,
            Stretch
        }
        
        public VerticalAlignment ChildAlignment { get; set; } = VerticalAlignment.Center;
        
        protected override void ArrangeChildren()
        {
            int x = Bounds.X + PaddingLeft;
            
            foreach (var child in Children)
            {
                // 计算子元素的Y坐标，根据对齐方式
                int y;
                int height = child.Bounds.Height;
                
                switch (ChildAlignment)
                {
                    case VerticalAlignment.Top:
                        y = Bounds.Y + PaddingTop;
                        break;
                    case VerticalAlignment.Center:
                        y = Bounds.Y + (Bounds.Height - height) / 2;
                        break;
                    case VerticalAlignment.Bottom:
                        y = Bounds.Y + Bounds.Height - height - PaddingBottom;
                        break;
                    case VerticalAlignment.Stretch:
                        y = Bounds.Y + PaddingTop;
                        height = Bounds.Height - PaddingTop - PaddingBottom;
                        break;
                    default:
                        y = Bounds.Y + PaddingTop;
                        break;
                }
                
                // 设置子元素的位置
                child.Bounds = new Rectangle(x, y, child.Bounds.Width, height);
                
                // 更新下一个元素的X坐标
                x += child.Bounds.Width + Spacing;
            }
            
            // 如果是自动大小，调整面板宽度
            if (AutoSize && Children.Count > 0)
            {
                var lastChild = Children[Children.Count - 1];
                int width = lastChild.Bounds.X + lastChild.Bounds.Width - Bounds.X + PaddingRight;
                Bounds = new Rectangle(Bounds.X, Bounds.Y, width, Bounds.Height);
            }
        }
    }
}
