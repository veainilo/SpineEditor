using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpineEditor.Events;
using System;

namespace SpineEditor.UI
{
    /// <summary>
    /// 攻击形状渲染器，用于可视化显示攻击帧事件的形状
    /// </summary>
    public class AttackShapeRenderer
    {
        private GraphicsDevice _graphicsDevice;
        private Texture2D _pixel;
        private BasicEffect _effect;
        private VertexPositionColor[] _vertices;
        private int _vertexCount;
        private const int MAX_VERTICES = 100; // 最大顶点数

        /// <summary>
        /// 创建攻击形状渲染器
        /// </summary>
        /// <param name="graphicsDevice">图形设备</param>
        public AttackShapeRenderer(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;

            // 创建1x1像素纹理
            _pixel = new Texture2D(graphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });

            // 创建基本效果
            _effect = new BasicEffect(graphicsDevice);
            _effect.VertexColorEnabled = true;
            _effect.World = Matrix.Identity;
            _effect.View = Matrix.CreateLookAt(new Vector3(0, 0, 1), Vector3.Zero, Vector3.Up);
            _effect.Projection = Matrix.CreateOrthographicOffCenter(
                0, graphicsDevice.Viewport.Width,
                graphicsDevice.Viewport.Height, 0,
                0, 1);

            // 初始化顶点数组
            _vertices = new VertexPositionColor[MAX_VERTICES];
            _vertexCount = 0;

            // 创建拖拽处理器
            _dragHandler = new ShapeDragHandler();
        }

        /// <summary>
        /// 拖拽处理器
        /// </summary>
        private ShapeDragHandler _dragHandler;

        /// <summary>
        /// 获取拖拽处理器
        /// </summary>
        public ShapeDragHandler DragHandler => _dragHandler;

        /// <summary>
        /// 绘制攻击形状
        /// </summary>
        /// <param name="attackShape">攻击形状数据</param>
        /// <param name="position">Spine动画的位置（原点位置）</param>
        /// <param name="scale">缩放</param>
        /// <param name="color">颜色</param>
        /// <param name="showHandles">是否显示拖拽控制点</param>
        /// <remarks>
        /// 攻击形状的坐标是相对于Spine动画原点的，需要将其转换为屏幕坐标
        /// </remarks>
        public void DrawAttackShape(AttackShape attackShape, Vector2 position, float scale, Color color, bool showHandles = false)
        {
            if (attackShape == null)
                return;

            // 计算实际位置（考虑Spine原点位置、形状相对坐标和缩放）
            Vector2 actualPosition = new Vector2(
                position.X + attackShape.X * scale,
                position.Y + attackShape.Y * scale
            );

            // 根据形状类型绘制
            switch (attackShape.Type)
            {
                case ShapeType.Rectangle:
                    DrawRectangle(
                        actualPosition,
                        attackShape.Width * scale,
                        attackShape.Height * scale,
                        attackShape.Rotation,
                        color
                    );

                    // 如果需要显示拖拽控制点
                    if (showHandles)
                    {
                        DrawResizeHandles(
                            actualPosition,
                            attackShape.Width * scale,
                            attackShape.Height * scale,
                            attackShape.Rotation,
                            Color.Yellow
                        );
                    }
                    break;

                case ShapeType.Circle:
                    DrawCircle(
                        actualPosition,
                        attackShape.Width * scale, // 半径
                        32, // 分段数
                        color
                    );

                    // 如果需要显示拖拽控制点
                    if (showHandles)
                    {
                        DrawCircleResizeHandles(
                            actualPosition,
                            attackShape.Width * scale,
                            Color.Yellow
                        );
                    }
                    break;
            }
        }

        /// <summary>
        /// 绘制矩形
        /// </summary>
        private void DrawRectangle(Vector2 center, float width, float height, float rotation, Color color)
        {
            // 计算矩形的四个角点（未旋转）
            Vector2 halfSize = new Vector2(width / 2, height / 2);
            Vector2[] corners = new Vector2[4]
            {
                new Vector2(-halfSize.X, -halfSize.Y), // 左上
                new Vector2(halfSize.X, -halfSize.Y),  // 右上
                new Vector2(halfSize.X, halfSize.Y),   // 右下
                new Vector2(-halfSize.X, halfSize.Y)   // 左下
            };

            // 应用旋转
            float radians = MathHelper.ToRadians(rotation);
            for (int i = 0; i < 4; i++)
            {
                float x = corners[i].X;
                float y = corners[i].Y;
                corners[i].X = x * (float)Math.Cos(radians) - y * (float)Math.Sin(radians);
                corners[i].Y = x * (float)Math.Sin(radians) + y * (float)Math.Cos(radians);
                corners[i] += center;
            }

            // 准备顶点
            _vertexCount = 0;
            for (int i = 0; i < 4; i++)
            {
                int nextIndex = (i + 1) % 4;
                AddLine(corners[i], corners[nextIndex], color);
            }

            // 绘制线条
            DrawLines();
        }

        /// <summary>
        /// 绘制圆形
        /// </summary>
        private void DrawCircle(Vector2 center, float radius, int segments, Color color)
        {
            _vertexCount = 0;

            // 生成圆周上的点
            Vector2 prev = new Vector2(
                center.X + radius,
                center.Y
            );

            for (int i = 1; i <= segments; i++)
            {
                float angle = i * MathHelper.TwoPi / segments;
                Vector2 current = new Vector2(
                    center.X + radius * (float)Math.Cos(angle),
                    center.Y + radius * (float)Math.Sin(angle)
                );

                AddLine(prev, current, color);
                prev = current;
            }

            // 绘制线条
            DrawLines();
        }

        /// <summary>
        /// 添加一条线段
        /// </summary>
        private void AddLine(Vector2 start, Vector2 end, Color color)
        {
            if (_vertexCount + 2 > MAX_VERTICES)
                return;

            _vertices[_vertexCount++] = new VertexPositionColor(
                new Vector3(start, 0),
                color
            );

            _vertices[_vertexCount++] = new VertexPositionColor(
                new Vector3(end, 0),
                color
            );
        }

        /// <summary>
        /// 绘制所有线条
        /// </summary>
        private void DrawLines()
        {
            if (_vertexCount == 0)
                return;

            // 设置渲染状态
            _graphicsDevice.RasterizerState = new RasterizerState { CullMode = CullMode.None };
            _graphicsDevice.BlendState = BlendState.AlphaBlend;
            _graphicsDevice.DepthStencilState = DepthStencilState.None;

            // 应用效果
            foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _graphicsDevice.DrawUserPrimitives(
                    PrimitiveType.LineList,
                    _vertices,
                    0,
                    _vertexCount / 2
                );
            }
        }

        /// <summary>
        /// 绘制矩形的调整大小控制点
        /// </summary>
        private void DrawResizeHandles(Vector2 center, float width, float height, float rotation, Color color)
        {
            // 计算矩形的四个角点和四个边的中点
            Vector2 halfSize = new Vector2(width / 2, height / 2);
            Vector2[] corners = new Vector2[8];

            // 四个角点
            corners[0] = new Vector2(-halfSize.X, -halfSize.Y); // 左上
            corners[1] = new Vector2(halfSize.X, -halfSize.Y);  // 右上
            corners[2] = new Vector2(halfSize.X, halfSize.Y);   // 右下
            corners[3] = new Vector2(-halfSize.X, halfSize.Y);  // 左下

            // 四个边的中点
            corners[4] = new Vector2(0, -halfSize.Y);           // 上边中点
            corners[5] = new Vector2(halfSize.X, 0);            // 右边中点
            corners[6] = new Vector2(0, halfSize.Y);            // 下边中点
            corners[7] = new Vector2(-halfSize.X, 0);           // 左边中点

            // 应用旋转
            float radians = MathHelper.ToRadians(rotation);
            for (int i = 0; i < 8; i++)
            {
                float x = corners[i].X;
                float y = corners[i].Y;
                corners[i].X = x * (float)Math.Cos(radians) - y * (float)Math.Sin(radians);
                corners[i].Y = x * (float)Math.Sin(radians) + y * (float)Math.Cos(radians);
                corners[i] += center;
            }

            // 绘制控制点
            const float handleSize = 6.0f;
            for (int i = 0; i < 8; i++)
            {
                DrawSquareHandle(corners[i], handleSize, color);
            }

            // 绘制旋转控制点（在形状上方）
            Vector2 rotateHandle = new Vector2(0, -halfSize.Y - 40.0f);

            // 应用旋转
            float rotateX = rotateHandle.X;
            float rotateY = rotateHandle.Y;
            rotateHandle.X = rotateX * (float)Math.Cos(radians) - rotateY * (float)Math.Sin(radians);
            rotateHandle.Y = rotateX * (float)Math.Sin(radians) + rotateY * (float)Math.Cos(radians);
            rotateHandle += center;

            // 绘制旋转控制点
            DrawSquareHandle(rotateHandle, handleSize, Color.Green);
        }

        /// <summary>
        /// 绘制圆形的调整大小控制点
        /// </summary>
        private void DrawCircleResizeHandles(Vector2 center, float radius, Color color)
        {
            const float handleSize = 6.0f;

            // 绘制四个方向的控制点
            DrawSquareHandle(new Vector2(center.X, center.Y - radius), handleSize, color); // 上
            DrawSquareHandle(new Vector2(center.X + radius, center.Y), handleSize, color); // 右
            DrawSquareHandle(new Vector2(center.X, center.Y + radius), handleSize, color); // 下
            DrawSquareHandle(new Vector2(center.X - radius, center.Y), handleSize, color); // 左

            // 绘制中心点（用于移动）
            DrawSquareHandle(center, handleSize, color);
        }

        /// <summary>
        /// 绘制方形控制点
        /// </summary>
        private void DrawSquareHandle(Vector2 position, float size, Color color)
        {
            _vertexCount = 0;

            float halfSize = size / 2;
            Vector2[] corners = new Vector2[4]
            {
                new Vector2(position.X - halfSize, position.Y - halfSize), // 左上
                new Vector2(position.X + halfSize, position.Y - halfSize), // 右上
                new Vector2(position.X + halfSize, position.Y + halfSize), // 右下
                new Vector2(position.X - halfSize, position.Y + halfSize)  // 左下
            };

            // 准备顶点
            for (int i = 0; i < 4; i++)
            {
                int nextIndex = (i + 1) % 4;
                AddLine(corners[i], corners[nextIndex], color);
            }

            // 绘制线条
            DrawLines();
        }
    }
}
