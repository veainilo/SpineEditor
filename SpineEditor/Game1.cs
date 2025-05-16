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
        _skeletonRenderer = new SkeletonRenderer(GraphicsDevice);
        _skeletonRenderer.PremultipliedAlpha = true;

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // 加载 Spine 动画
        try
        {
            // 创建 Atlas
            Atlas atlas = new Atlas("Content/spine/tianshen.atlas", new XnaTextureLoader(GraphicsDevice));

            // 使用二进制 skel 文件加载骨骼数据
            SkeletonBinary binary = new SkeletonBinary(atlas);
            binary.Scale = 0.5f; // 调整缩放比例
            SkeletonData skeletonData = binary.ReadSkeletonData("Content/spine/tianshen.skel");

            // 创建 Skeleton 和设置位置
            _skeleton = new Skeleton(skeletonData);
            _skeleton.X = GraphicsDevice.Viewport.Width / 2;
            _skeleton.Y = GraphicsDevice.Viewport.Height / 2;

            // 创建动画状态
            AnimationStateData stateData = new AnimationStateData(skeletonData);
            _animationState = new AnimationState(stateData);

            // 获取第一个动画名称并播放
            if (skeletonData.Animations.Count > 0)
            {
                string firstAnimation = skeletonData.Animations.Items[0].Name;
                _animationState.SetAnimation(0, firstAnimation, true);
                System.Console.WriteLine($"播放动画: {firstAnimation}");
            }
            else
            {
                System.Console.WriteLine("没有找到动画");
            }
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine($"加载 Spine 动画时出错: {ex.Message}");
            if (ex.InnerException != null)
                System.Console.WriteLine($"内部错误: {ex.InnerException.Message}");
        }
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // 更新 Spine 动画
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _currentTime += deltaTime;

        // 更新 Spine 动画
        if (_animationState != null && _skeleton != null)
        {
            _animationState.Update(deltaTime);
            _animationState.Apply(_skeleton);
            // 不同版本的 Spine API 可能有不同的参数
            // _skeleton.UpdateWorldTransform();
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // 绘制 Spine 动画
        if (_skeleton != null && _skeletonRenderer != null)
        {
            _spriteBatch.Begin();

            try
            {
                _skeletonRenderer.Begin();
                _skeletonRenderer.Draw(_skeleton);
                _skeletonRenderer.End();
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"渲染 Spine 动画时出错: {ex.Message}");
            }

            _spriteBatch.End();
        }

        base.Draw(gameTime);
    }
}
