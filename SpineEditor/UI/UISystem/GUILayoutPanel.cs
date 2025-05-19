using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpineEditor.UI.UISystem
{
    /// <summary>
    /// GUILayout面板基类，提供基本的GUILayout功能
    /// </summary>
    public abstract class GUILayoutPanel
    {
        // UI管理器
        protected UIManager _uiManager;
        
        // 字体
        protected SpriteFont _font;
        
        // 图形设备
        protected GraphicsDevice _graphicsDevice;
        
        // 面板标题
        protected string _title;
        
        // 面板边界
        protected Rectangle _bounds;
        
        // 是否显示标题
        protected bool _showTitle = true;
        
        // 标题高度
        protected int _titleHeight = 25;
        
        // 内容区域
        protected Rectangle _contentBounds;
        
        /// <summary>
        /// 创建GUILayout面板
        /// </summary>
        /// <param name="title">面板标题</param>
        /// <param name="bounds">面板边界</param>
        /// <param name="graphicsDevice">图形设备</param>
        /// <param name="font">字体</param>
        public GUILayoutPanel(string title, Rectangle bounds, GraphicsDevice graphicsDevice, SpriteFont font)
        {
            _title = title;
            _bounds = bounds;
            _graphicsDevice = graphicsDevice;
            _font = font;
            
            // 计算内容区域
            UpdateContentBounds();
            
            // 创建UI管理器
            _uiManager = new UIManager(graphicsDevice);
            
            // 初始化GUILayout系统
            GUILayout.Initialize(_uiManager, font);
        }
        
        /// <summary>
        /// 更新面板
        /// </summary>
        /// <param name="gameTime">游戏时间</param>
        public virtual void Update(GameTime gameTime)
        {
            // 更新UI管理器
            _uiManager.Update(gameTime);
        }
        
        /// <summary>
        /// 绘制面板
        /// </summary>
        /// <param name="spriteBatch">精灵批处理</param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            // 绘制面板框架
            DrawPanelFrame(spriteBatch);
            
            // 绘制GUI内容
            DrawGUI();
            
            // 绘制UI管理器
            _uiManager.Draw(spriteBatch);
        }
        
        /// <summary>
        /// 绘制面板框架
        /// </summary>
        /// <param name="spriteBatch">精灵批处理</param>
        protected virtual void DrawPanelFrame(SpriteBatch spriteBatch)
        {
            // 绘制面板背景
            spriteBatch.Draw(TextureManager.Pixel, _bounds, new Color(40, 40, 40, 200));
            
            // 绘制面板边框
            DrawBorder(spriteBatch, _bounds, Color.Gray, 1);
            
            // 如果显示标题，绘制标题栏
            if (_showTitle)
            {
                Rectangle titleBar = new Rectangle(_bounds.X, _bounds.Y, _bounds.Width, _titleHeight);
                spriteBatch.Draw(TextureManager.Pixel, titleBar, new Color(60, 60, 60));
                
                // 绘制面板标题
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
        /// 更新内容区域
        /// </summary>
        protected void UpdateContentBounds()
        {
            if (_showTitle)
            {
                _contentBounds = new Rectangle(
                    _bounds.X,
                    _bounds.Y + _titleHeight,
                    _bounds.Width,
                    _bounds.Height - _titleHeight
                );
            }
            else
            {
                _contentBounds = _bounds;
            }
        }
        
        /// <summary>
        /// 设置面板边界
        /// </summary>
        /// <param name="bounds">新的边界</param>
        public void SetBounds(Rectangle bounds)
        {
            _bounds = bounds;
            UpdateContentBounds();
        }
        
        /// <summary>
        /// 绘制GUI内容 - 子类必须实现此方法
        /// </summary>
        protected abstract void DrawGUI();
    }
}
