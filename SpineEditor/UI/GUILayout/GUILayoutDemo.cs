using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpineEditor.UI.UISystem;
using System;

namespace SpineEditor.UI.GUILayoutComponents
{
    /// <summary>
    /// 基于GUILayout系统的演示游戏
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
            UISystem.GUILayout.Initialize(_uiManager, _font);
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

            // 绘制GUI
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
            UISystem.GUILayout.BeginVertical();

            // 标题
            UISystem.GUILayout.Label("GUILayout演示", UISystem.GUILayout.Width(200));

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

            UISystem.GUILayout.EndVertical();
        }

        private void DrawTabs()
        {
            UISystem.GUILayout.BeginHorizontal();

            for (int i = 0; i < _tabs.Length; i++)
            {
                if (UISystem.GUILayout.Button(_tabs[i], UISystem.GUILayout.Width(100)))
                {
                    _selectedTab = i;
                }
            }

            UISystem.GUILayout.EndHorizontal();
        }

        private void DrawBasicControls()
        {
            UISystem.GUILayout.BeginVertical();

            UISystem.GUILayout.Label("基本控件示例");

            // 按钮
            UISystem.GUILayout.BeginHorizontal();
            if (UISystem.GUILayout.Button("按钮1", UISystem.GUILayout.Width(100)))
            {
                Console.WriteLine("按钮1被点击");
            }
            if (UISystem.GUILayout.Button("按钮2", UISystem.GUILayout.Width(100)))
            {
                Console.WriteLine("按钮2被点击");
            }
            UISystem.GUILayout.EndHorizontal();

            // 文本框
            UISystem.GUILayout.Label("文本框:");
            _name = UISystem.GUILayout.TextField(_name, UISystem.GUILayout.Width(200));

            UISystem.GUILayout.EndVertical();
        }

        private void DrawLayoutExamples()
        {
            UISystem.GUILayout.BeginVertical();

            UISystem.GUILayout.Label("布局示例");

            // 水平布局示例
            UISystem.GUILayout.Label("水平布局:");
            UISystem.GUILayout.BeginHorizontal();
            for (int i = 0; i < 5; i++)
            {
                if (UISystem.GUILayout.Button($"按钮{i+1}", UISystem.GUILayout.Width(80)))
                {
                    Console.WriteLine($"按钮{i+1}被点击");
                }
            }
            UISystem.GUILayout.EndHorizontal();

            // 嵌套布局示例
            UISystem.GUILayout.Label("嵌套布局:");
            UISystem.GUILayout.BeginHorizontal();

            // 左侧垂直布局
            UISystem.GUILayout.BeginVertical(UISystem.GUILayout.Width(200));
            UISystem.GUILayout.Label("左侧面板");
            for (int i = 0; i < 3; i++)
            {
                if (UISystem.GUILayout.Button($"选项{i+1}"))
                {
                    Console.WriteLine($"选项{i+1}被点击");
                }
            }
            UISystem.GUILayout.EndVertical();

            // 右侧垂直布局
            UISystem.GUILayout.BeginVertical();
            UISystem.GUILayout.Label("右侧面板");
            UISystem.GUILayout.Label("这是一个嵌套布局示例，展示了如何使用GUILayout创建复杂的UI布局。");
            UISystem.GUILayout.EndVertical();

            UISystem.GUILayout.EndHorizontal();

            UISystem.GUILayout.EndVertical();
        }

        private void DrawFormExample()
        {
            UISystem.GUILayout.BeginVertical();

            UISystem.GUILayout.Label("表单示例");

            // 姓名输入
            UISystem.GUILayout.Label("姓名:");
            _name = UISystem.GUILayout.TextField(_name, UISystem.GUILayout.Width(200));

            // 邮箱输入
            UISystem.GUILayout.Label("邮箱:");
            _email = UISystem.GUILayout.TextField(_email, UISystem.GUILayout.Width(200));

            // 同意条款
            UISystem.GUILayout.BeginHorizontal();
            if (UISystem.GUILayout.Button(_agreeTerms ? "✓" : "□", UISystem.GUILayout.Width(30)))
            {
                _agreeTerms = !_agreeTerms;
            }
            UISystem.GUILayout.Label("我同意服务条款");
            UISystem.GUILayout.EndHorizontal();

            // 提交按钮
            if (UISystem.GUILayout.Button("提交", UISystem.GUILayout.Width(100)))
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

            UISystem.GUILayout.EndVertical();
        }
    }
}
