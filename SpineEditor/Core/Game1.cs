using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using Spine;
using SpineEditor.Animation;

namespace SpineEditor.Core
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Spine 动画播放器
        private SpineAnimationPlayer _spinePlayer;
        private float _currentTime;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // 创建 Spine 动画播放器
            _spinePlayer = new SpineAnimationPlayer(GraphicsDevice);

            // 加载 Spine 动画
            string atlasPath = Path.Combine(Content.RootDirectory, "spine", "tianshen.atlas");
            string skelPath = Path.Combine(Content.RootDirectory, "spine", "tianshen.skel");

            // 加载动画，设置缩放为 0.5，位置在屏幕中央偏下
            bool success = _spinePlayer.LoadAnimation(
                atlasPath,
                skelPath,
                0.5f,
                new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height * 0.75f)
            );

            if (success)
            {
                // 获取所有可用的动画名称
                string[] animations = _spinePlayer.AnimationNames;

                // 打印所有动画名称
                System.Console.WriteLine("可用动画:");
                for (int i = 0; i < animations.Length; i++)
                {
                    System.Console.WriteLine($"  {i}: {animations[i]}");
                }

                // 播放第一个动画
                if (animations.Length > 0)
                {
                    _spinePlayer.PlayAnimation(animations[0], true);
                    System.Console.WriteLine($"播放动画: {animations[0]}");
                }
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // 更新 Spine 动画
            // 将毫秒转换为秒
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;
            _currentTime += deltaTime;

            // 更新 Spine 动画
            _spinePlayer?.Update(deltaTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // 绘制 Spine 动画
            _spinePlayer?.Draw();

            base.Draw(gameTime);
        }
    }
}
