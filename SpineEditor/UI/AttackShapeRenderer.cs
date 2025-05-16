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
        }

        /// <summary>
        /// 绘制攻击形状
        /// </summary>
        /// <param name="attackShape">攻击形状数据</param>
        /// <param name="position">Spine动画的位置（原点位置）</param>
        /// <param name="scale">缩放</param>
        /// <param name="color">颜色</param>
        /// <remarks>
        /// 攻击形状的坐标是相对于Spine动画原点的，需要将其转换为屏幕坐标
        /// </remarks>
        public void DrawAttackShape(AttackShape attackShape, Vector2 position, float scale, Color color)
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
                    break;

                case ShapeType.Circle:
                    DrawCircle(
                        actualPosition,
                        attackShape.Width * scale, // 半径
                        32, // 分段数
                        color
                    );
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
    }
}
