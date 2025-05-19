using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SpineEditor.UI.UISystem
{
    /// <summary>
    /// GUILayout示例面板，展示如何使用GUILayoutPanel基类
    /// </summary>
    public class GUILayoutExamplePanel : GUILayoutPanel
    {
        // 示例状态变量
        private string _name = "";
        private string _email = "";
        private bool _agreeTerms = false;
        private int _selectedTab = 0;
        private string[] _tabs = { "基本控件", "布局示例", "表单示例" };
        
        /// <summary>
        /// 创建GUILayout示例面板
        /// </summary>
        /// <param name="title">面板标题</param>
        /// <param name="bounds">面板边界</param>
        /// <param name="graphicsDevice">图形设备</param>
        /// <param name="font">字体</param>
        public GUILayoutExamplePanel(string title, Rectangle bounds, GraphicsDevice graphicsDevice, SpriteFont font)
            : base(title, bounds, graphicsDevice, font)
        {
        }
        
        /// <summary>
        /// 绘制GUI内容
        /// </summary>
        protected override void DrawGUI()
        {
            // 主面板
            GUILayout.BeginVertical();
            
            // 选项卡
            _selectedTab = GUILayoutHelper.Tabs(_selectedTab, _tabs);
            
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
        
        /// <summary>
        /// 绘制基本控件
        /// </summary>
        private void DrawBasicControls()
        {
            GUILayout.BeginVertical();
            
            GUILayoutHelper.Title("基本控件示例", 2);
            GUILayoutHelper.Separator();
            
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
            _name = GUILayoutHelper.LabelField("姓名", _name);
            
            // 整数框
            int age = 25;
            age = GUILayoutHelper.IntField("年龄", age);
            
            // 浮点数框
            float height = 1.75f;
            height = GUILayoutHelper.FloatField("身高", height, "F2");
            
            // 复选框
            _agreeTerms = GUILayoutHelper.Toggle("同意条款", _agreeTerms);
            
            // 向量2
            Vector2 position = new Vector2(100, 200);
            position = GUILayoutHelper.Vector2Field("位置", position);
            
            GUILayout.EndVertical();
        }
        
        /// <summary>
        /// 绘制布局示例
        /// </summary>
        private void DrawLayoutExamples()
        {
            GUILayout.BeginVertical();
            
            GUILayoutHelper.Title("布局示例", 2);
            GUILayoutHelper.Separator();
            
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
        
        /// <summary>
        /// 绘制表单示例
        /// </summary>
        private void DrawFormExample()
        {
            GUILayout.BeginVertical();
            
            GUILayoutHelper.Title("表单示例", 2);
            GUILayoutHelper.Separator();
            
            // 姓名输入
            _name = GUILayoutHelper.LabelField("姓名", _name);
            
            // 邮箱输入
            _email = GUILayoutHelper.LabelField("邮箱", _email);
            
            // 同意条款
            _agreeTerms = GUILayoutHelper.Toggle("我同意服务条款", _agreeTerms);
            
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
