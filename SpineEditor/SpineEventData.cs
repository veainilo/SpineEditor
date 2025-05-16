using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using System.Collections.Concurrent;

namespace SpineEditor
{
    /// <summary>
    /// 表示攻击形状
    /// </summary>
    public class AttackShape
    {
        /// <summary>
        /// X 坐标
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Y 坐标
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// 宽度
        /// </summary>
        public float Width { get; set; }

        /// <summary>
        /// 高度
        /// </summary>
        public float Height { get; set; }

        /// <summary>
        /// 旋转角度
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
        public AttackShape Shape { get; set; }
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
        public string Type { get; set; }

        /// <summary>
        /// 攻击数据
        /// </summary>
        public AttackData Attack { get; set; }

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
            Type = "";
            IntValue = 0;
            FloatValue = 0;
            StringValue = "";
        }

        /// <summary>
        /// 创建一个新的帧事件
        /// </summary>
        /// <param name="name">事件名称</param>
        /// <param name="time">事件触发时间（秒）</param>
        /// <param name="intValue">整数参数</param>
        /// <param name="floatValue">浮点参数</param>
        /// <param name="stringValue">字符串参数</param>
        public FrameEvent(string name, float time, int intValue = 0, float floatValue = 0, string stringValue = "")
        {
            Name = name;
            Time = time;
            Frame = (int)(time * 30); // 假设 30 帧每秒
            Type = "";
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
