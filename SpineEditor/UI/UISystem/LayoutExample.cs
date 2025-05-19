using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpineEditor.UI.UISystem
{
    /// <summary>
    /// 布局示例类，展示如何使用水平和垂直布局
    /// </summary>
    public class LayoutExample
    {
        private UIManager _uiManager;
        private GraphicsDevice _graphicsDevice;
        private SpriteFont _font;
        
        /// <summary>
        /// 创建布局示例
        /// </summary>
        /// <param name="uiManager">UI管理器</param>
        /// <param name="graphicsDevice">图形设备</param>
        /// <param name="font">字体</param>
        public LayoutExample(UIManager uiManager, GraphicsDevice graphicsDevice, SpriteFont font)
        {
            _uiManager = uiManager;
            _graphicsDevice = graphicsDevice;
            _font = font;
            
            // 初始化纹理管理器
            TextureManager.Initialize(graphicsDevice);
            
            // 创建示例布局
            CreateExampleLayout();
        }
        
        /// <summary>
        /// 创建示例布局
        /// </summary>
        private void CreateExampleLayout()
        {
            // 创建一个垂直布局面板作为主面板
            var mainPanel = new VerticalLayout
            {
                Bounds = new Rectangle(10, 10, 300, 500),
                BackgroundColor = new Color(40, 40, 40),
                Spacing = 10,
                PaddingLeft = 10,
                PaddingRight = 10,
                PaddingTop = 10,
                PaddingBottom = 10
            };
            
            // 创建一个标签
            var titleLabel = new UILabel("布局示例", _font)
            {
                TextColor = Color.Yellow
            };
            
            // 创建一个水平布局面板作为工具栏
            var toolbar = new HorizontalLayout
            {
                Bounds = new Rectangle(0, 0, 280, 40),
                BackgroundColor = new Color(50, 50, 50),
                Spacing = 5,
                PaddingLeft = 5,
                PaddingRight = 5,
                PaddingTop = 5,
                PaddingBottom = 5
            };
            
            // 添加按钮到工具栏
            var button1 = new UIButton("按钮1", _font);
            var button2 = new UIButton("按钮2", _font);
            var button3 = new UIButton("按钮3", _font);
            
            toolbar.AddChild(button1);
            toolbar.AddChild(button2);
            toolbar.AddChild(button3);
            
            // 创建一个垂直布局面板作为内容区域
            var contentPanel = new VerticalLayout
            {
                Bounds = new Rectangle(0, 0, 280, 400),
                BackgroundColor = new Color(30, 30, 30),
                Spacing = 10,
                PaddingLeft = 10,
                PaddingRight = 10,
                PaddingTop = 10,
                PaddingBottom = 10
            };
            
            // 添加一些控件到内容区域
            var textBox1 = new UITextBox("姓名", "", _font);
            var textBox2 = new UITextBox("邮箱", "", _font);
            
            contentPanel.AddChild(textBox1);
            contentPanel.AddChild(textBox2);
            
            // 将标题、工具栏和内容区域添加到主面板
            mainPanel.AddChild(titleLabel);
            mainPanel.AddChild(toolbar);
            mainPanel.AddChild(contentPanel);
            
            // 将主面板添加到UI管理器
            _uiManager.AddElement(mainPanel);
        }
    }
}
