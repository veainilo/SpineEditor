using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpineEditor.Events;
using SpineEditor.UI.UISystem;
using System;
using System.Collections.Generic;

namespace SpineEditor.UI.GUILayoutComponents
{
    /// <summary>
    /// 基于GUILayout系统的事件属性面板
    /// </summary>
    public class EventPropertyPanelGUI : GUILayoutPanel
    {
        // 事件编辑器
        private SpineEventEditor _eventEditor;

        // 当前选中的事件
        private FrameEvent _selectedEvent;

        // 事件类型选项
        private string[] _eventTypes = { "普通", "攻击", "特效", "声音" };
        private int _selectedEventType = 0;

        // 形状类型选项
        private string[] _shapeTypes = { "矩形", "圆形" };
        private int _selectedShapeType = 0;

        // 事件
        public event EventHandler<FrameEvent> EventAdded;
        public event EventHandler<FrameEvent> EventDeleted;
        public event EventHandler<FrameEvent> EventModified;

        /// <summary>
        /// 创建事件属性面板
        /// </summary>
        /// <param name="eventEditor">事件编辑器</param>
        /// <param name="title">面板标题</param>
        /// <param name="bounds">面板边界</param>
        /// <param name="graphicsDevice">图形设备</param>
        /// <param name="font">字体</param>
        public EventPropertyPanelGUI(SpineEventEditor eventEditor, string title, Rectangle bounds, GraphicsDevice graphicsDevice, SpriteFont font)
            : base(title, bounds, graphicsDevice, font)
        {
            _eventEditor = eventEditor;
        }

        /// <summary>
        /// 设置选中的事件
        /// </summary>
        /// <param name="evt">事件</param>
        public void SetSelectedEvent(FrameEvent evt)
        {
            _selectedEvent = evt;

            if (evt != null)
            {
                // 设置事件类型
                _selectedEventType = (int)evt.EventType;

                // 设置形状类型
                if (evt.EventType == EventType.Attack)
                {
                    _selectedShapeType = (int)evt.Attack.Shape.Type;
                }
            }
        }

        /// <summary>
        /// 绘制GUI内容
        /// </summary>
        protected override void DrawGUI()
        {
            // 主面板
            UISystem.GUILayout.BeginVertical();

            // 添加事件按钮
            if (UISystem.GUILayout.Button("添加事件", UISystem.GUILayout.Width(100)))
            {
                AddNewEvent();
            }

            // 如果没有选中事件，显示提示信息
            if (_selectedEvent == null)
            {
                UISystem.GUILayout.Label("请选择一个事件进行编辑");
                UISystem.GUILayout.EndVertical();
                return;
            }

            // 分隔线
            UISystem.GUILayoutHelper.Separator();

            // 通用属性
            DrawCommonProperties();

            // 事件类型选择
            DrawEventTypeSelection();

            // 根据事件类型绘制特定属性
            switch (_selectedEvent.EventType)
            {
                case EventType.Normal:
                    DrawNormalEventProperties();
                    break;
                case EventType.Attack:
                    DrawAttackEventProperties();
                    break;
                case EventType.Effect:
                    DrawEffectEventProperties();
                    break;
                case EventType.Sound:
                    DrawSoundEventProperties();
                    break;
            }

            // 删除事件按钮
            if (UISystem.GUILayout.Button("删除事件", UISystem.GUILayout.Width(100)))
            {
                DeleteSelectedEvent();
            }

            UISystem.GUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制通用属性
        /// </summary>
        private void DrawCommonProperties()
        {
            UISystem.GUILayoutHelper.Title("通用属性", 2);

            // 事件名称
            string newName = UISystem.GUILayoutHelper.LabelField("事件名称", _selectedEvent.Name);
            if (newName != _selectedEvent.Name)
            {
                _selectedEvent.Name = newName;
                EventModified?.Invoke(this, _selectedEvent);
            }

            // 事件时间
            string timeText = UISystem.GUILayoutHelper.LabelField("事件时间", _selectedEvent.Time.ToString("F3"));
            if (float.TryParse(timeText, out float newTime) && newTime != _selectedEvent.Time)
            {
                _selectedEvent.Time = newTime;
                EventModified?.Invoke(this, _selectedEvent);
            }
        }

        /// <summary>
        /// 绘制事件类型选择
        /// </summary>
        private void DrawEventTypeSelection()
        {
            UISystem.GUILayoutHelper.Title("事件类型", 2);

            // 事件类型选择
            int newEventType = UISystem.GUILayoutHelper.Tabs(_selectedEventType, _eventTypes);
            if (newEventType != _selectedEventType)
            {
                _selectedEventType = newEventType;
                _selectedEvent.EventType = (EventType)_selectedEventType;
                EventModified?.Invoke(this, _selectedEvent);
            }
        }

        /// <summary>
        /// 绘制普通事件属性
        /// </summary>
        private void DrawNormalEventProperties()
        {
            UISystem.GUILayoutHelper.Title("普通事件属性", 2);

            // 整数值
            int newIntValue = UISystem.GUILayoutHelper.IntField("整数值", _selectedEvent.IntValue);
            if (newIntValue != _selectedEvent.IntValue)
            {
                _selectedEvent.IntValue = newIntValue;
                EventModified?.Invoke(this, _selectedEvent);
            }

            // 浮点值
            float newFloatValue = UISystem.GUILayoutHelper.FloatField("浮点值", _selectedEvent.FloatValue);
            if (newFloatValue != _selectedEvent.FloatValue)
            {
                _selectedEvent.FloatValue = newFloatValue;
                EventModified?.Invoke(this, _selectedEvent);
            }

            // 字符串值
            string newStringValue = UISystem.GUILayoutHelper.LabelField("字符串值", _selectedEvent.StringValue);
            if (newStringValue != _selectedEvent.StringValue)
            {
                _selectedEvent.StringValue = newStringValue;
                EventModified?.Invoke(this, _selectedEvent);
            }
        }

        /// <summary>
        /// 绘制攻击事件属性
        /// </summary>
        private void DrawAttackEventProperties()
        {
            UISystem.GUILayoutHelper.Title("攻击事件属性", 2);

            // 攻击类型
            string newAttackType = UISystem.GUILayoutHelper.LabelField("攻击类型", _selectedEvent.Attack.Type);
            if (newAttackType != _selectedEvent.Attack.Type)
            {
                _selectedEvent.Attack.Type = newAttackType;
                EventModified?.Invoke(this, _selectedEvent);
            }

            // 攻击伤害
            int newDamage = UISystem.GUILayoutHelper.IntField("伤害值", _selectedEvent.Attack.Damage);
            if (newDamage != _selectedEvent.Attack.Damage)
            {
                _selectedEvent.Attack.Damage = newDamage;
                EventModified?.Invoke(this, _selectedEvent);
            }

            // 形状类型
            int newShapeType = UISystem.GUILayoutHelper.Tabs(_selectedShapeType, _shapeTypes);
            if (newShapeType != _selectedShapeType)
            {
                _selectedShapeType = newShapeType;
                _selectedEvent.Attack.Shape.Type = (ShapeType)_selectedShapeType;
                EventModified?.Invoke(this, _selectedEvent);
            }

            // 形状位置
            Vector2 newPosition = UISystem.GUILayoutHelper.Vector2Field("位置", new Vector2(_selectedEvent.Attack.Shape.X, _selectedEvent.Attack.Shape.Y));
            if (newPosition.X != _selectedEvent.Attack.Shape.X || newPosition.Y != _selectedEvent.Attack.Shape.Y)
            {
                _selectedEvent.Attack.Shape.X = newPosition.X;
                _selectedEvent.Attack.Shape.Y = newPosition.Y;
                EventModified?.Invoke(this, _selectedEvent);
            }

            // 形状宽度
            float newWidth = UISystem.GUILayoutHelper.FloatField("宽度", _selectedEvent.Attack.Shape.Width);
            if (newWidth != _selectedEvent.Attack.Shape.Width)
            {
                _selectedEvent.Attack.Shape.Width = newWidth;
                EventModified?.Invoke(this, _selectedEvent);
            }

            // 根据形状类型显示不同的属性
            if (_selectedEvent.Attack.Shape.Type == ShapeType.Rectangle)
            {
                // 矩形高度
                float newHeight = UISystem.GUILayoutHelper.FloatField("高度", _selectedEvent.Attack.Shape.Height);
                if (newHeight != _selectedEvent.Attack.Shape.Height)
                {
                    _selectedEvent.Attack.Shape.Height = newHeight;
                    EventModified?.Invoke(this, _selectedEvent);
                }

                // 矩形旋转
                float newRotation = UISystem.GUILayoutHelper.FloatField("旋转", _selectedEvent.Attack.Shape.Rotation);
                if (newRotation != _selectedEvent.Attack.Shape.Rotation)
                {
                    _selectedEvent.Attack.Shape.Rotation = newRotation;
                    EventModified?.Invoke(this, _selectedEvent);
                }
            }
        }

        /// <summary>
        /// 绘制特效事件属性
        /// </summary>
        private void DrawEffectEventProperties()
        {
            UISystem.GUILayoutHelper.Title("特效事件属性", 2);

            // 特效名称
            string newEffectName = UISystem.GUILayoutHelper.LabelField("特效名称", _selectedEvent.Effect.Name);
            if (newEffectName != _selectedEvent.Effect.Name)
            {
                _selectedEvent.Effect.Name = newEffectName;
                EventModified?.Invoke(this, _selectedEvent);
            }

            // 特效位置
            Vector2 newPosition = UISystem.GUILayoutHelper.Vector2Field("位置", new Vector2(_selectedEvent.Effect.X, _selectedEvent.Effect.Y));
            if (newPosition.X != _selectedEvent.Effect.X || newPosition.Y != _selectedEvent.Effect.Y)
            {
                _selectedEvent.Effect.X = newPosition.X;
                _selectedEvent.Effect.Y = newPosition.Y;
                EventModified?.Invoke(this, _selectedEvent);
            }

            // 特效缩放
            float newScale = UISystem.GUILayoutHelper.FloatField("缩放", _selectedEvent.Effect.Scale);
            if (newScale != _selectedEvent.Effect.Scale)
            {
                _selectedEvent.Effect.Scale = newScale;
                EventModified?.Invoke(this, _selectedEvent);
            }
        }

        /// <summary>
        /// 绘制声音事件属性
        /// </summary>
        private void DrawSoundEventProperties()
        {
            UISystem.GUILayoutHelper.Title("声音事件属性", 2);

            // 声音名称
            string newSoundName = UISystem.GUILayoutHelper.LabelField("声音名称", _selectedEvent.Sound.Name);
            if (newSoundName != _selectedEvent.Sound.Name)
            {
                _selectedEvent.Sound.Name = newSoundName;
                EventModified?.Invoke(this, _selectedEvent);
            }

            // 声音音量
            float newVolume = UISystem.GUILayoutHelper.FloatField("音量", _selectedEvent.Sound.Volume);
            if (newVolume != _selectedEvent.Sound.Volume)
            {
                _selectedEvent.Sound.Volume = newVolume;
                EventModified?.Invoke(this, _selectedEvent);
            }

            // 声音音调
            float newPitch = UISystem.GUILayoutHelper.FloatField("音调", _selectedEvent.Sound.Pitch);
            if (newPitch != _selectedEvent.Sound.Pitch)
            {
                _selectedEvent.Sound.Pitch = newPitch;
                EventModified?.Invoke(this, _selectedEvent);
            }
        }

        /// <summary>
        /// 添加新事件
        /// </summary>
        private void AddNewEvent()
        {
            // 在当前时间点添加新事件
            float currentTime = _eventEditor.CurrentTime;
            FrameEvent newEvent = new FrameEvent("New Event", currentTime, 0, 0, "");
            _eventEditor.AddEvent(newEvent.Name, newEvent.Time);

            // 选中新添加的事件
            SetSelectedEvent(newEvent);

            // 触发事件
            EventAdded?.Invoke(this, newEvent);
        }

        /// <summary>
        /// 删除选中的事件
        /// </summary>
        private void DeleteSelectedEvent()
        {
            if (_selectedEvent != null)
            {
                int index = _eventEditor.Events.IndexOf(_selectedEvent);
                if (index >= 0)
                {
                    FrameEvent deletedEvent = _selectedEvent;
                    _eventEditor.RemoveEvent(index);
                    _selectedEvent = null;

                    // 触发事件
                    EventDeleted?.Invoke(this, deletedEvent);
                }
            }
        }

        /// <summary>
        /// 获取选中的事件
        /// </summary>
        public FrameEvent SelectedEvent => _selectedEvent;
    }
}
