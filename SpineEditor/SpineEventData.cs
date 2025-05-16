using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;

namespace SpineEditor
{
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
        /// 整数参数
        /// </summary>
        public int IntValue { get; set; }

        /// <summary>
        /// 浮点参数
        /// </summary>
        public float FloatValue { get; set; }

        /// <summary>
        /// 字符串参数
        /// </summary>
        public string StringValue { get; set; }

        /// <summary>
        /// 创建一个新的帧事件
        /// </summary>
        public FrameEvent()
        {
            Name = "New Event";
            Time = 0;
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
        /// <summary>
        /// 动画名称
        /// </summary>
        public string AnimationName { get; set; }

        /// <summary>
        /// Spine 文件名
        /// </summary>
        public string SpineFileName { get; set; }

        /// <summary>
        /// 帧事件列表
        /// </summary>
        public List<FrameEvent> Events { get; set; }

        /// <summary>
        /// 创建一个新的动画事件数据
        /// </summary>
        public AnimationEventData()
        {
            AnimationName = "";
            SpineFileName = "";
            Events = new List<FrameEvent>();
        }

        /// <summary>
        /// 保存事件数据到 JSON 文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public void SaveToJson(string filePath)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            string json = JsonSerializer.Serialize(this, options);
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
                return JsonSerializer.Deserialize<AnimationEventData>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"加载事件数据时出错: {ex.Message}");
                return null;
            }
        }
    }
}
