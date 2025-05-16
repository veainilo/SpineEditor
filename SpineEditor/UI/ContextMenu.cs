using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpineEditor.UI
{
    /// <summary>
    /// 表示上下文菜单中的一个菜单项
    /// </summary>
    public class MenuItem
    {
        /// <summary>
        /// 菜单项文本
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 菜单项点击事件
        /// </summary>
        public event EventHandler Click;

        /// <summary>
        /// 触发点击事件
        /// </summary>
        public void OnClick()
        {
            Click?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 创建菜单项
        /// </summary>
        /// <param name="text">菜单项文本</param>
        public MenuItem(string text)
        {
            Text = text;
        }
    }

    /// <summary>
    /// 上下文菜单控件
    /// </summary>
    public class ContextMenu
    {
        private List<MenuItem> _items = new List<MenuItem>();
        private Rectangle _bounds;
        private bool _isVisible = false;
        private Texture2D _texture;
        private SpriteFont _font;
        private MouseState _prevMouseState;
        private int _itemHeight = 30;
        private int _menuWidth = 150;
        private Vector2 _position;

        /// <summary>
        /// 获取菜单项列表
        /// </summary>
        public List<MenuItem> Items => _items;

        /// <summary>
        /// 获取或设置菜单是否可见
        /// </summary>
        public bool IsVisible
        {
            get => _isVisible;
            set => _isVisible = value;
        }

        /// <summary>
        /// 创建上下文菜单
        /// </summary>
        /// <param name="graphicsDevice">图形设备</param>
        /// <param name="font">字体</param>
        public ContextMenu(GraphicsDevice graphicsDevice, SpriteFont font)
        {
            _font = font;
            
            // 创建纹理
            _texture = new Texture2D(graphicsDevice, 1, 1);
            _texture.SetData(new[] { Color.White });
            
            _prevMouseState = Mouse.GetState();
        }

        /// <summary>
        /// 添加菜单项
        /// </summary>
        /// <param name="text">菜单项文本</param>
        /// <returns>添加的菜单项</returns>
        public MenuItem AddItem(string text)
        {
            MenuItem item = new MenuItem(text);
            _items.Add(item);
            return item;
        }

        /// <summary>
        /// 显示菜单
        /// </summary>
        /// <param name="position">显示位置</param>
        public void Show(Vector2 position)
        {
            _position = position;
            _isVisible = true;
            
            // 计算菜单边界
            _bounds = new Rectangle(
                (int)position.X,
                (int)position.Y,
                _menuWidth,
                _itemHeight * _items.Count
            );
        }

        /// <summary>
        /// 隐藏菜单
        /// </summary>
        public void Hide()
        {
            _isVisible = false;
        }

        /// <summary>
        /// 更新菜单
        /// </summary>
        public void Update()
        {
            if (!_isVisible)
                return;
                
            MouseState mouseState = Mouse.GetState();
            
            // 检查鼠标是否在菜单区域内
            bool isMouseOverMenu = _bounds.Contains(mouseState.Position);
            
            // 如果鼠标在菜单区域内并且点击了左键
            if (isMouseOverMenu && mouseState.LeftButton == ButtonState.Released && _prevMouseState.LeftButton == ButtonState.Pressed)
            {
                // 计算点击的菜单项索引
                int itemIndex = (mouseState.Y - _bounds.Y) / _itemHeight;
                
                // 如果点击了有效的菜单项
                if (itemIndex >= 0 && itemIndex < _items.Count)
                {
                    // 触发菜单项点击事件
                    _items[itemIndex].OnClick();
                    
                    // 隐藏菜单
                    Hide();
                }
            }
            // 如果鼠标不在菜单区域内并且点击了左键或右键，隐藏菜单
            else if (!isMouseOverMenu && 
                    (mouseState.LeftButton == ButtonState.Pressed || mouseState.RightButton == ButtonState.Pressed) && 
                    (_prevMouseState.LeftButton == ButtonState.Released || _prevMouseState.RightButton == ButtonState.Released))
            {
                Hide();
            }
            
            _prevMouseState = mouseState;
        }

        /// <summary>
        /// 绘制菜单
        /// </summary>
        /// <param name="spriteBatch">精灵批处理</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (!_isVisible)
                return;
                
            // 绘制菜单背景
            spriteBatch.Draw(_texture, _bounds, new Color(50, 50, 50));
            
            // 绘制菜单边框
            DrawBorder(spriteBatch, _bounds, new Color(100, 100, 100), 1);
            
            // 绘制菜单项
            for (int i = 0; i < _items.Count; i++)
            {
                Rectangle itemRect = new Rectangle(
                    _bounds.X,
                    _bounds.Y + i * _itemHeight,
                    _bounds.Width,
                    _itemHeight
                );
                
                // 检查鼠标是否悬停在菜单项上
                MouseState mouseState = Mouse.GetState();
                bool isItemHovered = itemRect.Contains(mouseState.Position);
                
                // 绘制菜单项背景
                Color itemBackgroundColor = isItemHovered ? new Color(70, 70, 70) : new Color(50, 50, 50);
                spriteBatch.Draw(_texture, itemRect, itemBackgroundColor);
                
                // 绘制菜单项文本
                spriteBatch.DrawString(
                    _font,
                    _items[i].Text,
                    new Vector2(itemRect.X + 10, itemRect.Y + (_itemHeight - _font.LineSpacing) / 2),
                    Color.White
                );
                
                // 如果不是最后一个菜单项，绘制分隔线
                if (i < _items.Count - 1)
                {
                    spriteBatch.Draw(
                        _texture,
                        new Rectangle(itemRect.X, itemRect.Y + itemRect.Height - 1, itemRect.Width, 1),
                        new Color(70, 70, 70)
                    );
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
    }
}
