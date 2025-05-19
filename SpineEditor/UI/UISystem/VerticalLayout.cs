using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace SpineEditor.UI.UISystem
{
    /// <summary>
    /// 垂直布局面板，自动垂直排列子元素
    /// </summary>
    public class VerticalLayout : LayoutPanel
    {
        // 水平对齐方式
        public enum HorizontalAlignment
        {
            Left,
            Center,
            Right,
            Stretch
        }
        
        public HorizontalAlignment ChildAlignment { get; set; } = HorizontalAlignment.Center;
        
        protected override void ArrangeChildren()
        {
            int y = Bounds.Y + PaddingTop;
            
            foreach (var child in Children)
            {
                // 计算子元素的X坐标，根据对齐方式
                int x;
                int width = child.Bounds.Width;
                
                switch (ChildAlignment)
                {
                    case HorizontalAlignment.Left:
                        x = Bounds.X + PaddingLeft;
                        break;
                    case HorizontalAlignment.Center:
                        x = Bounds.X + (Bounds.Width - width) / 2;
                        break;
                    case HorizontalAlignment.Right:
                        x = Bounds.X + Bounds.Width - width - PaddingRight;
                        break;
                    case HorizontalAlignment.Stretch:
                        x = Bounds.X + PaddingLeft;
                        width = Bounds.Width - PaddingLeft - PaddingRight;
                        break;
                    default:
                        x = Bounds.X + PaddingLeft;
                        break;
                }
                
                // 设置子元素的位置
                child.Bounds = new Rectangle(x, y, width, child.Bounds.Height);
                
                // 更新下一个元素的Y坐标
                y += child.Bounds.Height + Spacing;
            }
            
            // 如果是自动大小，调整面板高度
            if (AutoSize && Children.Count > 0)
            {
                var lastChild = Children[Children.Count - 1];
                int height = lastChild.Bounds.Y + lastChild.Bounds.Height - Bounds.Y + PaddingBottom;
                Bounds = new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, height);
            }
        }
    }
}
