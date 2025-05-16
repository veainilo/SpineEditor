using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spine;

namespace SpineEditor
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
        /// 添加事件
        /// </summary>
        /// <param name="name">事件名称</param>
        /// <param name="time">事件触发时间（秒）</param>
        /// <param name="intValue">整数参数</param>
        /// <param name="floatValue">浮点参数</param>
        /// <param name="stringValue">字符串参数</param>
        public void AddEvent(string name, float time, int intValue = 0, float floatValue = 0, string stringValue = "")
        {
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
            var data = new AnimationEventData
            {
                AnimationName = animationName,
                SpineFileName = Path.GetFileName(_skeletonDataFilePath),
                Events = _events
            };
            
            data.SaveToJson(filePath);
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
                _events = data.Events;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 更新动画
        /// </summary>
        /// <param name="deltaTime">时间增量（秒）</param>
        public override void Update(float deltaTime)
        {
            if (!_isPlaying)
                return;

            float previousTime = _currentTime;
            _currentTime += deltaTime * _playbackSpeed;
            
            // 循环播放
            if (AnimationState != null && AnimationState.GetCurrent(0) != null)
            {
                float duration = AnimationState.GetCurrent(0).Animation.Duration;
                if (_currentTime > duration)
                    _currentTime = 0;
            }

            // 调用基类的 Update 方法
            base.Update(deltaTime * _playbackSpeed);
            
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
