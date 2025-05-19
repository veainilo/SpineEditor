using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpineEditor.UI.UISystem
{
    /// <summary>
    /// GUILayout窗口基类，提供基本的GUILayout功能
    /// </summary>
    public abstract class GUILayoutWindow
    {
        // UI管理器
        protected UIManager _uiManager;
        
        // 字体
        protected SpriteFont _font;
        
        // 图形设备
        protected GraphicsDevice _graphicsDevice;
        
        // 窗口标题
        protected string _title;
        
        // 窗口边界
        protected Rectangle _bounds;
        
        // 是否可拖动
        protected bool _draggable = true;
        
        // 是否正在拖动
        protected bool _isDragging = false;
        
        // 拖动开始位置
        protected Point _dragStart;
        
        /// <summary>
        /// 创建GUILayout窗口
        /// </summary>
        /// <param name="title">窗口标题</param>
        /// <param name="bounds">窗口边界</param>
        /// <param name="graphicsDevice">图形设备</param>
        /// <param name="font">字体</param>
        public GUILayoutWindow(string title, Rectangle bounds, GraphicsDevice graphicsDevice, SpriteFont font)
        {
            _title = title;
            _bounds = bounds;
            _graphicsDevice = graphicsDevice;
            _font = font;
            
            // 创建UI管理器
            _uiManager = new UIManager(graphicsDevice);
            
            // 初始化GUILayout系统
            GUILayout.Initialize(_uiManager, font);
        }
        
        /// <summary>
        /// 更新窗口
        /// </summary>
        /// <param name="gameTime">游戏时间</param>
        public virtual void Update(GameTime gameTime)
        {
            // 更新UI管理器
            _uiManager.Update(gameTime);
        }
        
        /// <summary>
        /// 绘制窗口
        /// </summary>
        /// <param name="spriteBatch">精灵批处理</param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            // 绘制窗口框架
            DrawWindowFrame(spriteBatch);
            
            // 绘制GUI内容
            DrawGUI();
            
            // 绘制UI管理器
            _uiManager.Draw(spriteBatch);
        }
        
        /// <summary>
        /// 绘制窗口框架
        /// </summary>
        /// <param name="spriteBatch">精灵批处理</param>
        protected virtual void DrawWindowFrame(SpriteBatch spriteBatch)
        {
            // 绘制窗口背景
            spriteBatch.Draw(TextureManager.Pixel, _bounds, new Color(40, 40, 40, 200));
            
            // 绘制窗口边框
            DrawBorder(spriteBatch, _bounds, Color.Gray, 1);
            
            // 绘制窗口标题栏
            Rectangle titleBar = new Rectangle(_bounds.X, _bounds.Y, _bounds.Width, 30);
            spriteBatch.Draw(TextureManager.Pixel, titleBar, new Color(60, 60, 60));
            
            // 绘制窗口标题
            if (_font != null)
            {
                Vector2 titleSize = _font.MeasureString(_title);
                Vector2 titlePosition = new Vector2(
                    _bounds.X + 10,
                    _bounds.Y + (titleBar.Height - titleSize.Y) / 2
                );
                spriteBatch.DrawString(_font, _title, titlePosition, Color.White);
            }
        }
        
        /// <summary>
        /// 绘制边框
        /// </summary>
        /// <param name="spriteBatch">精灵批处理</param>
        /// <param name="rectangle">矩形</param>
        /// <param name="color">颜色</param>
        /// <param name="thickness">厚度</param>
        protected void DrawBorder(SpriteBatch spriteBatch, Rectangle rectangle, Color color, int thickness)
        {
            // 上边框
            spriteBatch.Draw(TextureManager.Pixel, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, thickness), color);
            // 下边框
            spriteBatch.Draw(TextureManager.Pixel, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height - thickness, rectangle.Width, thickness), color);
            // 左边框
            spriteBatch.Draw(TextureManager.Pixel, new Rectangle(rectangle.X, rectangle.Y, thickness, rectangle.Height), color);
            // 右边框
            spriteBatch.Draw(TextureManager.Pixel, new Rectangle(rectangle.X + rectangle.Width - thickness, rectangle.Y, thickness, rectangle.Height), color);
        }
        
        /// <summary>
        /// 绘制GUI内容 - 子类必须实现此方法
        /// </summary>
        protected abstract void DrawGUI();
        
        /// <summary>
        /// 处理鼠标输入
        /// </summary>
        /// <param name="mousePosition">鼠标位置</param>
        /// <param name="leftButtonPressed">左键是否按下</param>
        /// <param name="leftButtonJustPressed">左键是否刚刚按下</param>
        /// <param name="leftButtonJustReleased">左键是否刚刚释放</param>
        /// <returns>是否处理了输入</returns>
        public virtual bool HandleMouseInput(Point mousePosition, bool leftButtonPressed, bool leftButtonJustPressed, bool leftButtonJustReleased)
        {
            // 检查是否在窗口内
            if (!_bounds.Contains(mousePosition))
                return false;
                
            // 检查是否在标题栏内
            Rectangle titleBar = new Rectangle(_bounds.X, _bounds.Y, _bounds.Width, 30);
            if (_draggable && titleBar.Contains(mousePosition))
            {
                // 开始拖动
                if (leftButtonJustPressed)
                {
                    _isDragging = true;
                    _dragStart = mousePosition;
                    return true;
                }
                
                // 结束拖动
                if (leftButtonJustReleased && _isDragging)
                {
                    _isDragging = false;
                    return true;
                }
                
                // 拖动中
                if (leftButtonPressed && _isDragging)
                {
                    int deltaX = mousePosition.X - _dragStart.X;
                    int deltaY = mousePosition.Y - _dragStart.Y;
                    _bounds.X += deltaX;
                    _bounds.Y += deltaY;
                    _dragStart = mousePosition;
                    return true;
                }
            }
            
            return false;
        }
    }
}
