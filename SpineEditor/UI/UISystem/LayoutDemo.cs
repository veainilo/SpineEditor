using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpineEditor.UI.UISystem
{
    /// <summary>
    /// 布局演示游戏，展示如何使用水平和垂直布局
    /// </summary>
    public class LayoutDemo : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private UIManager _uiManager;
        private SpriteFont _font;
        
        public LayoutDemo()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
            // 设置窗口大小
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 600;
            
            // 允许调整窗口大小
            Window.AllowUserResizing = true;
            
            // 订阅窗口大小变化事件
            Window.ClientSizeChanged += Window_ClientSizeChanged;
        }
        
        protected override void Initialize()
        {
            base.Initialize();
        }
        
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            // 加载字体
            _font = Content.Load<SpriteFont>("Font");
            
            // 创建UI管理器
            _uiManager = new UIManager();
            
            // 初始化纹理管理器
            TextureManager.Initialize(GraphicsDevice);
            
            // 创建布局示例
            CreateLayoutDemo();
        }
        
        private void CreateLayoutDemo()
        {
            // 创建主面板（垂直布局）
            var mainPanel = new VerticalLayout
            {
                Bounds = new Rectangle(10, 10, GraphicsDevice.Viewport.Width - 20, GraphicsDevice.Viewport.Height - 20),
                BackgroundColor = new Color(30, 30, 35),
                Spacing = 10,
                PaddingLeft = 10,
                PaddingRight = 10,
                PaddingTop = 10,
                PaddingBottom = 10
            };
            
            // 创建标题
            var titleLabel = new UILabel("布局系统演示", _font)
            {
                TextColor = Color.Yellow
            };
            
            // 创建顶部工具栏（水平布局）
            var toolbar = new HorizontalLayout
            {
                Bounds = new Rectangle(0, 0, GraphicsDevice.Viewport.Width - 40, 40),
                BackgroundColor = new Color(40, 40, 45),
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
            
            button1.Click += (sender, e) => { System.Console.WriteLine("按钮1被点击"); };
            button2.Click += (sender, e) => { System.Console.WriteLine("按钮2被点击"); };
            button3.Click += (sender, e) => { System.Console.WriteLine("按钮3被点击"); };
            
            toolbar.AddChild(button1);
            toolbar.AddChild(button2);
            toolbar.AddChild(button3);
            
            // 创建内容区域（水平布局）
            var contentArea = new HorizontalLayout
            {
                Bounds = new Rectangle(0, 0, GraphicsDevice.Viewport.Width - 40, GraphicsDevice.Viewport.Height - 100),
                BackgroundColor = new Color(35, 35, 40),
                Spacing = 10,
                PaddingLeft = 10,
                PaddingRight = 10,
                PaddingTop = 10,
                PaddingBottom = 10
            };
            
            // 创建左侧面板（垂直布局）
            var leftPanel = new VerticalLayout
            {
                Bounds = new Rectangle(0, 0, 200, GraphicsDevice.Viewport.Height - 120),
                BackgroundColor = new Color(45, 45, 50),
                Spacing = 10,
                PaddingLeft = 10,
                PaddingRight = 10,
                PaddingTop = 10,
                PaddingBottom = 10
            };
            
            // 添加控件到左侧面板
            var leftPanelTitle = new UILabel("左侧面板", _font);
            var textBox1 = new UITextBox("姓名", "", _font);
            var textBox2 = new UITextBox("邮箱", "", _font);
            var submitButton = new UIButton("提交", _font);
            
            leftPanel.AddChild(leftPanelTitle);
            leftPanel.AddChild(textBox1);
            leftPanel.AddChild(textBox2);
            leftPanel.AddChild(submitButton);
            
            // 创建右侧面板（垂直布局）
            var rightPanel = new VerticalLayout
            {
                Bounds = new Rectangle(0, 0, GraphicsDevice.Viewport.Width - 280, GraphicsDevice.Viewport.Height - 120),
                BackgroundColor = new Color(45, 45, 50),
                Spacing = 10,
                PaddingLeft = 10,
                PaddingRight = 10,
                PaddingTop = 10,
                PaddingBottom = 10
            };
            
            // 添加控件到右侧面板
            var rightPanelTitle = new UILabel("右侧面板", _font);
            
            // 创建一个水平布局的按钮组
            var buttonGroup = new HorizontalLayout
            {
                Bounds = new Rectangle(0, 0, GraphicsDevice.Viewport.Width - 300, 40),
                BackgroundColor = new Color(50, 50, 55),
                Spacing = 5,
                PaddingLeft = 5,
                PaddingRight = 5,
                PaddingTop = 5,
                PaddingBottom = 5
            };
            
            var groupButton1 = new UIButton("选项1", _font);
            var groupButton2 = new UIButton("选项2", _font);
            var groupButton3 = new UIButton("选项3", _font);
            
            buttonGroup.AddChild(groupButton1);
            buttonGroup.AddChild(groupButton2);
            buttonGroup.AddChild(groupButton3);
            
            rightPanel.AddChild(rightPanelTitle);
            rightPanel.AddChild(buttonGroup);
            
            // 添加左侧和右侧面板到内容区域
            contentArea.AddChild(leftPanel);
            contentArea.AddChild(rightPanel);
            
            // 添加标题、工具栏和内容区域到主面板
            mainPanel.AddChild(titleLabel);
            mainPanel.AddChild(toolbar);
            mainPanel.AddChild(contentArea);
            
            // 将主面板添加到UI管理器
            _uiManager.AddElement(mainPanel);
        }
        
        private void Window_ClientSizeChanged(object sender, System.EventArgs e)
        {
            // 窗口大小变化时更新UI布局
            UpdateLayout();
        }
        
        private void UpdateLayout()
        {
            // 这里可以更新布局，但由于我们的布局系统会自动调整，所以不需要额外的代码
        }
        
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            // 更新UI管理器
            _uiManager.Update(gameTime);
            
            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            // 绘制UI管理器
            _spriteBatch.Begin();
            _uiManager.Draw(_spriteBatch);
            _spriteBatch.End();
            
            base.Draw(gameTime);
        }
    }
}
