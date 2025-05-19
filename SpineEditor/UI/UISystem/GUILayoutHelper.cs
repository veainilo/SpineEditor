using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SpineEditor.UI.UISystem
{
    /// <summary>
    /// GUILayout辅助类，提供常用的GUI布局方法
    /// </summary>
    public static class GUILayoutHelper
    {
        /// <summary>
        /// 绘制标题
        /// </summary>
        /// <param name="title">标题文本</param>
        /// <param name="fontSize">字体大小（1=正常，2=大，3=特大）</param>
        public static void Title(string title, int fontSize = 1)
        {
            GUILayout.BeginHorizontal();
            
            switch (fontSize)
            {
                case 1:
                    GUILayout.Label(title);
                    break;
                case 2:
                    // 在这里我们没有字体大小控制，所以用其他方式表示
                    GUILayout.Label("== " + title + " ==");
                    break;
                case 3:
                    GUILayout.Label("=== " + title + " ===");
                    break;
                default:
                    GUILayout.Label(title);
                    break;
            }
            
            GUILayout.EndHorizontal();
        }
        
        /// <summary>
        /// 绘制分隔线
        /// </summary>
        public static void Separator()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("----------------------------------------");
            GUILayout.EndHorizontal();
        }
        
        /// <summary>
        /// 绘制带标签的文本框
        /// </summary>
        /// <param name="label">标签文本</param>
        /// <param name="value">文本值</param>
        /// <param name="labelWidth">标签宽度</param>
        /// <param name="fieldWidth">文本框宽度</param>
        /// <returns>文本框的值</returns>
        public static string LabelField(string label, string value, int labelWidth = 80, int fieldWidth = 150)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label + ":", GUILayout.Width(labelWidth));
            string result = GUILayout.TextField(value, GUILayout.Width(fieldWidth));
            GUILayout.EndHorizontal();
            return result;
        }
        
        /// <summary>
        /// 绘制带标签的整数文本框
        /// </summary>
        /// <param name="label">标签文本</param>
        /// <param name="value">整数值</param>
        /// <param name="labelWidth">标签宽度</param>
        /// <param name="fieldWidth">文本框宽度</param>
        /// <returns>整数值</returns>
        public static int IntField(string label, int value, int labelWidth = 80, int fieldWidth = 100)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label + ":", GUILayout.Width(labelWidth));
            string text = GUILayout.TextField(value.ToString(), GUILayout.Width(fieldWidth));
            int result = value;
            if (int.TryParse(text, out int parsedValue))
            {
                result = parsedValue;
            }
            GUILayout.EndHorizontal();
            return result;
        }
        
        /// <summary>
        /// 绘制带标签的浮点数文本框
        /// </summary>
        /// <param name="label">标签文本</param>
        /// <param name="value">浮点数值</param>
        /// <param name="format">格式化字符串</param>
        /// <param name="labelWidth">标签宽度</param>
        /// <param name="fieldWidth">文本框宽度</param>
        /// <returns>浮点数值</returns>
        public static float FloatField(string label, float value, string format = "F2", int labelWidth = 80, int fieldWidth = 100)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label + ":", GUILayout.Width(labelWidth));
            string text = GUILayout.TextField(value.ToString(format), GUILayout.Width(fieldWidth));
            float result = value;
            if (float.TryParse(text, out float parsedValue))
            {
                result = parsedValue;
            }
            GUILayout.EndHorizontal();
            return result;
        }
        
        /// <summary>
        /// 绘制带标签的布尔值复选框
        /// </summary>
        /// <param name="label">标签文本</param>
        /// <param name="value">布尔值</param>
        /// <param name="labelWidth">标签宽度</param>
        /// <returns>布尔值</returns>
        public static bool Toggle(string label, bool value, int labelWidth = 80)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(value ? "✓" : "□", GUILayout.Width(30)))
            {
                value = !value;
            }
            GUILayout.Label(label, GUILayout.Width(labelWidth));
            GUILayout.EndHorizontal();
            return value;
        }
        
        /// <summary>
        /// 绘制带标签的向量2文本框
        /// </summary>
        /// <param name="label">标签文本</param>
        /// <param name="value">向量2值</param>
        /// <param name="labelWidth">标签宽度</param>
        /// <param name="fieldWidth">文本框宽度</param>
        /// <returns>向量2值</returns>
        public static Vector2 Vector2Field(string label, Vector2 value, int labelWidth = 80, int fieldWidth = 50)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label + ":", GUILayout.Width(labelWidth));
            GUILayout.Label("X:", GUILayout.Width(20));
            string xText = GUILayout.TextField(value.X.ToString("F2"), GUILayout.Width(fieldWidth));
            GUILayout.Label("Y:", GUILayout.Width(20));
            string yText = GUILayout.TextField(value.Y.ToString("F2"), GUILayout.Width(fieldWidth));
            
            Vector2 result = value;
            if (float.TryParse(xText, out float x))
            {
                result.X = x;
            }
            if (float.TryParse(yText, out float y))
            {
                result.Y = y;
            }
            
            GUILayout.EndHorizontal();
            return result;
        }
        
        /// <summary>
        /// 绘制选项卡
        /// </summary>
        /// <param name="selectedTab">当前选中的选项卡索引</param>
        /// <param name="tabs">选项卡标题数组</param>
        /// <param name="tabWidth">选项卡宽度</param>
        /// <returns>选中的选项卡索引</returns>
        public static int Tabs(int selectedTab, string[] tabs, int tabWidth = 100)
        {
            GUILayout.BeginHorizontal();
            
            for (int i = 0; i < tabs.Length; i++)
            {
                if (GUILayout.Button(tabs[i], GUILayout.Width(tabWidth)))
                {
                    selectedTab = i;
                }
            }
            
            GUILayout.EndHorizontal();
            return selectedTab;
        }
    }
}
