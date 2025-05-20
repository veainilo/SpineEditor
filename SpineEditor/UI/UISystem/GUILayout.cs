using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpineEditor.UI.UISystem
{
    /// <summary>
    /// 类似Unity的GUILayout系统，提供自动布局功能
    /// </summary>
    public static class GUILayout
    {
        // 布局堆栈，用于跟踪当前的布局容器
        private static Stack<LayoutPanel> _layoutStack = new Stack<LayoutPanel>();

        // 当前的UI管理器
        private static UIManager _uiManager;

        // 当前的字体
        private static SpriteFont _font;

        // 当前帧创建的控件列表，用于事件处理
        private static Dictionary<UIButton, bool> _buttonStates = new Dictionary<UIButton, bool>();
        private static Dictionary<UITextBox, string> _textBoxStates = new Dictionary<UITextBox, string>();

        // 布局选项
        public class Options
        {
            public int? Width { get; set; }
            public int? Height { get; set; }
            public bool ExpandWidth { get; set; } = false;
            public bool ExpandHeight { get; set; } = false;

            public Options() { }

            public Options(int width)
            {
                Width = width;
            }

            public Options(int width, int height)
            {
                Width = width;
                Height = height;
            }
        }

        /// <summary>
        /// 初始化GUILayout系统
        /// </summary>
        /// <param name="uiManager">UI管理器</param>
        /// <param name="font">字体</param>
        /// <param name="autoBeginEnd">是否自动处理BeginFrame/EndFrame</param>
        public static void Initialize(UIManager uiManager, SpriteFont font, bool autoBeginEnd = true)
        {
            _uiManager = uiManager;
            _font = font;
            _autoBeginEnd = autoBeginEnd;
            _initialized = true;
            TextureManager.Initialize(uiManager.GraphicsDevice);
        }

        // 内部状态
        private static bool _initialized = false;
        private static bool _autoBeginEnd = false;
        private static bool _inGUI = false;

        /// <summary>
        /// 开始一个新的GUI帧 - 通常不需要手动调用
        /// </summary>
        public static void BeginFrame()
        {
            if (_inGUI) return;
            _inGUI = true;

            // 清空布局堆栈
            _layoutStack.Clear();

            // 清空控件状态
            _buttonStates.Clear();
            _textBoxStates.Clear();
        }

        /// <summary>
        /// 结束当前GUI帧 - 通常不需要手动调用
        /// </summary>
        public static void EndFrame()
        {
            if (!_inGUI) return;

            // 确保所有布局都已结束
            if (_layoutStack.Count > 0)
            {
                System.Console.WriteLine("警告：有未结束的布局");
                _layoutStack.Clear();
            }

            _inGUI = false;
        }

        /// <summary>
        /// 开始水平布局
        /// </summary>
        /// <param name="options">布局选项</param>
        public static void BeginHorizontal(params Options[] options)
        {
            // 自动开始GUI帧
            if (_autoBeginEnd && !_inGUI)
            {
                BeginFrame();
            }

            // 检查是否已初始化
            if (!_initialized)
            {
                Console.WriteLine("错误：GUILayout未初始化，请先调用GUILayout.Initialize");
                return;
            }

            var layout = new HorizontalLayout
            {
                BackgroundColor = new Color(50, 50, 50),
                Spacing = 5,
                PaddingLeft = 5,
                PaddingRight = 5,
                PaddingTop = 5,
                PaddingBottom = 5,
                AutoSize = true
            };

            // 应用布局选项
            ApplyOptions(layout, options);

            if (_layoutStack.Count > 0)
            {
                // 如果有父布局，添加到父布局中
                _layoutStack.Peek().AddChild(layout);
            }
            else
            {
                // 如果没有父布局，添加到UI管理器中
                _uiManager.AddElement(layout);
            }

            // 将当前布局压入堆栈
            _layoutStack.Push(layout);
        }

        /// <summary>
        /// 结束水平布局
        /// </summary>
        public static void EndHorizontal()
        {
            if (_layoutStack.Count > 0 && _layoutStack.Peek() is HorizontalLayout)
            {
                _layoutStack.Pop();

                // 如果布局堆栈为空且启用了自动结束，则结束GUI帧
                if (_autoBeginEnd && _layoutStack.Count == 0)
                {
                    EndFrame();
                }
            }
            else
            {
                Console.WriteLine("错误：没有匹配的BeginHorizontal");
            }
        }

        /// <summary>
        /// 开始垂直布局
        /// </summary>
        /// <param name="options">布局选项</param>
        public static void BeginVertical(params Options[] options)
        {
            // 自动开始GUI帧
            if (_autoBeginEnd && !_inGUI)
            {
                BeginFrame();
            }

            // 检查是否已初始化
            if (!_initialized)
            {
                Console.WriteLine("错误：GUILayout未初始化，请先调用GUILayout.Initialize");
                return;
            }

            var layout = new VerticalLayout
            {
                BackgroundColor = new Color(40, 40, 40),
                Spacing = 5,  // 减小间距
                PaddingLeft = 10,
                PaddingRight = 10,
                PaddingTop = 5,  // 减小上边距
                PaddingBottom = 5,  // 减小下边距
                AutoSize = true
            };

            // 应用布局选项
            ApplyOptions(layout, options);

            if (_layoutStack.Count > 0)
            {
                // 如果有父布局，添加到父布局中
                _layoutStack.Peek().AddChild(layout);
            }
            else
            {
                // 如果没有父布局，添加到UI管理器中
                _uiManager.AddElement(layout);
            }

            // 将当前布局压入堆栈
            _layoutStack.Push(layout);
        }

        /// <summary>
        /// 结束垂直布局
        /// </summary>
        public static void EndVertical()
        {
            if (_layoutStack.Count > 0 && _layoutStack.Peek() is VerticalLayout)
            {
                _layoutStack.Pop();

                // 如果布局堆栈为空且启用了自动结束，则结束GUI帧
                if (_autoBeginEnd && _layoutStack.Count == 0)
                {
                    EndFrame();
                }
            }
            else
            {
                Console.WriteLine("错误：没有匹配的BeginVertical");
            }
        }

        /// <summary>
        /// 添加标签
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="options">布局选项</param>
        public static void Label(string text, params Options[] options)
        {
            // 自动开始GUI帧
            if (_autoBeginEnd && !_inGUI)
            {
                BeginFrame();
            }

            // 检查是否已初始化
            if (!_initialized)
            {
                Console.WriteLine("错误：GUILayout未初始化，请先调用GUILayout.Initialize");
                return;
            }

            // 确保文本不为null
            text = text ?? string.Empty;

            // 创建标签
            var label = new UILabel(text, _font);

            // 应用布局选项
            ApplyOptions(label, options);

            if (_layoutStack.Count > 0)
            {
                _layoutStack.Peek().AddChild(label);
            }
            else
            {
                _uiManager.AddElement(label);
            }

            // 如果没有活动布局且启用了自动结束，则结束GUI帧
            if (_autoBeginEnd && _layoutStack.Count == 0)
            {
                EndFrame();
            }
        }

        /// <summary>
        /// 添加按钮
        /// </summary>
        /// <param name="text">按钮文本</param>
        /// <param name="options">布局选项</param>
        /// <returns>按钮是否被点击</returns>
        public static bool Button(string text, params Options[] options)
        {
            // 自动开始GUI帧
            if (_autoBeginEnd && !_inGUI)
            {
                BeginFrame();
            }

            // 检查是否已初始化
            if (!_initialized)
            {
                Console.WriteLine("错误：GUILayout未初始化，请先调用GUILayout.Initialize");
                return false;
            }

            // 确保文本不为null
            text = text ?? string.Empty;

            // 创建按钮
            var button = new UIButton(text, _font);

            // 应用布局选项
            ApplyOptions(button, options);

            // 跟踪按钮状态
            _buttonStates[button] = false;

            button.Click += (sender, e) => {
                _buttonStates[button] = true;
            };

            if (_layoutStack.Count > 0)
            {
                _layoutStack.Peek().AddChild(button);
            }
            else
            {
                _uiManager.AddElement(button);
            }

            // 如果没有活动布局且启用了自动结束，则结束GUI帧
            if (_autoBeginEnd && _layoutStack.Count == 0)
            {
                EndFrame();
            }

            return _buttonStates[button];
        }

        /// <summary>
        /// 添加文本框
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="options">布局选项</param>
        /// <returns>文本框的文本</returns>
        public static string TextField(string text, params Options[] options)
        {
            // 自动开始GUI帧
            if (_autoBeginEnd && !_inGUI)
            {
                BeginFrame();
            }

            // 检查是否已初始化
            if (!_initialized)
            {
                Console.WriteLine("错误：GUILayout未初始化，请先调用GUILayout.Initialize");
                return text;
            }

            // 确保文本不为null
            text = text ?? string.Empty;

            // 创建文本框
            var textBox = new UITextBox("", text, _font);

            // 应用布局选项
            ApplyOptions(textBox, options);

            // 跟踪文本框状态
            _textBoxStates[textBox] = text;

            textBox.TextChanged += (sender, e) => {
                _textBoxStates[textBox] = textBox.Text;
            };

            if (_layoutStack.Count > 0)
            {
                _layoutStack.Peek().AddChild(textBox);
            }
            else
            {
                _uiManager.AddElement(textBox);
            }

            // 如果没有活动布局且启用了自动结束，则结束GUI帧
            if (_autoBeginEnd && _layoutStack.Count == 0)
            {
                EndFrame();
            }

            return _textBoxStates[textBox];
        }

        /// <summary>
        /// 应用布局选项
        /// </summary>
        /// <param name="element">UI元素</param>
        /// <param name="options">布局选项</param>
        private static void ApplyOptions(UIElement element, Options[] options)
        {
            if (options == null || options.Length == 0)
                return;

            foreach (var option in options)
            {
                if (option.Width.HasValue)
                {
                    element.Bounds = new Rectangle(
                        element.Bounds.X,
                        element.Bounds.Y,
                        option.Width.Value,
                        element.Bounds.Height
                    );
                }

                if (option.Height.HasValue)
                {
                    element.Bounds = new Rectangle(
                        element.Bounds.X,
                        element.Bounds.Y,
                        element.Bounds.Width,
                        option.Height.Value
                    );
                }

                // 其他选项可以在这里添加
            }
        }

        /// <summary>
        /// 创建宽度选项
        /// </summary>
        /// <param name="width">宽度</param>
        /// <returns>布局选项</returns>
        public static Options Width(int width)
        {
            return new Options { Width = width };
        }

        /// <summary>
        /// 创建高度选项
        /// </summary>
        /// <param name="height">高度</param>
        /// <returns>布局选项</returns>
        public static Options Height(int height)
        {
            return new Options { Height = height };
        }

        /// <summary>
        /// 创建宽度和高度选项
        /// </summary>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <returns>布局选项</returns>
        public static Options Size(int width, int height)
        {
            return new Options { Width = width, Height = height };
        }

        /// <summary>
        /// 创建扩展宽度选项
        /// </summary>
        /// <returns>布局选项</returns>
        public static Options ExpandWidth()
        {
            return new Options { ExpandWidth = true };
        }

        /// <summary>
        /// 创建扩展高度选项
        /// </summary>
        /// <returns>布局选项</returns>
        public static Options ExpandHeight()
        {
            return new Options { ExpandHeight = true };
        }
    }
}
