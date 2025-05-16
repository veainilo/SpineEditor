using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

        // UI 元素
        private Button _deleteButton;
        private TextBox _nameTextBox;
        private TextBox _timeTextBox;
        private TextBox _intValueTextBox;
        private TextBox _floatValueTextBox;
        private TextBox _stringValueTextBox;

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

            // 创建背景
            _background = new Texture2D(graphicsDevice, 1, 1);
            _background.SetData(new[] { new Color(30, 30, 30) });

            // 创建 UI 元素
            _deleteButton = new Button(graphicsDevice, "Delete", new Rectangle(0, 0, 100, 30));
            _nameTextBox = new TextBox(graphicsDevice, "Name", "", new Rectangle(0, 0, 200, 30));
            _timeTextBox = new TextBox(graphicsDevice, "Time (sec)", "0", new Rectangle(0, 0, 200, 30));
            _intValueTextBox = new TextBox(graphicsDevice, "Int Value", "0", new Rectangle(0, 0, 200, 30));
            _floatValueTextBox = new TextBox(graphicsDevice, "Float Value", "0", new Rectangle(0, 0, 200, 30));
            _stringValueTextBox = new TextBox(graphicsDevice, "String Value", "", new Rectangle(0, 0, 200, 30));

            // 设置事件处理
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
                _nameTextBox.Text = evt.Name;
                _timeTextBox.Text = evt.Time.ToString("0.000");
                _intValueTextBox.Text = evt.IntValue.ToString();
                _floatValueTextBox.Text = evt.FloatValue.ToString("0.000");
                _stringValueTextBox.Text = evt.StringValue;
            }
        }

        /// <summary>
        /// 更新面板
        /// </summary>
        /// <param name="gameTime">游戏时间</param>
        public void Update(GameTime gameTime)
        {
            // 更新 UI 元素
            _deleteButton.Update();
            _nameTextBox.Update(gameTime);
            _timeTextBox.Update(gameTime);
            _intValueTextBox.Update(gameTime);
            _floatValueTextBox.Update(gameTime);
            _stringValueTextBox.Update(gameTime);
        }

        /// <summary>
        /// 绘制面板
        /// </summary>
        /// <param name="spriteBatch">精灵批处理</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            // 绘制背景
            spriteBatch.Draw(_background, _bounds, Color.White);

            // 绘制标题
            spriteBatch.DrawString(_font, "Event Properties", new Vector2(_bounds.X + 10, _bounds.Y + 10), Color.White);

            if (_selectedEvent == null)
            {
                spriteBatch.DrawString(_font, "No event selected", new Vector2(_bounds.X + 10, _bounds.Y + 40), Color.Gray);
                return;
            }

            // 绘制 UI 元素
            _deleteButton.Draw(spriteBatch, _font);
            _nameTextBox.Draw(spriteBatch, _font);
            _timeTextBox.Draw(spriteBatch, _font);
            _intValueTextBox.Draw(spriteBatch, _font);
            _floatValueTextBox.Draw(spriteBatch, _font);
            _stringValueTextBox.Draw(spriteBatch, _font);
        }

        /// <summary>
        /// 设置面板的边界
        /// </summary>
        /// <param name="bounds">边界矩形</param>
        public void SetBounds(Rectangle bounds)
        {
            _bounds = bounds;

            // 更新 UI 元素的位置
            int y = _bounds.Y + 40;
            _deleteButton.Bounds = new Rectangle(_bounds.X + 10, y, 100, 30);
            y += 40;
            _nameTextBox.Bounds = new Rectangle(_bounds.X + 10, y, _bounds.Width - 20, 30);
            y += 60;
            _timeTextBox.Bounds = new Rectangle(_bounds.X + 10, y, _bounds.Width - 20, 30);
            y += 60;
            _intValueTextBox.Bounds = new Rectangle(_bounds.X + 10, y, _bounds.Width - 20, 30);
            y += 60;
            _floatValueTextBox.Bounds = new Rectangle(_bounds.X + 10, y, _bounds.Width - 20, 30);
            y += 60;
            _stringValueTextBox.Bounds = new Rectangle(_bounds.X + 10, y, _bounds.Width - 20, 30);
        }
    }
}
