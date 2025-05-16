using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using System.Collections.Concurrent;

namespace SpineEditor.Events
{
    /// <summary>
    /// 事件类型枚举
    /// </summary>
    public enum EventType
    {
        /// <summary>
        /// 普通事件
        /// </summary>
        Normal,

        /// <summary>
        /// 攻击事件
        /// </summary>
        Attack,

        /// <summary>
        /// 特效事件
        /// </summary>
        Effect,

        /// <summary>
        /// 声音事件
        /// </summary>
        Sound
    }

    /// <summary>
    /// 形状类型枚举
    /// </summary>
    public enum ShapeType
    {
        /// <summary>
        /// 矩形
        /// </summary>
        Rectangle,

        /// <summary>
        /// 圆形
        /// </summary>
        Circle
    }

    /// <summary>
    /// 表示攻击形状
    /// </summary>
    public class AttackShape
    {
        /// <summary>
        /// 形状类型
        /// </summary>
        public ShapeType Type { get; set; } = ShapeType.Rectangle;

        /// <summary>
        /// X 坐标
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Y 坐标
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// 宽度（矩形）或半径（圆形）
        /// </summary>
        public float Width { get; set; }

        /// <summary>
        /// 高度（仅矩形）
        /// </summary>
        public float Height { get; set; }

        /// <summary>
        /// 旋转角度（仅矩形）
        /// </summary>
        public float Rotation { get; set; }
    }

    /// <summary>
    /// 表示攻击数据
    /// </summary>
    public class AttackData
    {
        /// <summary>
        /// 攻击类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 伤害值
        /// </summary>
        public int Damage { get; set; }

        /// <summary>
        /// 攻击形状
        /// </summary>
        public AttackShape Shape { get; set; } = new AttackShape();
    }

    /// <summary>
    /// 表示特效数据
    /// </summary>
    public class EffectData
    {
        /// <summary>
        /// 特效名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 特效位置 X
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// 特效位置 Y
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// 特效缩放
        /// </summary>
        public float Scale { get; set; } = 1.0f;
    }

    /// <summary>
    /// 表示声音数据
    /// </summary>
    public class SoundData
    {
        /// <summary>
        /// 声音名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 音量
        /// </summary>
        public float Volume { get; set; } = 1.0f;

        /// <summary>
        /// 音调
        /// </summary>
        public float Pitch { get; set; } = 1.0f;
    }

    /// <summary>
    /// 表示 Spine 动画中的一个帧事件
    /// </summary>
    public class FrameEvent
    {
        /// <summary>
        /// 事件名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 事件触发时间（秒）
        /// </summary>
        public float Time { get; set; }

        /// <summary>
        /// 事件对应的帧数
        /// </summary>
        public int Frame { get; set; }

        /// <summary>
        /// 事件类型
        /// </summary>
        public EventType EventType { get; set; } = EventType.Normal;

        /// <summary>
        /// 攻击数据（仅当 EventType 为 Attack 时有效）
        /// </summary>
        public AttackData Attack { get; set; }

        /// <summary>
        /// 特效数据（仅当 EventType 为 Effect 时有效）
        /// </summary>
        public EffectData Effect { get; set; }

        /// <summary>
        /// 声音数据（仅当 EventType 为 Sound 时有效）
        /// </summary>
        public SoundData Sound { get; set; }

        /// <summary>
        /// 整数参数（兼容旧版本）
        /// </summary>
        [JsonIgnore]
        public int IntValue { get; set; }

        /// <summary>
        /// 浮点参数（兼容旧版本）
        /// </summary>
        [JsonIgnore]
        public float FloatValue { get; set; }

        /// <summary>
        /// 字符串参数（兼容旧版本）
        /// </summary>
        [JsonIgnore]
        public string StringValue { get; set; }

        /// <summary>
        /// 创建一个新的帧事件
        /// </summary>
        public FrameEvent()
        {
            Name = "New Event";
            Time = 0;
            Frame = 0;
            EventType = EventType.Normal;
            IntValue = 0;
            FloatValue = 0;
            StringValue = "";
        }

        /// <summary>
        /// 创建一个新的帧事件
        /// </summary>
        /// <param name="name">事件名称</param>
        /// <param name="time">事件触发时间（秒）</param>
        /// <param name="eventType">事件类型</param>
        public FrameEvent(string name, float time, EventType eventType = EventType.Normal)
        {
            Name = name;
            Time = time;
            Frame = (int)(time * 30); // 假设 30 帧每秒
            EventType = eventType;

            // 根据事件类型初始化相应的数据
            switch (eventType)
            {
                case EventType.Attack:
                    Attack = new AttackData();
                    break;
                case EventType.Effect:
                    Effect = new EffectData();
                    break;
                case EventType.Sound:
                    Sound = new SoundData();
                    break;
            }
        }

        /// <summary>
        /// 创建一个新的帧事件（兼容旧版本）
        /// </summary>
        /// <param name="name">事件名称</param>
        /// <param name="time">事件触发时间（秒）</param>
        /// <param name="intValue">整数参数</param>
        /// <param name="floatValue">浮点参数</param>
        /// <param name="stringValue">字符串参数</param>
        public FrameEvent(string name, float time, int intValue = 0, float floatValue = 0, string stringValue = "")
            : this(name, time, EventType.Normal)
        {
            IntValue = intValue;
            FloatValue = floatValue;
            StringValue = stringValue;
        }
    }

    /// <summary>
    /// 表示 Spine 动画的所有帧事件数据
    /// </summary>
    public class AnimationEventData
    {
        // 静态 JSON 序列化选项
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        /// <summary>
        /// 动画名称（兼容旧版本）
        /// </summary>
        [JsonIgnore]
        public string AnimationName { get; set; }

        /// <summary>
        /// Spine 文件名（兼容旧版本）
        /// </summary>
        [JsonIgnore]
        public string SpineFileName { get; set; }

        /// <summary>
        /// 帧事件列表（兼容旧版本）
        /// </summary>
        [JsonIgnore]
        public List<FrameEvent> Events { get; set; }

        /// <summary>
        /// 所有动画的事件数据
        /// </summary>
        public Dictionary<string, List<FrameEvent>> Animations { get; set; }

        /// <summary>
        /// 创建一个新的动画事件数据
        /// </summary>
        public AnimationEventData()
        {
            AnimationName = "";
            SpineFileName = "";
            Events = new List<FrameEvent>();
            Animations = new Dictionary<string, List<FrameEvent>>();
        }

        /// <summary>
        /// 获取指定动画的事件列表
        /// </summary>
        /// <param name="animationName">动画名称</param>
        /// <returns>事件列表</returns>
        public List<FrameEvent> GetEventsForAnimation(string animationName)
        {
            // 如果 Animations 为空，但 Events 不为空，说明是旧版本数据
            if ((Animations == null || Animations.Count == 0) && Events != null && Events.Count > 0)
            {
                Animations = new Dictionary<string, List<FrameEvent>>
                {
                    { AnimationName, Events }
                };
            }

            // 确保 Animations 不为空
            Animations ??= new Dictionary<string, List<FrameEvent>>();

            // 如果不存在该动画的事件列表，创建一个空列表
            if (!Animations.ContainsKey(animationName))
            {
                Animations[animationName] = new List<FrameEvent>();
            }

            return Animations[animationName];
        }

        /// <summary>
        /// 设置指定动画的事件列表
        /// </summary>
        /// <param name="animationName">动画名称</param>
        /// <param name="events">事件列表</param>
        public void SetEventsForAnimation(string animationName, List<FrameEvent> events)
        {
            // 确保 Animations 不为空
            Animations ??= new Dictionary<string, List<FrameEvent>>();

            // 设置事件列表
            Animations[animationName] = events;

            // 同时更新兼容旧版本的属性
            AnimationName = animationName;
            Events = events;
        }

        /// <summary>
        /// 保存事件数据到 JSON 文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public void SaveToJson(string filePath)
        {
            // 如果 Animations 为空，但 Events 不为空，说明是旧版本数据
            if ((Animations == null || Animations.Count == 0) && Events != null && Events.Count > 0)
            {
                Animations = new Dictionary<string, List<FrameEvent>>
                {
                    { AnimationName, Events }
                };
            }

            string json = JsonSerializer.Serialize(this, _jsonOptions);
            File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// 从 JSON 文件加载事件数据
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>加载的动画事件数据，如果加载失败则返回 null</returns>
        public static AnimationEventData LoadFromJson(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            try
            {
                string json = File.ReadAllText(filePath);
                var data = JsonSerializer.Deserialize<AnimationEventData>(json);

                // 如果 Animations 为空，但 Events 不为空，说明是旧版本数据
                if ((data.Animations == null || data.Animations.Count == 0) && data.Events != null && data.Events.Count > 0)
                {
                    data.Animations = new Dictionary<string, List<FrameEvent>>
                    {
                        { data.AnimationName, data.Events }
                    };
                }

                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"加载事件数据时出错: {ex.Message}");
                return null;
            }
        }
    }
}
