using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SpineEditor.Events;
using System;

namespace SpineEditor.UI
{
    /// <summary>
    /// 拖拽操作类型
    /// </summary>
    public enum DragOperationType
    {
        /// <summary>
        /// 无操作
        /// </summary>
        None,

        /// <summary>
        /// 移动位置
        /// </summary>
        Move,

        /// <summary>
        /// 调整左边
        /// </summary>
        ResizeLeft,

        /// <summary>
        /// 调整右边
        /// </summary>
        ResizeRight,

        /// <summary>
        /// 调整上边
        /// </summary>
        ResizeTop,

        /// <summary>
        /// 调整下边
        /// </summary>
        ResizeBottom,

        /// <summary>
        /// 调整左上角
        /// </summary>
        ResizeTopLeft,

        /// <summary>
        /// 调整右上角
        /// </summary>
        ResizeTopRight,

        /// <summary>
        /// 调整左下角
        /// </summary>
        ResizeBottomLeft,

        /// <summary>
        /// 调整右下角
        /// </summary>
        ResizeBottomRight
    }

    /// <summary>
    /// 形状拖拽处理器，用于处理攻击形状的拖拽操作
    /// </summary>
    public class ShapeDragHandler
    {
        // 拖拽操作相关
        private bool _isDragging = false;
        private DragOperationType _dragOperation = DragOperationType.None;
        private Vector2 _dragStartPosition;
        private Vector2 _dragStartMousePosition;
        private AttackShape _originalShape;
        private AttackShape _currentShape;
        private Vector2 _spinePosition;
        private float _spineScale;
        private MouseState _prevMouseState;

        // 拖拽操作的灵敏度和判定范围
        private const int DRAG_HANDLE_SIZE = 8;
        private const float MIN_SHAPE_SIZE = 10.0f;

        /// <summary>
        /// 获取或设置当前正在拖拽的形状
        /// </summary>
        public AttackShape CurrentShape
        {
            get => _currentShape;
            set
            {
                _currentShape = value;
                if (value != null)
                {
                    // 创建原始形状的副本，用于计算拖拽操作
                    _originalShape = new AttackShape
                    {
                        Type = value.Type,
                        X = value.X,
                        Y = value.Y,
                        Width = value.Width,
                        Height = value.Height,
                        Rotation = value.Rotation
                    };
                }
                else
                {
                    _originalShape = null;
                }
            }
        }

        /// <summary>
        /// 获取是否正在拖拽
        /// </summary>
        public bool IsDragging => _isDragging;

        /// <summary>
        /// 获取当前拖拽操作类型
        /// </summary>
        public DragOperationType DragOperation => _dragOperation;

        /// <summary>
        /// 创建形状拖拽处理器
        /// </summary>
        public ShapeDragHandler()
        {
            _prevMouseState = Mouse.GetState();
        }

        /// <summary>
        /// 更新拖拽操作
        /// </summary>
        /// <param name="gameTime">游戏时间</param>
        /// <param name="spinePosition">Spine动画位置</param>
        /// <param name="spineScale">Spine动画缩放</param>
        /// <returns>是否处理了鼠标事件</returns>
        public bool Update(GameTime gameTime, Vector2 spinePosition, float spineScale)
        {
            if (_currentShape == null)
                return false;

            _spinePosition = spinePosition;
            _spineScale = spineScale;

            MouseState mouseState = Mouse.GetState();
            Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y);

            // 如果没有在拖拽，检查是否开始拖拽
            if (!_isDragging)
            {
                if (_prevMouseState.LeftButton == ButtonState.Released && mouseState.LeftButton == ButtonState.Pressed)
                {
                    // 检查鼠标是否在形状上
                    _dragOperation = GetDragOperationAtPosition(mousePosition);
                    if (_dragOperation != DragOperationType.None)
                    {
                        _isDragging = true;
                        _dragStartPosition = new Vector2(_currentShape.X, _currentShape.Y);
                        _dragStartMousePosition = mousePosition;
                        return true;
                    }
                }
            }
            // 如果正在拖拽，处理拖拽操作
            else
            {
                // 如果释放鼠标，结束拖拽
                if (mouseState.LeftButton == ButtonState.Released)
                {
                    _isDragging = false;
                    _dragOperation = DragOperationType.None;
                    return true;
                }

                // 计算鼠标移动距离（相对于Spine坐标系）
                Vector2 mouseDelta = mousePosition - _dragStartMousePosition;
                float deltaX = mouseDelta.X / _spineScale;
                float deltaY = mouseDelta.Y / _spineScale;

                // 根据拖拽操作类型更新形状
                switch (_dragOperation)
                {
                    case DragOperationType.Move:
                        _currentShape.X = _originalShape.X + deltaX;
                        _currentShape.Y = _originalShape.Y + deltaY;
                        break;

                    case DragOperationType.ResizeRight:
                        _currentShape.Width = Math.Max(MIN_SHAPE_SIZE, _originalShape.Width + deltaX);
                        break;

                    case DragOperationType.ResizeLeft:
                        float newWidth = Math.Max(MIN_SHAPE_SIZE, _originalShape.Width - deltaX);
                        float widthDiff = _originalShape.Width - newWidth;
                        _currentShape.X = _originalShape.X + widthDiff;
                        _currentShape.Width = newWidth;
                        break;

                    case DragOperationType.ResizeBottom:
                        _currentShape.Height = Math.Max(MIN_SHAPE_SIZE, _originalShape.Height + deltaY);
                        break;

                    case DragOperationType.ResizeTop:
                        float newHeight = Math.Max(MIN_SHAPE_SIZE, _originalShape.Height - deltaY);
                        float heightDiff = _originalShape.Height - newHeight;
                        _currentShape.Y = _originalShape.Y + heightDiff;
                        _currentShape.Height = newHeight;
                        break;

                    case DragOperationType.ResizeTopLeft:
                        // 调整宽度
                        newWidth = Math.Max(MIN_SHAPE_SIZE, _originalShape.Width - deltaX);
                        widthDiff = _originalShape.Width - newWidth;
                        _currentShape.X = _originalShape.X + widthDiff;
                        _currentShape.Width = newWidth;

                        // 调整高度
                        newHeight = Math.Max(MIN_SHAPE_SIZE, _originalShape.Height - deltaY);
                        heightDiff = _originalShape.Height - newHeight;
                        _currentShape.Y = _originalShape.Y + heightDiff;
                        _currentShape.Height = newHeight;
                        break;

                    case DragOperationType.ResizeTopRight:
                        // 调整宽度
                        _currentShape.Width = Math.Max(MIN_SHAPE_SIZE, _originalShape.Width + deltaX);

                        // 调整高度
                        newHeight = Math.Max(MIN_SHAPE_SIZE, _originalShape.Height - deltaY);
                        heightDiff = _originalShape.Height - newHeight;
                        _currentShape.Y = _originalShape.Y + heightDiff;
                        _currentShape.Height = newHeight;
                        break;

                    case DragOperationType.ResizeBottomLeft:
                        // 调整宽度
                        newWidth = Math.Max(MIN_SHAPE_SIZE, _originalShape.Width - deltaX);
                        widthDiff = _originalShape.Width - newWidth;
                        _currentShape.X = _originalShape.X + widthDiff;
                        _currentShape.Width = newWidth;

                        // 调整高度
                        _currentShape.Height = Math.Max(MIN_SHAPE_SIZE, _originalShape.Height + deltaY);
                        break;

                    case DragOperationType.ResizeBottomRight:
                        // 调整宽度和高度
                        _currentShape.Width = Math.Max(MIN_SHAPE_SIZE, _originalShape.Width + deltaX);
                        _currentShape.Height = Math.Max(MIN_SHAPE_SIZE, _originalShape.Height + deltaY);
                        break;
                }

                return true;
            }

            _prevMouseState = mouseState;
            return false;
        }

        /// <summary>
        /// 获取指定位置的拖拽操作类型
        /// </summary>
        /// <param name="position">鼠标位置</param>
        /// <returns>拖拽操作类型</returns>
        private DragOperationType GetDragOperationAtPosition(Vector2 position)
        {
            if (_currentShape == null)
                return DragOperationType.None;

            // 计算形状在屏幕上的位置和大小
            Vector2 shapePosition = new Vector2(
                _spinePosition.X + _currentShape.X * _spineScale,
                _spinePosition.Y + _currentShape.Y * _spineScale
            );
            float shapeWidth = _currentShape.Width * _spineScale;
            float shapeHeight = _currentShape.Height * _spineScale;

            // 计算形状的边界
            Rectangle shapeBounds = new Rectangle(
                (int)(shapePosition.X - shapeWidth / 2),
                (int)(shapePosition.Y - shapeHeight / 2),
                (int)shapeWidth,
                (int)shapeHeight
            );

            // 检查是否在调整大小的控制点上
            // 左上角
            if (IsNearPoint(position, new Vector2(shapeBounds.Left, shapeBounds.Top), DRAG_HANDLE_SIZE))
                return DragOperationType.ResizeTopLeft;

            // 右上角
            if (IsNearPoint(position, new Vector2(shapeBounds.Right, shapeBounds.Top), DRAG_HANDLE_SIZE))
                return DragOperationType.ResizeTopRight;

            // 左下角
            if (IsNearPoint(position, new Vector2(shapeBounds.Left, shapeBounds.Bottom), DRAG_HANDLE_SIZE))
                return DragOperationType.ResizeBottomLeft;

            // 右下角
            if (IsNearPoint(position, new Vector2(shapeBounds.Right, shapeBounds.Bottom), DRAG_HANDLE_SIZE))
                return DragOperationType.ResizeBottomRight;

            // 左边
            if (IsNearPoint(position, new Vector2(shapeBounds.Left, shapeBounds.Center.Y), DRAG_HANDLE_SIZE))
                return DragOperationType.ResizeLeft;

            // 右边
            if (IsNearPoint(position, new Vector2(shapeBounds.Right, shapeBounds.Center.Y), DRAG_HANDLE_SIZE))
                return DragOperationType.ResizeRight;

            // 上边
            if (IsNearPoint(position, new Vector2(shapeBounds.Center.X, shapeBounds.Top), DRAG_HANDLE_SIZE))
                return DragOperationType.ResizeTop;

            // 下边
            if (IsNearPoint(position, new Vector2(shapeBounds.Center.X, shapeBounds.Bottom), DRAG_HANDLE_SIZE))
                return DragOperationType.ResizeBottom;

            // 检查是否在形状内部（用于移动）
            if (shapeBounds.Contains(position))
                return DragOperationType.Move;

            return DragOperationType.None;
        }

        /// <summary>
        /// 检查位置是否接近指定点
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="point">指定点</param>
        /// <param name="threshold">阈值</param>
        /// <returns>是否接近</returns>
        private bool IsNearPoint(Vector2 position, Vector2 point, float threshold)
        {
            return Vector2.Distance(position, point) <= threshold;
        }
    }
}
