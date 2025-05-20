using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpineEditor.UI.UISystem;
using System;
using System.Collections.Generic;

namespace SpineEditor.UI.GUILayoutComponents
{
    /// <summary>
    /// 基于GUILayout系统的左侧面板
    /// </summary>
    public class LeftPanelGUI : GUILayoutPanel
    {
        // 状态变量
        private string _currentTime = "0.000";
        private string _totalTime = "0.000";
        private string _currentAnimation = "";
        private int _eventCount = 0;
        private float _scale = 1.0f;
        private bool _isPlaying = false;
        private string _speedText = "1.0";
        private List<string> _animations = new List<string>();
        private int _selectedAnimationIndex = -1;

        // 事件
        public event EventHandler PlayPauseClicked;
        public event EventHandler ResetClicked;
        public event EventHandler SaveClicked;
        public event EventHandler<string> SpeedChanged;
        public event EventHandler<string> AnimationSelected;

        /// <summary>
        /// 创建左侧面板
        /// </summary>
        /// <param name="title">面板标题</param>
        /// <param name="bounds">面板边界</param>
        /// <param name="graphicsDevice">图形设备</param>
        /// <param name="font">字体</param>
        public LeftPanelGUI(string title, Rectangle bounds, GraphicsDevice graphicsDevice, SpriteFont font)
            : base(title, bounds, graphicsDevice, font)
        {
        }

        /// <summary>
        /// 设置播放/暂停按钮文本
        /// </summary>
        /// <param name="isPlaying">是否正在播放</param>
        public void SetPlayPauseButtonText(bool isPlaying)
        {
            _isPlaying = isPlaying;
        }

        /// <summary>
        /// 更新信息
        /// </summary>
        /// <param name="currentTime">当前时间</param>
        /// <param name="totalTime">总时间</param>
        /// <param name="currentAnimation">当前动画</param>
        /// <param name="eventCount">事件数量</param>
        /// <param name="scale">缩放比例</param>
        public void UpdateInfo(float currentTime, float totalTime, string currentAnimation, int eventCount, float scale)
        {
            _currentTime = currentTime.ToString("F3");
            _totalTime = totalTime.ToString("F3");
            _currentAnimation = currentAnimation;
            _eventCount = eventCount;
            _scale = scale;
        }

        /// <summary>
        /// 设置动画列表
        /// </summary>
        /// <param name="animations">动画列表</param>
        public void SetAnimations(IEnumerable<string> animations)
        {
            _animations = new List<string>(animations);
            if (_animations.Count > 0 && _selectedAnimationIndex < 0)
            {
                _selectedAnimationIndex = 0;
            }
        }

        /// <summary>
        /// 绘制GUI内容
        /// </summary>
        protected override void DrawGUI()
        {
            // 主面板
            UISystem.GUILayout.BeginVertical();

            // 信息区域
            DrawInfoSection();

            // 控制区域
            DrawControlSection();

            // 动画列表区域
            DrawAnimationSection();

            UISystem.GUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制信息区域
        /// </summary>
        private void DrawInfoSection()
        {
            UISystem.GUILayout.BeginVertical();

            // 标题
            UISystem.GUILayoutHelper.Title("信息", 2);
            UISystem.GUILayoutHelper.Separator();

            // 添加一些空间
            UISystem.GUILayout.Label("", UISystem.GUILayout.Height(5));

            // 当前时间
            UISystem.GUILayout.BeginHorizontal();
            UISystem.GUILayout.Label("当前时间:", UISystem.GUILayout.Width(80));
            UISystem.GUILayout.Label(_currentTime, UISystem.GUILayout.Width(80));
            UISystem.GUILayout.EndHorizontal();

            // 添加一些空间
            UISystem.GUILayout.Label("", UISystem.GUILayout.Height(5));

            // 总时间
            UISystem.GUILayout.BeginHorizontal();
            UISystem.GUILayout.Label("总时间:", UISystem.GUILayout.Width(80));
            UISystem.GUILayout.Label(_totalTime, UISystem.GUILayout.Width(80));
            UISystem.GUILayout.EndHorizontal();

            // 添加一些空间
            UISystem.GUILayout.Label("", UISystem.GUILayout.Height(5));

            // 当前动画
            UISystem.GUILayout.BeginHorizontal();
            UISystem.GUILayout.Label("当前动画:", UISystem.GUILayout.Width(80));
            UISystem.GUILayout.Label(_currentAnimation, UISystem.GUILayout.Width(180));
            UISystem.GUILayout.EndHorizontal();

            // 添加一些空间
            UISystem.GUILayout.Label("", UISystem.GUILayout.Height(5));

            // 事件数量
            UISystem.GUILayout.BeginHorizontal();
            UISystem.GUILayout.Label("事件数量:", UISystem.GUILayout.Width(80));
            UISystem.GUILayout.Label(_eventCount.ToString(), UISystem.GUILayout.Width(80));
            UISystem.GUILayout.EndHorizontal();

            // 添加一些空间
            UISystem.GUILayout.Label("", UISystem.GUILayout.Height(5));

            // 缩放比例
            UISystem.GUILayout.BeginHorizontal();
            UISystem.GUILayout.Label("缩放比例:", UISystem.GUILayout.Width(80));
            UISystem.GUILayout.Label(_scale.ToString("F2"), UISystem.GUILayout.Width(80));
            UISystem.GUILayout.EndHorizontal();

            // 添加一些空间
            UISystem.GUILayout.Label("", UISystem.GUILayout.Height(10));

            UISystem.GUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制控制区域
        /// </summary>
        private void DrawControlSection()
        {
            UISystem.GUILayout.BeginVertical();

            // 标题
            UISystem.GUILayoutHelper.Title("控制", 2);
            UISystem.GUILayoutHelper.Separator();

            // 添加一些空间
            UISystem.GUILayout.Label("", UISystem.GUILayout.Height(5));

            // 播放/暂停按钮
            UISystem.GUILayout.BeginHorizontal();
            if (UISystem.GUILayout.Button(_isPlaying ? "暂停" : "播放", UISystem.GUILayout.Width(80)))
            {
                PlayPauseClicked?.Invoke(this, EventArgs.Empty);
            }

            // 重置按钮
            if (UISystem.GUILayout.Button("重置", UISystem.GUILayout.Width(80)))
            {
                ResetClicked?.Invoke(this, EventArgs.Empty);
            }

            // 保存按钮
            if (UISystem.GUILayout.Button("保存", UISystem.GUILayout.Width(80)))
            {
                SaveClicked?.Invoke(this, EventArgs.Empty);
            }
            UISystem.GUILayout.EndHorizontal();

            // 添加一些空间
            UISystem.GUILayout.Label("", UISystem.GUILayout.Height(5));

            // 速度控制
            UISystem.GUILayout.BeginHorizontal();
            UISystem.GUILayout.Label("播放速度:", UISystem.GUILayout.Width(80));
            string newSpeedText = UISystem.GUILayout.TextField(_speedText, UISystem.GUILayout.Width(80));
            if (newSpeedText != _speedText)
            {
                _speedText = newSpeedText;
                SpeedChanged?.Invoke(this, _speedText);
            }
            UISystem.GUILayout.EndHorizontal();

            // 添加一些空间
            UISystem.GUILayout.Label("", UISystem.GUILayout.Height(10));

            UISystem.GUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制动画列表区域
        /// </summary>
        private void DrawAnimationSection()
        {
            UISystem.GUILayout.BeginVertical();

            // 标题
            UISystem.GUILayoutHelper.Title("动画列表", 2);
            UISystem.GUILayoutHelper.Separator();

            // 添加一些空间
            UISystem.GUILayout.Label("", UISystem.GUILayout.Height(5));

            // 动画列表
            for (int i = 0; i < _animations.Count; i++)
            {
                bool isSelected = i == _selectedAnimationIndex;
                string buttonText = isSelected ? $"▶ {_animations[i]}" : $"   {_animations[i]}";

                if (UISystem.GUILayout.Button(buttonText, UISystem.GUILayout.Width(250)))
                {
                    if (_selectedAnimationIndex != i)
                    {
                        _selectedAnimationIndex = i;
                        AnimationSelected?.Invoke(this, _animations[i]);
                    }
                }

                // 添加一些空间
                if (i < _animations.Count - 1)
                {
                    UISystem.GUILayout.Label("", UISystem.GUILayout.Height(2));
                }
            }

            UISystem.GUILayout.EndVertical();
        }
    }
}
