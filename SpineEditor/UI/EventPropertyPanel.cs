using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpineEditor.Events;

namespace SpineEditor.UI
{
    /// <summary>
    /// 事件属性编辑面板
    /// </summary>
    public class EventPropertyPanel
    {
        private SpineEventEditor _eventEditor;
        private FrameEvent _selectedEvent;
        private Rectangle _bounds;
        private Texture2D _background;
        private SpriteFont _font;

        /// <summary>
        /// 获取当前选中的事件
        /// </summary>
        public FrameEvent SelectedEvent => _selectedEvent;

        // 通用 UI 元素
        private Button _deleteButton;
        private TextBox _nameTextBox;
        private TextBox _timeTextBox;
        private DropdownList _eventTypeDropdown;

        // 普通事件 UI 元素
        private TextBox _intValueTextBox;
        private TextBox _floatValueTextBox;
        private TextBox _stringValueTextBox;

        // 攻击事件 UI 元素
        private TextBox _attackTypeTextBox;
        private TextBox _attackDamageTextBox;
        private DropdownList _shapeTypeDropdown;
        private TextBox _shapeXTextBox;
        private TextBox _shapeYTextBox;
        private TextBox _shapeWidthTextBox;
        private TextBox _shapeHeightTextBox;
        private TextBox _shapeRotationTextBox;

        // 特效事件 UI 元素
        private TextBox _effectNameTextBox;
        private TextBox _effectXTextBox;
        private TextBox _effectYTextBox;
        private TextBox _effectScaleTextBox;

        // 声音事件 UI 元素
        private TextBox _soundNameTextBox;
        private TextBox _soundVolumeTextBox;
        private TextBox _soundPitchTextBox;

        // 添加事件按钮
        private Button _addEventButton;

        /// <summary>
        /// 创建事件属性编辑面板
        /// </summary>
        /// <param name="eventEditor">Spine 事件编辑器</param>
        /// <param name="graphicsDevice">图形设备</param>
        /// <param name="font">字体</param>
        public EventPropertyPanel(SpineEventEditor eventEditor, GraphicsDevice graphicsDevice, SpriteFont font)
        {
            _eventEditor = eventEditor;
            _font = font;

            // 创建背景 - 使用更亮的颜色
            _background = new Texture2D(graphicsDevice, 1, 1);
            _background.SetData(new[] { new Color(40, 40, 40) });

            // 创建添加事件按钮
            _addEventButton = new Button(graphicsDevice, "Add Event", new Rectangle(0, 0, 120, 30));

            // 创建通用 UI 元素
            _deleteButton = new Button(graphicsDevice, "Delete", new Rectangle(0, 0, 100, 30));
            _nameTextBox = new TextBox(graphicsDevice, "Name", "", new Rectangle(0, 0, 200, 30));
            _timeTextBox = new TextBox(graphicsDevice, "Time (sec)", "0", new Rectangle(0, 0, 200, 30));

            // 创建事件类型下拉列表
            List<string> eventTypes = new List<string>
            {
                "Normal",
                "Attack",
                "Effect",
                "Sound"
            };
            _eventTypeDropdown = new DropdownList(graphicsDevice, _font, "Event Type", eventTypes, new Rectangle(0, 0, 200, 30));

            // 创建普通事件 UI 元素
            _intValueTextBox = new TextBox(graphicsDevice, "Int Value", "0", new Rectangle(0, 0, 200, 30));
            _floatValueTextBox = new TextBox(graphicsDevice, "Float Value", "0", new Rectangle(0, 0, 200, 30));
            _stringValueTextBox = new TextBox(graphicsDevice, "String Value", "", new Rectangle(0, 0, 200, 30));

            // 创建攻击事件 UI 元素
            _attackTypeTextBox = new TextBox(graphicsDevice, "Attack Type", "", new Rectangle(0, 0, 200, 30));
            _attackDamageTextBox = new TextBox(graphicsDevice, "Damage", "0", new Rectangle(0, 0, 200, 30));

            // 创建形状类型下拉列表
            List<string> shapeTypes = new List<string>
            {
                "Rectangle",
                "Circle"
            };
            _shapeTypeDropdown = new DropdownList(graphicsDevice, _font, "Shape Type", shapeTypes, new Rectangle(0, 0, 200, 30));

            // 注意：X和Y坐标是相对于Spine动画原点的
            _shapeXTextBox = new TextBox(graphicsDevice, "X (相对原点)", "0", new Rectangle(0, 0, 200, 30));
            _shapeYTextBox = new TextBox(graphicsDevice, "Y (相对原点)", "0", new Rectangle(0, 0, 200, 30));
            _shapeWidthTextBox = new TextBox(graphicsDevice, "Width/Radius", "0", new Rectangle(0, 0, 200, 30));
            _shapeHeightTextBox = new TextBox(graphicsDevice, "Height", "0", new Rectangle(0, 0, 200, 30));
            _shapeRotationTextBox = new TextBox(graphicsDevice, "Rotation", "0", new Rectangle(0, 0, 200, 30));

            // 创建特效事件 UI 元素
            _effectNameTextBox = new TextBox(graphicsDevice, "Effect Name", "", new Rectangle(0, 0, 200, 30));
            _effectXTextBox = new TextBox(graphicsDevice, "X", "0", new Rectangle(0, 0, 200, 30));
            _effectYTextBox = new TextBox(graphicsDevice, "Y", "0", new Rectangle(0, 0, 200, 30));
            _effectScaleTextBox = new TextBox(graphicsDevice, "Scale", "1.0", new Rectangle(0, 0, 200, 30));

            // 创建声音事件 UI 元素
            _soundNameTextBox = new TextBox(graphicsDevice, "Sound Name", "", new Rectangle(0, 0, 200, 30));
            _soundVolumeTextBox = new TextBox(graphicsDevice, "Volume", "1.0", new Rectangle(0, 0, 200, 30));
            _soundPitchTextBox = new TextBox(graphicsDevice, "Pitch", "1.0", new Rectangle(0, 0, 200, 30));

            // 设置添加事件按钮事件处理
            _addEventButton.Click += (sender, e) => {
                // 在当前时间点添加新事件
                float currentTime = _eventEditor.CurrentTime;
                FrameEvent newEvent = new FrameEvent("New Event", currentTime, 0, 0, "");
                _eventEditor.AddEvent(newEvent.Name, newEvent.Time);

                // 选中新添加的事件
                _selectedEvent = newEvent;

                // 更新UI
                UpdateUIForEventType();
            };

            // 设置删除事件按钮事件处理
            _deleteButton.Click += (sender, e) => {
                if (_selectedEvent != null)
                {
                    int index = _eventEditor.Events.IndexOf(_selectedEvent);
                    if (index >= 0)
                    {
                        _eventEditor.RemoveEvent(index);
                        _selectedEvent = null;
                    }
                }
            };

            _nameTextBox.TextChanged += (sender, e) => {
                if (_selectedEvent != null)
                {
                    _selectedEvent.Name = _nameTextBox.Text;
                }
            };

            _timeTextBox.TextChanged += (sender, e) => {
                if (_selectedEvent != null && float.TryParse(_timeTextBox.Text, out float time))
                {
                    _selectedEvent.Time = MathHelper.Clamp(time, 0, _eventEditor.AnimationDuration);

                    // 重新排序事件
                    _eventEditor.Events.Sort((a, b) => a.Time.CompareTo(b.Time));
                }
            };

            // 事件类型下拉列表事件处理
            _eventTypeDropdown.SelectedIndexChanged += (sender, e) => {
                if (_selectedEvent != null)
                {
                    // 根据选择的事件类型更新事件
                    switch (_eventTypeDropdown.SelectedIndex)
                    {
                        case 0: // Normal
                            _selectedEvent.EventType = EventType.Normal;
                            break;
                        case 1: // Attack
                            _selectedEvent.EventType = EventType.Attack;
                            if (_selectedEvent.Attack == null)
                                _selectedEvent.Attack = new AttackData();
                            break;
                        case 2: // Effect
                            _selectedEvent.EventType = EventType.Effect;
                            if (_selectedEvent.Effect == null)
                                _selectedEvent.Effect = new EffectData();
                            break;
                        case 3: // Sound
                            _selectedEvent.EventType = EventType.Sound;
                            if (_selectedEvent.Sound == null)
                                _selectedEvent.Sound = new SoundData();
                            break;
                    }

                    // 更新 UI 显示
                    UpdateUIForEventType();
                }
            };

            // 普通事件 UI 元素事件处理
            _intValueTextBox.TextChanged += (sender, e) => {
                if (_selectedEvent != null && int.TryParse(_intValueTextBox.Text, out int value))
                {
                    _selectedEvent.IntValue = value;
                }
            };

            _floatValueTextBox.TextChanged += (sender, e) => {
                if (_selectedEvent != null && float.TryParse(_floatValueTextBox.Text, out float value))
                {
                    _selectedEvent.FloatValue = value;
                }
            };

            _stringValueTextBox.TextChanged += (sender, e) => {
                if (_selectedEvent != null)
                {
                    _selectedEvent.StringValue = _stringValueTextBox.Text;
                }
            };

            // 攻击事件 UI 元素事件处理
            _attackTypeTextBox.TextChanged += (sender, e) => {
                if (_selectedEvent != null && _selectedEvent.Attack != null)
                {
                    _selectedEvent.Attack.Type = _attackTypeTextBox.Text;
                }
            };

            _attackDamageTextBox.TextChanged += (sender, e) => {
                if (_selectedEvent != null && _selectedEvent.Attack != null && int.TryParse(_attackDamageTextBox.Text, out int damage))
                {
                    _selectedEvent.Attack.Damage = damage;
                }
            };

            _shapeTypeDropdown.SelectedIndexChanged += (sender, e) => {
                if (_selectedEvent != null && _selectedEvent.Attack != null && _selectedEvent.Attack.Shape != null)
                {
                    _selectedEvent.Attack.Shape.Type = (ShapeType)_shapeTypeDropdown.SelectedIndex;
                    UpdateShapeUIVisibility();
                }
            };

            _shapeXTextBox.TextChanged += (sender, e) => {
                if (_selectedEvent != null && _selectedEvent.Attack != null && _selectedEvent.Attack.Shape != null &&
                    float.TryParse(_shapeXTextBox.Text, out float x))
                {
                    _selectedEvent.Attack.Shape.X = x;
                }
            };

            _shapeYTextBox.TextChanged += (sender, e) => {
                if (_selectedEvent != null && _selectedEvent.Attack != null && _selectedEvent.Attack.Shape != null &&
                    float.TryParse(_shapeYTextBox.Text, out float y))
                {
                    _selectedEvent.Attack.Shape.Y = y;
                }
            };

            _shapeWidthTextBox.TextChanged += (sender, e) => {
                if (_selectedEvent != null && _selectedEvent.Attack != null && _selectedEvent.Attack.Shape != null &&
                    float.TryParse(_shapeWidthTextBox.Text, out float width))
                {
                    _selectedEvent.Attack.Shape.Width = width;
                }
            };

            _shapeHeightTextBox.TextChanged += (sender, e) => {
                if (_selectedEvent != null && _selectedEvent.Attack != null && _selectedEvent.Attack.Shape != null &&
                    float.TryParse(_shapeHeightTextBox.Text, out float height))
                {
                    _selectedEvent.Attack.Shape.Height = height;
                }
            };

            _shapeRotationTextBox.TextChanged += (sender, e) => {
                if (_selectedEvent != null && _selectedEvent.Attack != null && _selectedEvent.Attack.Shape != null &&
                    float.TryParse(_shapeRotationTextBox.Text, out float rotation))
                {
                    _selectedEvent.Attack.Shape.Rotation = rotation;
                }
            };

            // 特效事件 UI 元素事件处理
            _effectNameTextBox.TextChanged += (sender, e) => {
                if (_selectedEvent != null && _selectedEvent.Effect != null)
                {
                    _selectedEvent.Effect.Name = _effectNameTextBox.Text;
                }
            };

            _effectXTextBox.TextChanged += (sender, e) => {
                if (_selectedEvent != null && _selectedEvent.Effect != null &&
                    float.TryParse(_effectXTextBox.Text, out float x))
                {
                    _selectedEvent.Effect.X = x;
                }
            };

            _effectYTextBox.TextChanged += (sender, e) => {
                if (_selectedEvent != null && _selectedEvent.Effect != null &&
                    float.TryParse(_effectYTextBox.Text, out float y))
                {
                    _selectedEvent.Effect.Y = y;
                }
            };

            _effectScaleTextBox.TextChanged += (sender, e) => {
                if (_selectedEvent != null && _selectedEvent.Effect != null &&
                    float.TryParse(_effectScaleTextBox.Text, out float scale))
                {
                    _selectedEvent.Effect.Scale = scale;
                }
            };

            // 声音事件 UI 元素事件处理
            _soundNameTextBox.TextChanged += (sender, e) => {
                if (_selectedEvent != null && _selectedEvent.Sound != null)
                {
                    _selectedEvent.Sound.Name = _soundNameTextBox.Text;
                }
            };

            _soundVolumeTextBox.TextChanged += (sender, e) => {
                if (_selectedEvent != null && _selectedEvent.Sound != null &&
                    float.TryParse(_soundVolumeTextBox.Text, out float volume))
                {
                    _selectedEvent.Sound.Volume = volume;
                }
            };

            _soundPitchTextBox.TextChanged += (sender, e) => {
                if (_selectedEvent != null && _selectedEvent.Sound != null &&
                    float.TryParse(_soundPitchTextBox.Text, out float pitch))
                {
                    _selectedEvent.Sound.Pitch = pitch;
                }
            };
        }

        /// <summary>
        /// 根据事件类型更新 UI 显示
        /// </summary>
        private void UpdateUIForEventType()
        {
            if (_selectedEvent == null)
                return;

            // 隐藏所有特定类型的控件
            HideAllTypeSpecificControls();

            // 设置事件类型下拉列表
            _eventTypeDropdown.SelectedIndex = (int)_selectedEvent.EventType;

            // 根据事件类型显示相应的控件
            switch (_selectedEvent.EventType)
            {
                case EventType.Normal:
                    // 显示普通事件控件
                    _intValueTextBox.Visible = true;
                    _floatValueTextBox.Visible = true;
                    _stringValueTextBox.Visible = true;

                    // 更新控件值
                    _intValueTextBox.Text = _selectedEvent.IntValue.ToString();
                    _floatValueTextBox.Text = _selectedEvent.FloatValue.ToString("0.000");
                    _stringValueTextBox.Text = _selectedEvent.StringValue;
                    break;

                case EventType.Attack:
                    // 确保攻击数据不为空
                    if (_selectedEvent.Attack == null)
                        _selectedEvent.Attack = new AttackData();

                    // 显示攻击事件控件
                    _attackTypeTextBox.Visible = true;
                    _attackDamageTextBox.Visible = true;
                    _shapeTypeDropdown.Visible = true;

                    // 更新控件值
                    _attackTypeTextBox.Text = _selectedEvent.Attack.Type ?? "";
                    _attackDamageTextBox.Text = _selectedEvent.Attack.Damage.ToString();

                    // 确保形状数据不为空
                    if (_selectedEvent.Attack.Shape == null)
                        _selectedEvent.Attack.Shape = new AttackShape();

                    // 设置形状类型
                    _shapeTypeDropdown.SelectedIndex = (int)_selectedEvent.Attack.Shape.Type;

                    // 更新形状控件
                    UpdateShapeUIVisibility();
                    break;

                case EventType.Effect:
                    // 确保特效数据不为空
                    if (_selectedEvent.Effect == null)
                        _selectedEvent.Effect = new EffectData();

                    // 显示特效事件控件
                    _effectNameTextBox.Visible = true;
                    _effectXTextBox.Visible = true;
                    _effectYTextBox.Visible = true;
                    _effectScaleTextBox.Visible = true;

                    // 更新控件值
                    _effectNameTextBox.Text = _selectedEvent.Effect.Name ?? "";
                    _effectXTextBox.Text = _selectedEvent.Effect.X.ToString("0.00");
                    _effectYTextBox.Text = _selectedEvent.Effect.Y.ToString("0.00");
                    _effectScaleTextBox.Text = _selectedEvent.Effect.Scale.ToString("0.00");
                    break;

                case EventType.Sound:
                    // 确保声音数据不为空
                    if (_selectedEvent.Sound == null)
                        _selectedEvent.Sound = new SoundData();

                    // 显示声音事件控件
                    _soundNameTextBox.Visible = true;
                    _soundVolumeTextBox.Visible = true;
                    _soundPitchTextBox.Visible = true;

                    // 更新控件值
                    _soundNameTextBox.Text = _selectedEvent.Sound.Name ?? "";
                    _soundVolumeTextBox.Text = _selectedEvent.Sound.Volume.ToString("0.00");
                    _soundPitchTextBox.Text = _selectedEvent.Sound.Pitch.ToString("0.00");
                    break;
            }
        }

        /// <summary>
        /// 根据形状类型更新形状 UI 控件的可见性
        /// </summary>
        private void UpdateShapeUIVisibility()
        {
            if (_selectedEvent == null || _selectedEvent.Attack == null || _selectedEvent.Attack.Shape == null)
                return;

            // 显示所有形状控件
            _shapeXTextBox.Visible = true;
            _shapeYTextBox.Visible = true;
            _shapeWidthTextBox.Visible = true;

            // 更新控件值
            _shapeXTextBox.Text = _selectedEvent.Attack.Shape.X.ToString("0.00");
            _shapeYTextBox.Text = _selectedEvent.Attack.Shape.Y.ToString("0.00");
            _shapeWidthTextBox.Text = _selectedEvent.Attack.Shape.Width.ToString("0.00");

            // 根据形状类型显示或隐藏特定控件
            if (_selectedEvent.Attack.Shape.Type == ShapeType.Rectangle)
            {
                // 矩形需要高度和旋转
                _shapeHeightTextBox.Visible = true;
                _shapeRotationTextBox.Visible = true;

                // 更新控件值
                _shapeHeightTextBox.Text = _selectedEvent.Attack.Shape.Height.ToString("0.00");
                _shapeRotationTextBox.Text = _selectedEvent.Attack.Shape.Rotation.ToString("0.00");

                // 更新标签
                _shapeWidthTextBox.Label = "Width";
            }
            else // Circle
            {
                // 圆形不需要高度和旋转
                _shapeHeightTextBox.Visible = false;
                _shapeRotationTextBox.Visible = false;

                // 更新标签
                _shapeWidthTextBox.Label = "Radius";
            }
        }

        /// <summary>
        /// 隐藏所有特定类型的控件
        /// </summary>
        private void HideAllTypeSpecificControls()
        {
            // 隐藏普通事件控件
            _intValueTextBox.Visible = false;
            _floatValueTextBox.Visible = false;
            _stringValueTextBox.Visible = false;

            // 隐藏攻击事件控件
            _attackTypeTextBox.Visible = false;
            _attackDamageTextBox.Visible = false;
            _shapeTypeDropdown.Visible = false;
            _shapeXTextBox.Visible = false;
            _shapeYTextBox.Visible = false;
            _shapeWidthTextBox.Visible = false;
            _shapeHeightTextBox.Visible = false;
            _shapeRotationTextBox.Visible = false;

            // 隐藏特效事件控件
            _effectNameTextBox.Visible = false;
            _effectXTextBox.Visible = false;
            _effectYTextBox.Visible = false;
            _effectScaleTextBox.Visible = false;

            // 隐藏声音事件控件
            _soundNameTextBox.Visible = false;
            _soundVolumeTextBox.Visible = false;
            _soundPitchTextBox.Visible = false;
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
                // 更新通用控件
                _nameTextBox.Text = evt.Name;
                _timeTextBox.Text = evt.Time.ToString("0.000");

                // 根据事件类型更新 UI
                UpdateUIForEventType();
            }
            else
            {
                // 隐藏所有特定类型的控件
                HideAllTypeSpecificControls();
            }
        }

        /// <summary>
        /// 更新面板
        /// </summary>
        /// <param name="gameTime">游戏时间</param>
        public void Update(GameTime gameTime)
        {
            // 始终更新添加事件按钮
            _addEventButton.Update();

            // 如果没有选中事件，只更新添加按钮
            if (_selectedEvent == null)
                return;

            // 更新通用 UI 元素
            _deleteButton.Update();
            _nameTextBox.Update(gameTime);
            _timeTextBox.Update(gameTime);
            _eventTypeDropdown.Update();

            // 根据事件类型更新特定的 UI 元素
            switch (_selectedEvent.EventType)
            {
                case EventType.Normal:
                    // 更新普通事件 UI 元素
                    _intValueTextBox.Update(gameTime);
                    _floatValueTextBox.Update(gameTime);
                    _stringValueTextBox.Update(gameTime);
                    break;

                case EventType.Attack:
                    // 更新攻击事件 UI 元素
                    _attackTypeTextBox.Update(gameTime);
                    _attackDamageTextBox.Update(gameTime);
                    _shapeTypeDropdown.Update();
                    _shapeXTextBox.Update(gameTime);
                    _shapeYTextBox.Update(gameTime);
                    _shapeWidthTextBox.Update(gameTime);

                    // 根据形状类型更新特定控件
                    if (_selectedEvent.Attack.Shape.Type == ShapeType.Rectangle)
                    {
                        _shapeHeightTextBox.Update(gameTime);
                        _shapeRotationTextBox.Update(gameTime);
                    }
                    break;

                case EventType.Effect:
                    // 更新特效事件 UI 元素
                    _effectNameTextBox.Update(gameTime);
                    _effectXTextBox.Update(gameTime);
                    _effectYTextBox.Update(gameTime);
                    _effectScaleTextBox.Update(gameTime);
                    break;

                case EventType.Sound:
                    // 更新声音事件 UI 元素
                    _soundNameTextBox.Update(gameTime);
                    _soundVolumeTextBox.Update(gameTime);
                    _soundPitchTextBox.Update(gameTime);
                    break;
            }
        }

        /// <summary>
        /// 绘制面板
        /// </summary>
        /// <param name="spriteBatch">精灵批处理</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            // 绘制背景
            spriteBatch.Draw(_background, _bounds, Color.White);

            // 绘制标题 - 使用更明显的颜色
            spriteBatch.DrawString(_font, "Event Properties", new Vector2(_bounds.X + 10, _bounds.Y + 10), new Color(255, 255, 0)); // 黄色标题

            // 始终绘制添加事件按钮
            _addEventButton.Draw(spriteBatch, _font);

            if (_selectedEvent == null)
            {
                spriteBatch.DrawString(_font, "No event selected", new Vector2(_bounds.X + 10, _bounds.Y + 80), Color.Gray);
                return;
            }

            // 绘制通用 UI 元素
            _deleteButton.Draw(spriteBatch, _font);
            _nameTextBox.Draw(spriteBatch, _font);
            _timeTextBox.Draw(spriteBatch, _font);

            // 先绘制下拉列表的主体部分
            _eventTypeDropdown.Draw(spriteBatch);

            // 根据事件类型绘制特定的 UI 元素
            switch (_selectedEvent.EventType)
            {
                case EventType.Normal:
                    // 绘制普通事件 UI 元素
                    _intValueTextBox.Draw(spriteBatch, _font);
                    _floatValueTextBox.Draw(spriteBatch, _font);
                    _stringValueTextBox.Draw(spriteBatch, _font);
                    break;

                case EventType.Attack:
                    // 绘制攻击事件 UI 元素
                    _attackTypeTextBox.Draw(spriteBatch, _font);
                    _attackDamageTextBox.Draw(spriteBatch, _font);
                    _shapeTypeDropdown.Draw(spriteBatch);
                    _shapeXTextBox.Draw(spriteBatch, _font);
                    _shapeYTextBox.Draw(spriteBatch, _font);
                    _shapeWidthTextBox.Draw(spriteBatch, _font);

                    // 根据形状类型绘制特定控件
                    if (_selectedEvent.Attack.Shape.Type == ShapeType.Rectangle)
                    {
                        _shapeHeightTextBox.Draw(spriteBatch, _font);
                        _shapeRotationTextBox.Draw(spriteBatch, _font);
                    }
                    break;

                case EventType.Effect:
                    // 绘制特效事件 UI 元素
                    _effectNameTextBox.Draw(spriteBatch, _font);
                    _effectXTextBox.Draw(spriteBatch, _font);
                    _effectYTextBox.Draw(spriteBatch, _font);
                    _effectScaleTextBox.Draw(spriteBatch, _font);
                    break;

                case EventType.Sound:
                    // 绘制声音事件 UI 元素
                    _soundNameTextBox.Draw(spriteBatch, _font);
                    _soundVolumeTextBox.Draw(spriteBatch, _font);
                    _soundPitchTextBox.Draw(spriteBatch, _font);
                    break;
            }

            // 如果下拉列表展开，再次绘制它以确保它显示在最上层
            if (_eventTypeDropdown.IsExpanded)
            {
                _eventTypeDropdown.Draw(spriteBatch);
            }

            // 如果形状类型下拉列表展开，也确保它显示在最上层
            if (_shapeTypeDropdown.IsExpanded)
            {
                _shapeTypeDropdown.Draw(spriteBatch);
            }

        }

        /// <summary>
        /// 设置面板的边界
        /// </summary>
        /// <param name="bounds">边界矩形</param>
        public void SetBounds(Rectangle bounds)
        {
            _bounds = bounds;

            // 更新通用 UI 元素的位置
            int y = _bounds.Y + 40;

            // 添加事件按钮和删除按钮并排放置
            int buttonWidth = 120;
            int buttonSpacing = 10;
            _addEventButton.Bounds = new Rectangle(_bounds.X + 10, y, buttonWidth, 30);
            _deleteButton.Bounds = new Rectangle(_bounds.X + buttonWidth + buttonSpacing + 10, y, buttonWidth, 30);

            y += 40;
            _nameTextBox.Bounds = new Rectangle(_bounds.X + 10, y, _bounds.Width - 20, 30);
            y += 60;
            _timeTextBox.Bounds = new Rectangle(_bounds.X + 10, y, _bounds.Width - 20, 30);
            y += 60;
            _eventTypeDropdown.Bounds = new Rectangle(_bounds.X + 10, y, _bounds.Width - 20, 30);
            y += 60;

            // 更新普通事件 UI 元素的位置
            _intValueTextBox.Bounds = new Rectangle(_bounds.X + 10, y, _bounds.Width - 20, 30);
            y += 60;
            _floatValueTextBox.Bounds = new Rectangle(_bounds.X + 10, y, _bounds.Width - 20, 30);
            y += 60;
            _stringValueTextBox.Bounds = new Rectangle(_bounds.X + 10, y, _bounds.Width - 20, 30);

            // 更新攻击事件 UI 元素的位置
            y = _bounds.Y + 220; // 重置 y 坐标
            _attackTypeTextBox.Bounds = new Rectangle(_bounds.X + 10, y, _bounds.Width - 20, 30);
            y += 60;
            _attackDamageTextBox.Bounds = new Rectangle(_bounds.X + 10, y, _bounds.Width - 20, 30);
            y += 60;
            _shapeTypeDropdown.Bounds = new Rectangle(_bounds.X + 10, y, _bounds.Width - 20, 30);
            y += 60;
            _shapeXTextBox.Bounds = new Rectangle(_bounds.X + 10, y, _bounds.Width - 20, 30);
            y += 60;
            _shapeYTextBox.Bounds = new Rectangle(_bounds.X + 10, y, _bounds.Width - 20, 30);
            y += 60;
            _shapeWidthTextBox.Bounds = new Rectangle(_bounds.X + 10, y, _bounds.Width - 20, 30);
            y += 60;
            _shapeHeightTextBox.Bounds = new Rectangle(_bounds.X + 10, y, _bounds.Width - 20, 30);
            y += 60;
            _shapeRotationTextBox.Bounds = new Rectangle(_bounds.X + 10, y, _bounds.Width - 20, 30);

            // 更新特效事件 UI 元素的位置
            y = _bounds.Y + 220; // 重置 y 坐标
            _effectNameTextBox.Bounds = new Rectangle(_bounds.X + 10, y, _bounds.Width - 20, 30);
            y += 60;
            _effectXTextBox.Bounds = new Rectangle(_bounds.X + 10, y, _bounds.Width - 20, 30);
            y += 60;
            _effectYTextBox.Bounds = new Rectangle(_bounds.X + 10, y, _bounds.Width - 20, 30);
            y += 60;
            _effectScaleTextBox.Bounds = new Rectangle(_bounds.X + 10, y, _bounds.Width - 20, 30);

            // 更新声音事件 UI 元素的位置
            y = _bounds.Y + 220; // 重置 y 坐标
            _soundNameTextBox.Bounds = new Rectangle(_bounds.X + 10, y, _bounds.Width - 20, 30);
            y += 60;
            _soundVolumeTextBox.Bounds = new Rectangle(_bounds.X + 10, y, _bounds.Width - 20, 30);
            y += 60;
            _soundPitchTextBox.Bounds = new Rectangle(_bounds.X + 10, y, _bounds.Width - 20, 30);
        }
    }
}
