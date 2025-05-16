using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Spine;
using System.IO;

namespace SpineEditor;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    // Spine 相关变量
    private SkeletonRenderer _skeletonRenderer;
    private Skeleton _skeleton;
    private AnimationState _animationState;
    private float _currentTime;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // 初始化 Spine 渲染器
        // 注意：在实际使用时，您需要取消注释这些代码
        // _skeletonRenderer = new SkeletonRenderer(GraphicsDevice);
        // _skeletonRenderer.PremultipliedAlpha = true;

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // 当您有 Spine 动画资源时，可以取消注释以下代码
        // 加载 Spine 动画
        /*
        // 创建 Atlas 和 SkeletonData
        Atlas atlas = new Atlas("Content/spine/spineboy.atlas", new XnaTextureLoader(GraphicsDevice));
        SkeletonJson json = new SkeletonJson(atlas);
        json.Scale = 1.0f; // 可以调整缩放比例
        SkeletonData skeletonData = json.ReadSkeletonData("Content/spine/spineboy.json");

        // 创建 Skeleton 和 AnimationState
        _skeleton = new Skeleton(skeletonData);
        _skeleton.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height);

        AnimationStateData stateData = new AnimationStateData(skeletonData);
        _animationState = new AnimationState(stateData);
        _animationState.SetAnimation(0, "walk", true); // 设置动画，第三个参数表示是否循环
        */
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // 更新 Spine 动画
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _currentTime += deltaTime;

        // 当您有 Spine 动画资源时，可以取消注释以下代码
        /*
        _animationState.Update(deltaTime);
        _animationState.Apply(_skeleton);
        _skeleton.UpdateWorldTransform();
        */

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // 绘制 Spine 动画
        _spriteBatch.Begin();

        // 当您有 Spine 动画资源时，可以取消注释以下代码
        /*
        _skeletonRenderer.Begin();
        _skeletonRenderer.Draw(_skeleton);
        _skeletonRenderer.End();
        */

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
