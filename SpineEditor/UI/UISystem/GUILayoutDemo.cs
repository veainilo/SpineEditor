using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace SpineEditor.UI.UISystem
{
    /// <summary>
    /// GUILayout演示游戏，展示如何使用类似Unity的GUILayout系统
    /// </summary>
    public class GUILayoutDemo : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private UIManager _uiManager;
        private SpriteFont _font;

        // 演示用的状态变量
        private string _name = "";
        private string _email = "";
        private bool _agreeTerms = false;
        private int _selectedTab = 0;
        private string[] _tabs = { "基本控件", "布局示例", "表单示例" };

        public GUILayoutDemo()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // 设置窗口大小
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 600;

            // 允许调整窗口大小
            Window.AllowUserResizing = true;
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
            _uiManager = new UIManager(GraphicsDevice);

            // 初始化GUILayout系统
            GUILayout.Initialize(_uiManager, _font);
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

            // 绘制GUI - 不需要手动调用BeginFrame/EndFrame
            DrawGUI();

            // 绘制UI管理器
            _spriteBatch.Begin();
            _uiManager.Draw(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawGUI()
        {
            // 主面板
            GUILayout.BeginVertical();

            // 标题
            GUILayout.Label("GUILayout演示", GUILayout.Width(200));

            // 选项卡
            DrawTabs();

            // 根据选中的选项卡绘制不同的内容
            switch (_selectedTab)
            {
                case 0:
                    DrawBasicControls();
                    break;
                case 1:
                    DrawLayoutExamples();
                    break;
                case 2:
                    DrawFormExample();
                    break;
            }

            GUILayout.EndVertical();
        }

        private void DrawTabs()
        {
            GUILayout.BeginHorizontal();

            for (int i = 0; i < _tabs.Length; i++)
            {
                if (GUILayout.Button(_tabs[i], GUILayout.Width(100)))
                {
                    _selectedTab = i;
                }
            }

            GUILayout.EndHorizontal();
        }

        private void DrawBasicControls()
        {
            GUILayout.BeginVertical();

            GUILayout.Label("基本控件示例");

            // 按钮
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("按钮1", GUILayout.Width(100)))
            {
                Console.WriteLine("按钮1被点击");
            }
            if (GUILayout.Button("按钮2", GUILayout.Width(100)))
            {
                Console.WriteLine("按钮2被点击");
            }
            GUILayout.EndHorizontal();

            // 文本框
            GUILayout.Label("文本框:");
            _name = GUILayout.TextField(_name, GUILayout.Width(200));

            GUILayout.EndVertical();
        }

        private void DrawLayoutExamples()
        {
            GUILayout.BeginVertical();

            GUILayout.Label("布局示例");

            // 水平布局示例
            GUILayout.Label("水平布局:");
            GUILayout.BeginHorizontal();
            for (int i = 0; i < 5; i++)
            {
                if (GUILayout.Button($"按钮{i+1}", GUILayout.Width(80)))
                {
                    Console.WriteLine($"按钮{i+1}被点击");
                }
            }
            GUILayout.EndHorizontal();

            // 嵌套布局示例
            GUILayout.Label("嵌套布局:");
            GUILayout.BeginHorizontal();

            // 左侧垂直布局
            GUILayout.BeginVertical(GUILayout.Width(200));
            GUILayout.Label("左侧面板");
            for (int i = 0; i < 3; i++)
            {
                if (GUILayout.Button($"选项{i+1}"))
                {
                    Console.WriteLine($"选项{i+1}被点击");
                }
            }
            GUILayout.EndVertical();

            // 右侧垂直布局
            GUILayout.BeginVertical();
            GUILayout.Label("右侧面板");
            GUILayout.Label("这是一个嵌套布局示例，展示了如何使用GUILayout创建复杂的UI布局。");
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        private void DrawFormExample()
        {
            GUILayout.BeginVertical();

            GUILayout.Label("表单示例");

            // 姓名输入
            GUILayout.Label("姓名:");
            _name = GUILayout.TextField(_name, GUILayout.Width(200));

            // 邮箱输入
            GUILayout.Label("邮箱:");
            _email = GUILayout.TextField(_email, GUILayout.Width(200));

            // 同意条款
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(_agreeTerms ? "✓" : "□", GUILayout.Width(30)))
            {
                _agreeTerms = !_agreeTerms;
            }
            GUILayout.Label("我同意服务条款");
            GUILayout.EndHorizontal();

            // 提交按钮
            if (GUILayout.Button("提交", GUILayout.Width(100)))
            {
                if (string.IsNullOrEmpty(_name))
                {
                    Console.WriteLine("请输入姓名");
                }
                else if (string.IsNullOrEmpty(_email))
                {
                    Console.WriteLine("请输入邮箱");
                }
                else if (!_agreeTerms)
                {
                    Console.WriteLine("请同意服务条款");
                }
                else
                {
                    Console.WriteLine($"表单提交成功: 姓名={_name}, 邮箱={_email}");
                }
            }

            GUILayout.EndVertical();
        }
    }
}
