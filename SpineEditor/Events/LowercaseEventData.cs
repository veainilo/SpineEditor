using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SpineEditor.Events
{
    /// <summary>
    /// 用于JSON序列化的小写风格事件数据类
    /// </summary>
    public class LowercaseEventData
    {
        /// <summary>
        /// 所有动画的事件数据
        /// </summary>
        [JsonPropertyName("animations")]
        public Dictionary<string, List<LowercaseFrameEvent>> Animations { get; set; }

        /// <summary>
        /// 创建一个新的小写风格事件数据
        /// </summary>
        public LowercaseEventData()
        {
            Animations = new Dictionary<string, List<LowercaseFrameEvent>>();
        }

        /// <summary>
        /// 从AnimationEventData转换
        /// </summary>
        /// <param name="data">原始事件数据</param>
        /// <returns>小写风格事件数据</returns>
        public static LowercaseEventData FromAnimationEventData(AnimationEventData data)
        {
            var result = new LowercaseEventData();

            if (data.Animations != null)
            {
                foreach (var animPair in data.Animations)
                {
                    var events = new List<LowercaseFrameEvent>();
                    foreach (var evt in animPair.Value)
                    {
                        events.Add(LowercaseFrameEvent.FromFrameEvent(evt));
                    }
                    result.Animations[animPair.Key] = events;
                }
            }

            return result;
        }
    }

    /// <summary>
    /// 用于JSON序列化的小写风格帧事件
    /// </summary>
    public class LowercaseFrameEvent
    {
        /// <summary>
        /// 事件名称
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// 事件触发时间（秒）
        /// </summary>
        [JsonPropertyName("time")]
        public float Time { get; set; }

        /// <summary>
        /// 事件对应的帧数
        /// </summary>
        [JsonPropertyName("frame")]
        public int Frame { get; set; }

        /// <summary>
        /// 事件类型
        /// </summary>
        [JsonPropertyName("type")]
        public int Type { get; set; }

        /// <summary>
        /// 攻击数据（仅当 Type 为 1 时有效）
        /// </summary>
        [JsonPropertyName("attack")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public LowercaseAttackData Attack { get; set; }

        /// <summary>
        /// 特效数据（仅当 Type 为 2 时有效）
        /// </summary>
        [JsonPropertyName("effect")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public LowercaseEffectData Effect { get; set; }

        /// <summary>
        /// 声音数据（仅当 Type 为 3 时有效）
        /// </summary>
        [JsonPropertyName("sound")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public LowercaseSoundData Sound { get; set; }

        /// <summary>
        /// 从FrameEvent转换
        /// </summary>
        /// <param name="evt">原始帧事件</param>
        /// <returns>小写风格帧事件</returns>
        public static LowercaseFrameEvent FromFrameEvent(FrameEvent evt)
        {
            var result = new LowercaseFrameEvent
            {
                Name = evt.Name,
                Time = evt.Time,
                Frame = evt.Frame,
                Type = (int)evt.EventType
            };

            // 根据事件类型设置相应的数据
            switch (evt.EventType)
            {
                case EventType.Attack:
                    if (evt.Attack != null)
                    {
                        result.Attack = LowercaseAttackData.FromAttackData(evt.Attack);
                    }
                    break;
                case EventType.Effect:
                    if (evt.Effect != null)
                    {
                        result.Effect = LowercaseEffectData.FromEffectData(evt.Effect);
                    }
                    break;
                case EventType.Sound:
                    if (evt.Sound != null)
                    {
                        result.Sound = LowercaseSoundData.FromSoundData(evt.Sound);
                    }
                    break;
            }

            return result;
        }
    }

    /// <summary>
    /// 用于JSON序列化的小写风格攻击数据
    /// </summary>
    public class LowercaseAttackData
    {
        /// <summary>
        /// 攻击类型
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// 伤害值
        /// </summary>
        [JsonPropertyName("damage")]
        public int Damage { get; set; }

        /// <summary>
        /// 攻击形状
        /// </summary>
        [JsonPropertyName("shape")]
        public LowercaseAttackShape Shape { get; set; }

        /// <summary>
        /// 从AttackData转换
        /// </summary>
        /// <param name="data">原始攻击数据</param>
        /// <returns>小写风格攻击数据</returns>
        public static LowercaseAttackData FromAttackData(AttackData data)
        {
            return new LowercaseAttackData
            {
                Type = data.Type,
                Damage = data.Damage,
                Shape = data.Shape != null ? LowercaseAttackShape.FromAttackShape(data.Shape) : null
            };
        }
    }

    /// <summary>
    /// 用于JSON序列化的小写风格攻击形状
    /// </summary>
    public class LowercaseAttackShape
    {
        /// <summary>
        /// 形状类型
        /// </summary>
        [JsonPropertyName("type")]
        public int Type { get; set; }

        /// <summary>
        /// X 坐标
        /// </summary>
        [JsonPropertyName("x")]
        public float X { get; set; }

        /// <summary>
        /// Y 坐标
        /// </summary>
        [JsonPropertyName("y")]
        public float Y { get; set; }

        /// <summary>
        /// 宽度或半径
        /// </summary>
        [JsonPropertyName("width")]
        public float Width { get; set; }

        /// <summary>
        /// 高度
        /// </summary>
        [JsonPropertyName("height")]
        public float Height { get; set; }

        /// <summary>
        /// 旋转角度
        /// </summary>
        [JsonPropertyName("rotation")]
        public float Rotation { get; set; }

        /// <summary>
        /// 从AttackShape转换
        /// </summary>
        /// <param name="shape">原始攻击形状</param>
        /// <returns>小写风格攻击形状</returns>
        public static LowercaseAttackShape FromAttackShape(AttackShape shape)
        {
            return new LowercaseAttackShape
            {
                Type = (int)shape.Type,
                X = shape.X,
                Y = shape.Y,
                Width = shape.Width,
                Height = shape.Height,
                Rotation = shape.Rotation
            };
        }
    }

    /// <summary>
    /// 用于JSON序列化的小写风格特效数据
    /// </summary>
    public class LowercaseEffectData
    {
        /// <summary>
        /// 特效名称
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// 特效位置 X
        /// </summary>
        [JsonPropertyName("x")]
        public float X { get; set; }

        /// <summary>
        /// 特效位置 Y
        /// </summary>
        [JsonPropertyName("y")]
        public float Y { get; set; }

        /// <summary>
        /// 特效缩放
        /// </summary>
        [JsonPropertyName("scale")]
        public float Scale { get; set; }

        /// <summary>
        /// 从EffectData转换
        /// </summary>
        /// <param name="data">原始特效数据</param>
        /// <returns>小写风格特效数据</returns>
        public static LowercaseEffectData FromEffectData(EffectData data)
        {
            return new LowercaseEffectData
            {
                Name = data.Name,
                X = data.X,
                Y = data.Y,
                Scale = data.Scale
            };
        }
    }

    /// <summary>
    /// 用于JSON序列化的小写风格声音数据
    /// </summary>
    public class LowercaseSoundData
    {
        /// <summary>
        /// 声音名称
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// 音量
        /// </summary>
        [JsonPropertyName("volume")]
        public float Volume { get; set; }

        /// <summary>
        /// 音调
        /// </summary>
        [JsonPropertyName("pitch")]
        public float Pitch { get; set; }

        /// <summary>
        /// 从SoundData转换
        /// </summary>
        /// <param name="data">原始声音数据</param>
        /// <returns>小写风格声音数据</returns>
        public static LowercaseSoundData FromSoundData(SoundData data)
        {
            return new LowercaseSoundData
            {
                Name = data.Name,
                Volume = data.Volume,
                Pitch = data.Pitch
            };
        }
    }
}
