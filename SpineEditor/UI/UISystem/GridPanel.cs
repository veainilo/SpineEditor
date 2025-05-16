using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpineEditor.Utils;

namespace SpineEditor.UI.UISystem
{
    /// <summary>
    /// 网格控件，用于显示背景网格
    /// </summary>
    public class GridPanel : Panel
    {
        public int CellWidth { get; set; } = 50;
        public int CellHeight { get; set; } = 30;
        public Color GridColor { get; set; } = new Color(50, 50, 60, 30);
        
        protected override void OnDraw(SpriteBatch spriteBatch)
        {
            // 先绘制背景
            base.OnDraw(spriteBatch);
            
            // 绘制网格线
            for (int x = Bounds.X; x <= Bounds.X + Bounds.Width; x += CellWidth)
            {
                DrawingUtils.DrawVerticalLine(spriteBatch, x, Bounds.Y, Bounds.Height, GridColor);
            }
            
            for (int y = Bounds.Y; y <= Bounds.Y + Bounds.Height; y += CellHeight)
            {
                DrawingUtils.DrawHorizontalLine(spriteBatch, Bounds.X, y, Bounds.Width, GridColor);
            }
        }
        
        // 网格面板不处理任何事件，让事件传递给上层元素
        protected override bool OnMouseInput(MouseState mouseState, MouseState prevMouseState)
        {
            return false; // 不吞噬事件
        }
    }
}
