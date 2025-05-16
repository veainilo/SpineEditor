using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SpineEditor.Utils
{
    /// <summary>
    /// 绘制工具类，提供常用的绘制方法
    /// </summary>
    public static class DrawingUtils
    {
        // 单像素纹理，用于绘制简单形状
        private static Texture2D _pixel;

        /// <summary>
        /// 初始化绘制工具类
        /// </summary>
        /// <param name="graphicsDevice">图形设备</param>
        public static void Initialize(GraphicsDevice graphicsDevice)
        {
            if (_pixel == null)
            {
                _pixel = new Texture2D(graphicsDevice, 1, 1);
                _pixel.SetData(new[] { Color.White });
            }
        }

        /// <summary>
        /// 绘制矩形
        /// </summary>
        /// <param name="spriteBatch">精灵批处理</param>
        /// <param name="rectangle">矩形区域</param>
        /// <param name="color">颜色</param>
        public static void DrawRectangle(SpriteBatch spriteBatch, Rectangle rectangle, Color color)
        {
            EnsurePixelTexture(spriteBatch.GraphicsDevice);
            spriteBatch.Draw(_pixel, rectangle, color);
        }

        /// <summary>
        /// 绘制矩形
        /// </summary>
        /// <param name="spriteBatch">精灵批处理</param>
        /// <param name="x">X 坐标</param>
        /// <param name="y">Y 坐标</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="color">颜色</param>
        public static void DrawRectangle(SpriteBatch spriteBatch, int x, int y, int width, int height, Color color)
        {
            DrawRectangle(spriteBatch, new Rectangle(x, y, width, height), color);
        }

        /// <summary>
        /// 绘制边框
        /// </summary>
        /// <param name="spriteBatch">精灵批处理</param>
        /// <param name="rectangle">矩形区域</param>
        /// <param name="color">颜色</param>
        /// <param name="thickness">边框厚度</param>
        public static void DrawBorder(SpriteBatch spriteBatch, Rectangle rectangle, Color color, int thickness = 1)
        {
            EnsurePixelTexture(spriteBatch.GraphicsDevice);

            // 上边框
            DrawRectangle(spriteBatch, rectangle.X, rectangle.Y, rectangle.Width, thickness, color);
            // 下边框
            DrawRectangle(spriteBatch, rectangle.X, rectangle.Y + rectangle.Height - thickness, rectangle.Width, thickness, color);
            // 左边框
            DrawRectangle(spriteBatch, rectangle.X, rectangle.Y, thickness, rectangle.Height, color);
            // 右边框
            DrawRectangle(spriteBatch, rectangle.X + rectangle.Width - thickness, rectangle.Y, thickness, rectangle.Height, color);
        }

        /// <summary>
        /// 绘制水平线
        /// </summary>
        /// <param name="spriteBatch">精灵批处理</param>
        /// <param name="x">起始 X 坐标</param>
        /// <param name="y">Y 坐标</param>
        /// <param name="length">线长</param>
        /// <param name="color">颜色</param>
        /// <param name="thickness">线宽</param>
        public static void DrawHorizontalLine(SpriteBatch spriteBatch, int x, int y, int length, Color color, int thickness = 1)
        {
            DrawRectangle(spriteBatch, x, y, length, thickness, color);
        }

        /// <summary>
        /// 绘制垂直线
        /// </summary>
        /// <param name="spriteBatch">精灵批处理</param>
        /// <param name="x">X 坐标</param>
        /// <param name="y">起始 Y 坐标</param>
        /// <param name="length">线长</param>
        /// <param name="color">颜色</param>
        /// <param name="thickness">线宽</param>
        public static void DrawVerticalLine(SpriteBatch spriteBatch, int x, int y, int length, Color color, int thickness = 1)
        {
            DrawRectangle(spriteBatch, x, y, thickness, length, color);
        }

        /// <summary>
        /// 绘制纹理
        /// </summary>
        /// <param name="spriteBatch">精灵批处理</param>
        /// <param name="texture">纹理</param>
        /// <param name="position">位置</param>
        /// <param name="color">颜色</param>
        public static void DrawTexture(SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Color color)
        {
            spriteBatch.Draw(texture, position, color);
        }

        /// <summary>
        /// 绘制纹理
        /// </summary>
        /// <param name="spriteBatch">精灵批处理</param>
        /// <param name="texture">纹理</param>
        /// <param name="rectangle">矩形区域</param>
        /// <param name="color">颜色</param>
        public static void DrawTexture(SpriteBatch spriteBatch, Texture2D texture, Rectangle rectangle, Color color)
        {
            spriteBatch.Draw(texture, rectangle, color);
        }

        /// <summary>
        /// 绘制标记（小方块）
        /// </summary>
        /// <param name="spriteBatch">精灵批处理</param>
        /// <param name="position">位置</param>
        /// <param name="size">大小</param>
        /// <param name="color">颜色</param>
        public static void DrawMarker(SpriteBatch spriteBatch, Vector2 position, int size, Color color)
        {
            EnsurePixelTexture(spriteBatch.GraphicsDevice);
            DrawRectangle(spriteBatch, (int)position.X - size / 2, (int)position.Y - size / 2, size, size, color);
        }

        /// <summary>
        /// 绘制箭头
        /// </summary>
        /// <param name="spriteBatch">精灵批处理</param>
        /// <param name="position">位置</param>
        /// <param name="size">大小</param>
        /// <param name="isUp">是否向上</param>
        /// <param name="color">颜色</param>
        public static void DrawArrow(SpriteBatch spriteBatch, Vector2 position, int size, bool isUp, Color color)
        {
            EnsurePixelTexture(spriteBatch.GraphicsDevice);

            if (isUp)
            {
                // 绘制向上箭头
                for (int i = 0; i < size; i++)
                {
                    DrawRectangle(spriteBatch, (int)position.X - size + i, (int)position.Y + i, 2, 1, color);
                    DrawRectangle(spriteBatch, (int)position.X + size - i, (int)position.Y + i, 2, 1, color);
                }
            }
            else
            {
                // 绘制向下箭头
                for (int i = 0; i < size; i++)
                {
                    DrawRectangle(spriteBatch, (int)position.X - size + i, (int)position.Y - i, 2, 1, color);
                    DrawRectangle(spriteBatch, (int)position.X + size - i, (int)position.Y - i, 2, 1, color);
                }
            }
        }

        /// <summary>
        /// 绘制网格
        /// </summary>
        /// <param name="spriteBatch">精灵批处理</param>
        /// <param name="bounds">边界</param>
        /// <param name="cellSize">单元格大小</param>
        /// <param name="color">颜色</param>
        public static void DrawGrid(SpriteBatch spriteBatch, Rectangle bounds, int cellSize, Color color)
        {
            EnsurePixelTexture(spriteBatch.GraphicsDevice);

            // 绘制垂直线
            for (int x = bounds.X; x <= bounds.X + bounds.Width; x += cellSize)
            {
                DrawVerticalLine(spriteBatch, x, bounds.Y, bounds.Height, color);
            }

            // 绘制水平线
            for (int y = bounds.Y; y <= bounds.Y + bounds.Height; y += cellSize)
            {
                DrawHorizontalLine(spriteBatch, bounds.X, y, bounds.Width, color);
            }
        }

        /// <summary>
        /// 确保像素纹理已初始化
        /// </summary>
        /// <param name="graphicsDevice">图形设备</param>
        private static void EnsurePixelTexture(GraphicsDevice graphicsDevice)
        {
            if (_pixel == null)
            {
                Initialize(graphicsDevice);
            }
        }
    }
}
