using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SpineEditor.UI.UISystem
{
    /// <summary>
    /// GUILayout使用示例，展示如何在现有项目中集成GUILayout系统
    /// </summary>
    public class GUILayoutUsageExample
    {
        private UIManager _uiManager;
        private SpriteFont _font;
        private GraphicsDevice _graphicsDevice;

        // 示例状态变量
        private string _animationName = "";
        private float _animationSpeed = 1.0f;
        private bool _loopAnimation = true;
        private string _eventName = "";
        private float _eventTime = 0.0f;
        private int _selectedEventType = 0;
        private string[] _eventTypes = { "普通", "攻击", "特效", "声音" };

        /// <summary>
        /// 创建GUILayout使用示例
        /// </summary>
        /// <param name="graphicsDevice">图形设备</param>
        /// <param name="font">字体</param>
        public GUILayoutUsageExample(GraphicsDevice graphicsDevice, SpriteFont font)
        {
            _graphicsDevice = graphicsDevice;
            _font = font;

            // 创建UI管理器
            _uiManager = new UIManager(graphicsDevice);

            // 初始化GUILayout系统
            GUILayout.Initialize(_uiManager, font);
        }

        /// <summary>
        /// 更新示例
        /// </summary>
        /// <param name="gameTime">游戏时间</param>
        public void Update(GameTime gameTime)
        {
            // 更新UI管理器
            _uiManager.Update(gameTime);
        }

        /// <summary>
        /// 绘制示例
        /// </summary>
        /// <param name="spriteBatch">精灵批处理</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            // 绘制GUI - 不需要手动调用BeginFrame/EndFrame
            DrawGUI();

            // 绘制UI管理器
            _uiManager.Draw(spriteBatch);
        }

        /// <summary>
        /// 绘制GUI
        /// </summary>
        private void DrawGUI()
        {
            // 主面板
            GUILayout.BeginVertical();

            // 标题
            GUILayout.Label("Spine动画编辑器");

            // 动画设置
            DrawAnimationSettings();

            // 事件编辑
            DrawEventEditor();

            GUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制动画设置
        /// </summary>
        private void DrawAnimationSettings()
        {
            GUILayout.BeginVertical();

            GUILayout.Label("动画设置");

            // 动画名称
            GUILayout.BeginHorizontal();
            GUILayout.Label("动画名称:", GUILayout.Width(80));
            _animationName = GUILayout.TextField(_animationName, GUILayout.Width(150));
            GUILayout.EndHorizontal();

            // 动画速度
            GUILayout.BeginHorizontal();
            GUILayout.Label("播放速度:", GUILayout.Width(80));
            string speedText = GUILayout.TextField(_animationSpeed.ToString("F1"), GUILayout.Width(50));
            if (float.TryParse(speedText, out float speed))
            {
                _animationSpeed = speed;
            }
            GUILayout.EndHorizontal();

            // 循环播放
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(_loopAnimation ? "✓" : "□", GUILayout.Width(30)))
            {
                _loopAnimation = !_loopAnimation;
            }
            GUILayout.Label("循环播放");
            GUILayout.EndHorizontal();

            // 控制按钮
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("播放", GUILayout.Width(80)))
            {
                Console.WriteLine("播放动画");
            }
            if (GUILayout.Button("暂停", GUILayout.Width(80)))
            {
                Console.WriteLine("暂停动画");
            }
            if (GUILayout.Button("重置", GUILayout.Width(80)))
            {
                Console.WriteLine("重置动画");
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制事件编辑器
        /// </summary>
        private void DrawEventEditor()
        {
            GUILayout.BeginVertical();

            GUILayout.Label("事件编辑");

            // 事件名称
            GUILayout.BeginHorizontal();
            GUILayout.Label("事件名称:", GUILayout.Width(80));
            _eventName = GUILayout.TextField(_eventName, GUILayout.Width(150));
            GUILayout.EndHorizontal();

            // 事件时间
            GUILayout.BeginHorizontal();
            GUILayout.Label("事件时间:", GUILayout.Width(80));
            string timeText = GUILayout.TextField(_eventTime.ToString("F2"), GUILayout.Width(50));
            if (float.TryParse(timeText, out float time))
            {
                _eventTime = time;
            }
            GUILayout.EndHorizontal();

            // 事件类型
            GUILayout.BeginHorizontal();
            GUILayout.Label("事件类型:", GUILayout.Width(80));
            for (int i = 0; i < _eventTypes.Length; i++)
            {
                if (GUILayout.Button(_eventTypes[i], GUILayout.Width(60)))
                {
                    _selectedEventType = i;
                }
            }
            GUILayout.EndHorizontal();

            // 根据选中的事件类型显示不同的属性
            switch (_selectedEventType)
            {
                case 0: // 普通
                    DrawNormalEventProperties();
                    break;
                case 1: // 攻击
                    DrawAttackEventProperties();
                    break;
                case 2: // 特效
                    DrawEffectEventProperties();
                    break;
                case 3: // 声音
                    DrawSoundEventProperties();
                    break;
            }

            // 添加/删除按钮
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("添加事件", GUILayout.Width(100)))
            {
                Console.WriteLine("添加事件");
            }
            if (GUILayout.Button("删除事件", GUILayout.Width(100)))
            {
                Console.WriteLine("删除事件");
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制普通事件属性
        /// </summary>
        private void DrawNormalEventProperties()
        {
            GUILayout.BeginVertical();

            GUILayout.Label("普通事件属性");

            // 整数值
            GUILayout.BeginHorizontal();
            GUILayout.Label("整数值:", GUILayout.Width(80));
            GUILayout.TextField("0", GUILayout.Width(100));
            GUILayout.EndHorizontal();

            // 浮点值
            GUILayout.BeginHorizontal();
            GUILayout.Label("浮点值:", GUILayout.Width(80));
            GUILayout.TextField("0.0", GUILayout.Width(100));
            GUILayout.EndHorizontal();

            // 字符串值
            GUILayout.BeginHorizontal();
            GUILayout.Label("字符串值:", GUILayout.Width(80));
            GUILayout.TextField("", GUILayout.Width(150));
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制攻击事件属性
        /// </summary>
        private void DrawAttackEventProperties()
        {
            GUILayout.BeginVertical();

            GUILayout.Label("攻击事件属性");

            // 攻击类型
            GUILayout.BeginHorizontal();
            GUILayout.Label("攻击类型:", GUILayout.Width(80));
            GUILayout.TextField("", GUILayout.Width(100));
            GUILayout.EndHorizontal();

            // 伤害值
            GUILayout.BeginHorizontal();
            GUILayout.Label("伤害值:", GUILayout.Width(80));
            GUILayout.TextField("0", GUILayout.Width(100));
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制特效事件属性
        /// </summary>
        private void DrawEffectEventProperties()
        {
            GUILayout.BeginVertical();

            GUILayout.Label("特效事件属性");

            // 特效名称
            GUILayout.BeginHorizontal();
            GUILayout.Label("特效名称:", GUILayout.Width(80));
            GUILayout.TextField("", GUILayout.Width(150));
            GUILayout.EndHorizontal();

            // 位置
            GUILayout.BeginHorizontal();
            GUILayout.Label("位置:", GUILayout.Width(80));
            GUILayout.Label("X:", GUILayout.Width(20));
            GUILayout.TextField("0", GUILayout.Width(50));
            GUILayout.Label("Y:", GUILayout.Width(20));
            GUILayout.TextField("0", GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制声音事件属性
        /// </summary>
        private void DrawSoundEventProperties()
        {
            GUILayout.BeginVertical();

            GUILayout.Label("声音事件属性");

            // 声音名称
            GUILayout.BeginHorizontal();
            GUILayout.Label("声音名称:", GUILayout.Width(80));
            GUILayout.TextField("", GUILayout.Width(150));
            GUILayout.EndHorizontal();

            // 音量
            GUILayout.BeginHorizontal();
            GUILayout.Label("音量:", GUILayout.Width(80));
            GUILayout.TextField("1.0", GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }
    }
}
