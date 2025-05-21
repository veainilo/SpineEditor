using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spine;
using SpineEditor.Animation;

namespace SpineEditor.Events
{
    /// <summary>
    /// Spine 事件编辑器，扩展 SpineAnimationPlayer 添加事件管理功能
    /// </summary>
    public class SpineEventEditor : SpineAnimationPlayer
    {
        private List<FrameEvent> _events = new List<FrameEvent>();
        private float _currentTime = 0;
        private string _skeletonDataFilePath;
        private bool _isPlaying = false;
        private float _playbackSpeed = 1.0f;

        /// <summary>
        /// 获取或设置当前时间
        /// </summary>
        public float CurrentTime
        {
            get => _currentTime;
            set
            {
                _currentTime = value;
                // 确保时间不为负
                if (_currentTime < 0)
                    _currentTime = 0;

                // 如果有动画在播放，设置动画时间
                if (AnimationState != null && AnimationState.GetCurrent(0) != null)
                {
                    // 重置动画状态
                    AnimationState.GetCurrent(0).TrackTime = _currentTime;

                    // 更新骨骼
                    AnimationState.Apply(Skeleton);
                    Skeleton.UpdateWorldTransform();
                }
            }
        }

        /// <summary>
        /// 获取或设置是否正在播放
        /// </summary>
        public bool IsPlaying
        {
            get => _isPlaying;
            set => _isPlaying = value;
        }

        /// <summary>
        /// 获取或设置播放速度
        /// </summary>
        public float PlaybackSpeed
        {
            get => _playbackSpeed;
            set => _playbackSpeed = value;
        }

        /// <summary>
        /// 获取事件列表
        /// </summary>
        public List<FrameEvent> Events => _events;

        /// <summary>
        /// 获取骨骼数据文件路径
        /// </summary>
        public string SkeletonDataFilePath => _skeletonDataFilePath;

        /// <summary>
        /// 获取当前动画的持续时间
        /// </summary>
        public float AnimationDuration
        {
            get
            {
                if (AnimationState != null && AnimationState.GetCurrent(0) != null)
                    return AnimationState.GetCurrent(0).Animation.Duration;
                return 0;
            }
        }

        /// <summary>
        /// 事件触发委托
        /// </summary>
        public event EventHandler<FrameEvent> OnEventTriggered;

        /// <summary>
        /// 创建 Spine 事件编辑器
        /// </summary>
        /// <param name="graphicsDevice">图形设备</param>
        public SpineEventEditor(GraphicsDevice graphicsDevice) : base(graphicsDevice)
        {
        }

        /// <summary>
        /// 加载 Spine 动画
        /// </summary>
        /// <param name="atlasPath">Atlas 文件路径</param>
        /// <param name="skeletonPath">Skeleton 文件路径</param>
        /// <param name="scale">缩放比例</param>
        /// <param name="position">位置</param>
        /// <returns>是否加载成功</returns>
        public override bool LoadAnimation(string atlasPath, string skeletonPath, float scale = 1.0f, Vector2? position = null)
        {
            _skeletonDataFilePath = skeletonPath;
            _currentTime = 0;
            _events.Clear();
            return base.LoadAnimation(atlasPath, skeletonPath, scale, position);
        }

        /// <summary>
        /// 切换动画
        /// </summary>
        /// <param name="animationName">动画名称</param>
        /// <param name="loop">是否循环播放</param>
        /// <param name="saveEvents">是否保存当前动画的事件数据</param>
        /// <param name="loadEvents">是否加载新动画的事件数据</param>
        /// <returns>是否成功切换</returns>
        public bool SwitchAnimation(string animationName, bool loop = true, bool saveEvents = true, bool loadEvents = true)
        {
            // 如果当前没有动画，直接返回
            if (AnimationState == null || Skeleton == null)
            {
                Console.WriteLine("无法切换动画：AnimationState 或 Skeleton 为空");
                return false;
            }

            // 检查动画名称是否有效
            if (string.IsNullOrEmpty(animationName))
            {
                Console.WriteLine("无法切换动画：动画名称为空");
                return false;
            }

            // 检查动画是否存在
            bool animationExists = false;
            foreach (var anim in Skeleton.Data.Animations)
            {
                if (anim.Name == animationName)
                {
                    animationExists = true;
                    break;
                }
            }

            if (!animationExists)
            {
                Console.WriteLine($"无法切换动画：找不到名为 {animationName} 的动画");
                return false;
            }

            // 如果动画名称相同，不需要切换
            if (CurrentAnimation == animationName)
            {
                Console.WriteLine($"动画已经是 {animationName}，无需切换");
                return true;
            }

            Console.WriteLine($"开始切换动画从 {CurrentAnimation} 到 {animationName}");

            // 保存当前动画的事件数据
            if (saveEvents && !string.IsNullOrEmpty(CurrentAnimation))
            {
                string currentEventFilePath = Path.GetDirectoryName(_skeletonDataFilePath);
                if (!string.IsNullOrEmpty(currentEventFilePath))
                {
                    string fileName = Path.GetFileNameWithoutExtension(_skeletonDataFilePath);
                    string eventFilePath = Path.Combine(currentEventFilePath, fileName + "_events.json");

                    SaveEventsToJson(eventFilePath, CurrentAnimation);
                    Console.WriteLine($"已保存当前动画 {CurrentAnimation} 的事件数据到 {eventFilePath}");
                }
            }

            // 播放新动画
            bool success = PlayAnimation(animationName, loop);
            if (!success)
            {
                Console.WriteLine($"播放动画 {animationName} 失败");
                return false;
            }

            Console.WriteLine($"成功播放动画 {animationName}");

            // 重置时间
            _currentTime = 0;

            // 清除事件
            _events.Clear();

            // 加载新动画的事件数据
            if (loadEvents)
            {
                // 尝试从当前设置的事件文件路径加载
                string currentEventFilePath = Path.GetDirectoryName(_skeletonDataFilePath);
                if (!string.IsNullOrEmpty(currentEventFilePath))
                {
                    string fileName = Path.GetFileNameWithoutExtension(_skeletonDataFilePath);
                    string eventFilePath = Path.Combine(currentEventFilePath, fileName + "_events.json");

                    if (File.Exists(eventFilePath))
                    {
                        bool loadSuccess = LoadEventsFromJson(eventFilePath);
                        Console.WriteLine($"从文件 {eventFilePath} 加载动画 {animationName} 的事件数据{(loadSuccess ? "成功" : "失败")}");
                    }
                    else
                    {
                        Console.WriteLine($"事件文件不存在: {eventFilePath}");
                    }
                }
            }

            Console.WriteLine($"动画切换完成：{animationName}，持续时间：{AnimationDuration}秒");
            return true;
        }

        /// <summary>
        /// 添加事件
        /// </summary>
        /// <param name="name">事件名称</param>
        /// <param name="time">事件触发时间（秒）</param>
        /// <param name="intValue">整数参数</param>
        /// <param name="floatValue">浮点参数</param>
        /// <param name="stringValue">字符串参数</param>
        public void AddEvent(string name, float time, int intValue = 0, float floatValue = 0, string stringValue = "")
        {
            Console.WriteLine($"[SpineEventEditor] AddEvent called with Name: {name}, Time: {time}, Int: {intValue}, Float: {floatValue}, String: '{stringValue}'");
            _events.Add(new FrameEvent(name, time, intValue, floatValue, stringValue));

            // 按时间排序
            _events = _events.OrderBy(e => e.Time).ToList();
        }

        /// <summary>
        /// 删除事件
        /// </summary>
        /// <param name="index">事件索引</param>
        public void RemoveEvent(int index)
        {
            if (index >= 0 && index < _events.Count)
                _events.RemoveAt(index);
        }

        /// <summary>
        /// 获取指定时间点的事件
        /// </summary>
        /// <param name="time">时间点</param>
        /// <param name="tolerance">容差</param>
        /// <returns>找到的事件，如果没有找到则返回 null</returns>
        public FrameEvent GetEventAtTime(float time, float tolerance = 0.1f)
        {
            return _events.FirstOrDefault(e => Math.Abs(e.Time - time) <= tolerance);
        }

        /// <summary>
        /// 保存事件到 JSON
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="animationName">动画名称</param>
        public void SaveEventsToJson(string filePath, string animationName)
        {
            AnimationEventData data = null;
            bool existingFileLoadedSuccessfully = false;

            if (File.Exists(filePath))
            {
                data = AnimationEventData.LoadFromJson(filePath);
                if (data != null)
                {
                    existingFileLoadedSuccessfully = true;
                }
                else
                {
                    // 文件存在但加载失败
                    Console.WriteLine($"错误：无法加载现有的事件文件 {filePath}。保存操作已取消，以防止数据丢失。");
                    // 可以在这里添加用户提示，例如通过Toast消息
                    // _toast.Show($"错误：无法加载事件文件 {Path.GetFileName(filePath)}。保存已取消。", 3.0f, Color.Red);
                    return; // 阻止保存
                }
            }
            else
            {
                // 文件不存在，创建一个新的 AnimationEventData
                data = new AnimationEventData();
                existingFileLoadedSuccessfully = true; // 视为成功，因为是新文件
            }

            // 如果加载成功（或文件原先不存在），则设置当前动画的事件
            if (existingFileLoadedSuccessfully) // 理论上此时 data 不应为 null
            {
                data.SetEventsForAnimation(animationName, _events);
                data.SpineFileName = Path.GetFileName(_skeletonDataFilePath);

                // 保存数据
                data.SaveToJson(filePath);
            }
            // 此处可以添加一个 else 分支处理 data 为 null 的意外情况，尽管理论上不应发生
        }

        /// <summary>
        /// 从 JSON 加载事件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>是否加载成功</returns>
        public bool LoadEventsFromJson(string filePath)
        {
            var data = AnimationEventData.LoadFromJson(filePath);
            if (data != null)
            {
                // 获取当前动画的事件
                string currentAnimation = CurrentAnimation;
                if (!string.IsNullOrEmpty(currentAnimation))
                {
                    _events = data.GetEventsForAnimation(currentAnimation);
                    return true;
                }
                else if (data.Events != null && data.Events.Count > 0)
                {
                    // 兼容旧版本
                    _events = data.Events;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 更新动画
        /// </summary>
        /// <param name="deltaTime">时间增量（秒）</param>
        public override void Update(float deltaTime)
        {
            float previousTime = _currentTime;

            if (_isPlaying)
            {
                _currentTime += deltaTime * _playbackSpeed;

                // 循环播放
                if (AnimationState != null && AnimationState.GetCurrent(0) != null)
                {
                    float duration = AnimationState.GetCurrent(0).Animation.Duration;
                    if (_currentTime > duration)
                        _currentTime = 0;
                }
            }

            // 即使没有播放，也要确保骨骼位置正确
            if (AnimationState != null && AnimationState.GetCurrent(0) != null)
            {
                // 设置动画时间
                AnimationState.GetCurrent(0).TrackTime = _currentTime;

                // 更新骨骼
                AnimationState.Apply(Skeleton);
                Skeleton.UpdateWorldTransform();
            }

            // 调用基类的 Update 方法，但只有在播放时才传递时间
            if (_isPlaying)
            {
                base.Update(deltaTime * _playbackSpeed);
            }
            else
            {
                // 即使不播放，也要确保骨骼位置正确
                base.Update(0);
            }

            // 检查是否有事件需要触发
            foreach (var evt in _events)
            {
                if ((previousTime < evt.Time && _currentTime >= evt.Time) ||
                    (previousTime > _currentTime && (previousTime < evt.Time || _currentTime >= evt.Time))) // 处理循环播放
                {
                    // 触发事件
                    OnEventTriggered?.Invoke(this, evt);
                }
            }
        }
    }
}
