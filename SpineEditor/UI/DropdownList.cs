using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpineEditor.UI
{
    /// <summary>
    /// 下拉列表控件
    /// </summary>
    public class DropdownList
    {
        private Rectangle _bounds;
        private string _label;
        private List<string> _items;
        private int _selectedIndex = -1;
        private bool _isExpanded = false;
        private Texture2D _texture;
        private SpriteFont _font;
        private MouseState _prevMouseState;
        private int _maxVisibleItems = 5;
        private int _scrollOffset = 0;
        private bool _isHovered = false;
        private bool _visible = true;

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
        /// 获取或设置项目列表
        /// </summary>
        public List<string> Items
        {
            get => _items;
            set
            {
                _items = value;
                if (_selectedIndex >= _items.Count)
                {
                    SelectedIndex = _items.Count > 0 ? 0 : -1;
                }
            }
        }

        /// <summary>
        /// 获取或设置边界
        /// </summary>
        public Rectangle Bounds
        {
            get => _bounds;
            set => _bounds = value;
        }

        /// <summary>
        /// 选中项变更事件
        /// </summary>
        public event EventHandler SelectedIndexChanged;

        /// <summary>
        /// 创建下拉列表
        /// </summary>
        /// <param name="graphicsDevice">图形设备</param>
        /// <param name="font">字体</param>
        /// <param name="label">标签</param>
        /// <param name="items">项目列表</param>
        /// <param name="bounds">边界</param>
        public DropdownList(GraphicsDevice graphicsDevice, SpriteFont font, string label, List<string> items, Rectangle bounds)
        {
            _font = font;
            _label = label;
            _items = items;
            _bounds = bounds;
            _selectedIndex = items.Count > 0 ? 0 : -1;

            // 创建纹理
            _texture = new Texture2D(graphicsDevice, 1, 1);
            _texture.SetData(new[] { Color.White });

            _prevMouseState = Mouse.GetState();
        }

        /// <summary>
        /// 更新下拉列表
        /// </summary>
        public void Update()
        {
            MouseState mouseState = Mouse.GetState();

            // 计算下拉列表的区域（即使未展开也计算，用于检测点击）
            int itemHeight = 30;
            int visibleItems = Math.Min(_items.Count, _maxVisibleItems);
            Rectangle dropdownRect = new Rectangle(_bounds.X, _bounds.Y + _bounds.Height, _bounds.Width, itemHeight * visibleItems);

            // 检查鼠标是否悬停在控件上
            _isHovered = _bounds.Contains(mouseState.Position);

            // 处理主控件的点击事件
            if (_isHovered && mouseState.LeftButton == ButtonState.Released && _prevMouseState.LeftButton == ButtonState.Pressed)
            {
                _isExpanded = !_isExpanded;
            }

            // 如果下拉列表展开，处理项目选择
            if (_isExpanded)
            {
                // 检查鼠标是否在下拉列表区域内
                bool isMouseOverDropdown = dropdownRect.Contains(mouseState.Position);

                if (isMouseOverDropdown)
                {
                    // 计算鼠标悬停的项目索引
                    int hoveredIndex = _scrollOffset + (mouseState.Y - dropdownRect.Y) / itemHeight;

                    // 如果点击了项目，选中它并关闭下拉列表
                    if (mouseState.LeftButton == ButtonState.Released && _prevMouseState.LeftButton == ButtonState.Pressed)
                    {
                        if (hoveredIndex >= 0 && hoveredIndex < _items.Count)
                        {
                            // 记录旧的选中索引
                            int oldIndex = _selectedIndex;

                            // 设置新的选中索引
                            _selectedIndex = hoveredIndex;

                            // 如果索引发生变化，触发事件
                            if (oldIndex != _selectedIndex)
                            {
                                SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
                                Console.WriteLine($"Selected animation changed to: {_items[_selectedIndex]}");
                            }

                            _isExpanded = false;
                        }
                    }

                    // 处理滚动
                    if (mouseState.ScrollWheelValue != _prevMouseState.ScrollWheelValue)
                    {
                        int scrollDelta = (mouseState.ScrollWheelValue - _prevMouseState.ScrollWheelValue) / 120;
                        _scrollOffset = MathHelper.Clamp(_scrollOffset - scrollDelta, 0, Math.Max(0, _items.Count - _maxVisibleItems));
                    }
                }
                // 如果点击了下拉列表外部，关闭下拉列表
                else if (mouseState.LeftButton == ButtonState.Released && _prevMouseState.LeftButton == ButtonState.Pressed)
                {
                    _isExpanded = false;
                }
            }

            _prevMouseState = mouseState;
        }

        /// <summary>
        /// 绘制下拉列表
        /// </summary>
        /// <param name="spriteBatch">精灵批处理</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            // 如果控件不可见，则不绘制
            if (!_visible)
                return;

            // 绘制标签
            if (!string.IsNullOrEmpty(_label))
            {
                spriteBatch.DrawString(_font, _label, new Vector2(_bounds.X, _bounds.Y - 20), Color.White);
            }

            // 绘制下拉列表框
            Color backgroundColor = _isHovered ? new Color(60, 60, 60) : new Color(40, 40, 40);
            spriteBatch.Draw(_texture, _bounds, backgroundColor);

            // 绘制边框
            Color borderColor = _isHovered ? Color.White : new Color(100, 100, 100);
            DrawBorder(spriteBatch, _bounds, borderColor, 1);

            // 绘制选中项
            if (_selectedIndex >= 0 && _selectedIndex < _items.Count)
            {
                string text = _items[_selectedIndex];
                // 如果文本太长，截断它
                if (_font.MeasureString(text).X > _bounds.Width - 30)
                {
                    int maxChars = 0;
                    for (int i = 1; i <= text.Length; i++)
                    {
                        if (_font.MeasureString(text.Substring(0, i) + "...").X > _bounds.Width - 30)
                            break;
                        maxChars = i;
                    }
                    text = text.Substring(0, maxChars) + "...";
                }
                spriteBatch.DrawString(_font, text, new Vector2(_bounds.X + 5, _bounds.Y + (_bounds.Height - _font.MeasureString(text).Y) / 2), Color.White);
            }

            // 绘制下拉箭头
            DrawArrow(spriteBatch, new Vector2(_bounds.X + _bounds.Width - 20, _bounds.Y + _bounds.Height / 2), _isExpanded);

            // 如果下拉列表展开，绘制项目列表
            if (_isExpanded && _items.Count > 0)
            {
                int itemHeight = 30;
                int visibleItems = Math.Min(_items.Count, _maxVisibleItems);
                Rectangle dropdownRect = new Rectangle(_bounds.X, _bounds.Y + _bounds.Height, _bounds.Width, itemHeight * visibleItems);

                // 绘制下拉列表背景
                spriteBatch.Draw(_texture, dropdownRect, new Color(50, 50, 50));

                // 绘制边框
                DrawBorder(spriteBatch, dropdownRect, new Color(100, 100, 100), 1);

                // 绘制项目
                for (int i = 0; i < visibleItems; i++)
                {
                    int itemIndex = i + _scrollOffset;
                    if (itemIndex < _items.Count)
                    {
                        Rectangle itemRect = new Rectangle(dropdownRect.X, dropdownRect.Y + i * itemHeight, dropdownRect.Width, itemHeight);

                        // 检查鼠标是否悬停在项目上
                        MouseState mouseState = Mouse.GetState();
                        bool isItemHovered = itemRect.Contains(mouseState.Position);

                        // 绘制项目背景
                        Color itemBackgroundColor = isItemHovered ? new Color(70, 70, 70) : (itemIndex == _selectedIndex ? new Color(60, 60, 60) : new Color(50, 50, 50));
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
                        spriteBatch.DrawString(_font, text, new Vector2(itemRect.X + 5, itemRect.Y + (itemRect.Height - _font.MeasureString(text).Y) / 2), Color.White);
                    }
                }

                // 如果有滚动条，绘制滚动条
                if (_items.Count > _maxVisibleItems)
                {
                    int scrollBarWidth = 10;
                    Rectangle scrollBarRect = new Rectangle(dropdownRect.X + dropdownRect.Width - scrollBarWidth, dropdownRect.Y, scrollBarWidth, dropdownRect.Height);
                    spriteBatch.Draw(_texture, scrollBarRect, new Color(30, 30, 30));

                    // 计算滑块的大小和位置
                    float thumbRatio = (float)_maxVisibleItems / _items.Count;
                    int thumbHeight = (int)(dropdownRect.Height * thumbRatio);
                    int thumbY = dropdownRect.Y + (int)((dropdownRect.Height - thumbHeight) * ((float)_scrollOffset / (_items.Count - _maxVisibleItems)));
                    Rectangle thumbRect = new Rectangle(scrollBarRect.X, thumbY, scrollBarRect.Width, thumbHeight);
                    spriteBatch.Draw(_texture, thumbRect, new Color(100, 100, 100));
                }
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

        /// <summary>
        /// 绘制箭头
        /// </summary>
        /// <param name="spriteBatch">精灵批处理</param>
        /// <param name="position">位置</param>
        /// <param name="isUp">是否向上</param>
        private void DrawArrow(SpriteBatch spriteBatch, Vector2 position, bool isUp)
        {
            int arrowSize = 8;
            if (isUp)
            {
                // 绘制向上箭头
                for (int i = 0; i < arrowSize; i++)
                {
                    spriteBatch.Draw(_texture, new Rectangle((int)position.X - arrowSize + i, (int)position.Y + i, 2, 1), Color.White);
                    spriteBatch.Draw(_texture, new Rectangle((int)position.X + arrowSize - i, (int)position.Y + i, 2, 1), Color.White);
                }
            }
            else
            {
                // 绘制向下箭头
                for (int i = 0; i < arrowSize; i++)
                {
                    spriteBatch.Draw(_texture, new Rectangle((int)position.X - arrowSize + i, (int)position.Y - i, 2, 1), Color.White);
                    spriteBatch.Draw(_texture, new Rectangle((int)position.X + arrowSize - i, (int)position.Y - i, 2, 1), Color.White);
                }
            }
        }
    }
}
