using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpineEditor.UI
{
    /// <summary>
    /// 动画列表框控件
    /// </summary>
    public class AnimationListBox
    {
        private GraphicsDevice _graphicsDevice;
        private SpriteFont _font;
        private List<string> _items;
        private int _selectedIndex = -1;
        private Rectangle _bounds;
        private Texture2D _texture;
        private MouseState _prevMouseState;
        private int _scrollOffset = 0;
        private int _maxVisibleItems;
        private bool _isHovered = false;
        private bool _visible = true;

        /// <summary>
        /// 选中项变更事件
        /// </summary>
        public event EventHandler SelectedIndexChanged;

        /// <summary>
        /// 获取或设置控件是否可见
        /// </summary>
        public bool Visible
        {
            get => _visible;
            set => _visible = value;
        }

        /// <summary>
        /// 获取或设置选中项的索引
        /// </summary>
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (value >= -1 && value < _items.Count)
                {
                    int oldIndex = _selectedIndex;
                    _selectedIndex = value;
                    if (oldIndex != _selectedIndex)
                    {
                        SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
        }

        /// <summary>
        /// 获取选中项的文本
        /// </summary>
        public string SelectedItem
        {
            get => _selectedIndex >= 0 && _selectedIndex < _items.Count ? _items[_selectedIndex] : null;
        }

        /// <summary>
        /// 获取或设置列表项
        /// </summary>
        public List<string> Items
        {
            get => _items;
            set
            {
                _items = value ?? new List<string>();
                if (_selectedIndex >= _items.Count)
                {
                    _selectedIndex = _items.Count > 0 ? 0 : -1;
                }
            }
        }

        /// <summary>
        /// 获取控件的边界
        /// </summary>
        public Rectangle Bounds => _bounds;

        /// <summary>
        /// 设置控件的边界
        /// </summary>
        /// <param name="bounds">新的边界</param>
        public void SetBounds(Rectangle bounds)
        {
            _bounds = bounds;
            _maxVisibleItems = Math.Max(1, bounds.Height / 30); // 更新可见项目数量
        }

        /// <summary>
        /// 创建动画列表框
        /// </summary>
        /// <param name="graphicsDevice">图形设备</param>
        /// <param name="font">字体</param>
        /// <param name="items">列表项</param>
        /// <param name="bounds">边界</param>
        public AnimationListBox(GraphicsDevice graphicsDevice, SpriteFont font, List<string> items, Rectangle bounds)
        {
            _graphicsDevice = graphicsDevice;
            _font = font;
            _items = items ?? new List<string>();
            _bounds = bounds;
            _maxVisibleItems = Math.Max(1, bounds.Height / 30); // 假设每个项目高度为30

            // 创建纹理
            _texture = new Texture2D(graphicsDevice, 1, 1);
            _texture.SetData(new[] { Color.White });

            _prevMouseState = Mouse.GetState();
        }

        /// <summary>
        /// 更新列表框
        /// </summary>
        public void Update()
        {
            if (!_visible)
                return;

            MouseState mouseState = Mouse.GetState();

            // 检查鼠标是否悬停在控件上
            _isHovered = _bounds.Contains(mouseState.Position);

            if (_isHovered)
            {
                // 处理点击
                if (mouseState.LeftButton == ButtonState.Released && _prevMouseState.LeftButton == ButtonState.Pressed)
                {
                    int itemHeight = 30;
                    int clickedItem = _scrollOffset + (mouseState.Y - _bounds.Y) / itemHeight;
                    if (clickedItem >= 0 && clickedItem < _items.Count)
                    {
                        SelectedIndex = clickedItem;
                    }
                }

                // 处理滚动
                if (mouseState.ScrollWheelValue != _prevMouseState.ScrollWheelValue)
                {
                    int scrollDelta = (mouseState.ScrollWheelValue - _prevMouseState.ScrollWheelValue) / 120;
                    _scrollOffset = MathHelper.Clamp(_scrollOffset - scrollDelta, 0, Math.Max(0, _items.Count - _maxVisibleItems));
                }
            }

            _prevMouseState = mouseState;
        }

        /// <summary>
        /// 绘制列表框
        /// </summary>
        /// <param name="spriteBatch">精灵批处理</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (!_visible)
                return;

            // 绘制列表框背景
            Color backgroundColor = _isHovered ? new Color(60, 60, 60) : new Color(50, 50, 50);
            spriteBatch.Draw(_texture, _bounds, backgroundColor);

            // 绘制边框
            Color borderColor = _isHovered ? new Color(120, 180, 210) : new Color(100, 100, 100);
            DrawBorder(spriteBatch, _bounds, borderColor, 1);

            // 绘制列表项
            int itemHeight = 30;
            int visibleItems = Math.Min(_items.Count, _maxVisibleItems);

            for (int i = 0; i < visibleItems; i++)
            {
                int itemIndex = i + _scrollOffset;
                if (itemIndex < _items.Count)
                {
                    Rectangle itemRect = new Rectangle(_bounds.X, _bounds.Y + i * itemHeight, _bounds.Width, itemHeight);

                    // 检查鼠标是否悬停在项目上
                    MouseState mouseState = Mouse.GetState();
                    bool isItemHovered = itemRect.Contains(mouseState.Position);

                    // 绘制项目背景
                    Color itemBackgroundColor;
                    if (itemIndex == _selectedIndex)
                    {
                        // 选中项使用蓝色调
                        itemBackgroundColor = isItemHovered ? new Color(80, 130, 180) : new Color(60, 105, 150);
                    }
                    else
                    {
                        // 非选中项
                        itemBackgroundColor = isItemHovered ? new Color(70, 70, 70) : new Color(55, 55, 55);
                    }
                    spriteBatch.Draw(_texture, itemRect, itemBackgroundColor);

                    // 绘制项目文本
                    string text = _items[itemIndex];
                    // 如果文本太长，截断它
                    if (_font.MeasureString(text).X > itemRect.Width - 10)
                    {
                        int maxChars = 0;
                        for (int j = 1; j <= text.Length; j++)
                        {
                            if (_font.MeasureString(text.Substring(0, j) + "...").X > itemRect.Width - 10)
                                break;
                            maxChars = j;
                        }
                        text = text.Substring(0, maxChars) + "...";
                    }

                    // 选中项使用白色文本，非选中项使用浅灰色文本
                    Color textColor = (itemIndex == _selectedIndex) ? Color.White : new Color(220, 220, 220);
                    spriteBatch.DrawString(_font, text, new Vector2(itemRect.X + 8, itemRect.Y + (itemRect.Height - _font.MeasureString(text).Y) / 2), textColor);

                    // 为选中项添加左侧指示条
                    if (itemIndex == _selectedIndex)
                    {
                        spriteBatch.Draw(_texture, new Rectangle(itemRect.X, itemRect.Y, 3, itemRect.Height), new Color(120, 180, 210));
                    }

                    // 添加分隔线
                    if (i < visibleItems - 1)
                    {
                        spriteBatch.Draw(_texture, new Rectangle(itemRect.X, itemRect.Y + itemRect.Height - 1, itemRect.Width, 1), new Color(70, 70, 70));
                    }
                }
            }

            // 如果有滚动条，绘制滚动条
            if (_items.Count > _maxVisibleItems)
            {
                int scrollBarWidth = 8;
                Rectangle scrollBarRect = new Rectangle(_bounds.X + _bounds.Width - scrollBarWidth - 2, _bounds.Y + 2, scrollBarWidth, _bounds.Height - 4);
                spriteBatch.Draw(_texture, scrollBarRect, new Color(40, 40, 40));

                // 计算滑块的大小和位置
                float thumbRatio = (float)_maxVisibleItems / _items.Count;
                int thumbHeight = Math.Max(20, (int)(_bounds.Height * thumbRatio));
                int maxOffset = Math.Max(1, _items.Count - _maxVisibleItems);
                int thumbY = _bounds.Y + 2 + (int)((_bounds.Height - 4 - thumbHeight) * ((float)_scrollOffset / maxOffset));
                Rectangle thumbRect = new Rectangle(scrollBarRect.X, thumbY, scrollBarRect.Width, thumbHeight);

                // 滑块颜色根据鼠标悬停状态变化
                Color thumbColor = _isHovered ? new Color(120, 120, 120) : new Color(80, 80, 80);
                spriteBatch.Draw(_texture, thumbRect, thumbColor);
            }
        }

        /// <summary>
        /// 绘制边框
        /// </summary>
        /// <param name="spriteBatch">精灵批处理</param>
        /// <param name="rectangle">矩形</param>
        /// <param name="color">颜色</param>
        /// <param name="thickness">厚度</param>
        private void DrawBorder(SpriteBatch spriteBatch, Rectangle rectangle, Color color, int thickness)
        {
            // 上边框
            spriteBatch.Draw(_texture, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, thickness), color);
            // 下边框
            spriteBatch.Draw(_texture, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height - thickness, rectangle.Width, thickness), color);
            // 左边框
            spriteBatch.Draw(_texture, new Rectangle(rectangle.X, rectangle.Y, thickness, rectangle.Height), color);
            // 右边框
            spriteBatch.Draw(_texture, new Rectangle(rectangle.X + rectangle.Width - thickness, rectangle.Y, thickness, rectangle.Height), color);
        }
    }
}
