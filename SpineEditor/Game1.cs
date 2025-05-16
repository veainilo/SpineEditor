using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using Spine;

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
            // 检查文件是否存在
            string atlasPath = Path.Combine(Content.RootDirectory, "spine", "tianshen.atlas");
            string skelPath = Path.Combine(Content.RootDirectory, "spine", "tianshen.skel");

            System.Console.WriteLine($"检查文件路径: {atlasPath}");
            System.Console.WriteLine($"文件是否存在: {File.Exists(atlasPath)}");
            System.Console.WriteLine($"检查文件路径: {skelPath}");
            System.Console.WriteLine($"文件是否存在: {File.Exists(skelPath)}");

            if (!File.Exists(atlasPath) || !File.Exists(skelPath))
            {
                System.Console.WriteLine("Spine 文件不存在，请确保文件已正确放置");
                return;
            }

            // 创建 Atlas
            System.Console.WriteLine("开始创建 Atlas...");

            // 检查纹理文件是否存在
            string texturePath = Path.Combine(Content.RootDirectory, "spine", "tianshen.png");
            System.Console.WriteLine($"检查纹理文件: {texturePath}");
            System.Console.WriteLine($"纹理文件是否存在: {File.Exists(texturePath)}");

            // 尝试直接加载纹理
            try
            {
                Texture2D texture = Texture2D.FromFile(GraphicsDevice, texturePath);
                System.Console.WriteLine($"纹理加载成功: {texture.Width}x{texture.Height}");
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"纹理加载失败: {ex.Message}");
            }

            // 创建自定义的纹理加载器
            XnaTextureLoader textureLoader = new XnaTextureLoader(GraphicsDevice);
            Atlas atlas = new Atlas(atlasPath, textureLoader);
            System.Console.WriteLine("Atlas 创建成功");

            // 尝试使用不同的方法加载骨骼数据
            System.Console.WriteLine("开始创建 SkeletonBinary...");
            SkeletonBinary binary = new SkeletonBinary(atlas);
            binary.Scale = 0.5f; // 调整缩放比例

            // 尝试使用 ReadSkeletonDataFile 方法
            System.Console.WriteLine("开始读取 SkeletonData...");
            SkeletonData skeletonData = null;

            try
            {
                // 尝试方法 1：使用 ReadSkeletonData
                System.Console.WriteLine("尝试方法 1: ReadSkeletonData");
                skeletonData = binary.ReadSkeletonData(skelPath);
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"方法 1 失败: {ex.Message}");

                try
                {
                    // 尝试方法 2：使用文件流
                    System.Console.WriteLine("尝试方法 2: 使用文件流");
                    using (FileStream input = new FileStream(skelPath, FileMode.Open, FileAccess.Read))
                    {
                        skeletonData = binary.ReadSkeletonData(input);
                    }
                }
                catch (System.Exception ex2)
                {
                    System.Console.WriteLine($"方法 2 失败: {ex2.Message}");

                    try
                    {
                        // 尝试方法 3：使用 SkeletonJson
                        System.Console.WriteLine("尝试方法 3: 使用 SkeletonJson");
                        // 创建一个临时的 JSON 文件
                        string jsonPath = Path.Combine(Content.RootDirectory, "spine", "tianshen.json");
                        if (!File.Exists(jsonPath))
                        {
                            System.Console.WriteLine("JSON 文件不存在，无法使用方法 3");
                            throw new System.Exception("无法加载骨骼数据");
                        }

                        SkeletonJson json = new SkeletonJson(atlas);
                        json.Scale = 0.5f;
                        skeletonData = json.ReadSkeletonData(jsonPath);
                    }
                    catch (System.Exception ex3)
                    {
                        System.Console.WriteLine($"方法 3 失败: {ex3.Message}");
                        throw new System.Exception("所有方法都失败了");
                    }
                }
            }

            if (skeletonData == null)
            {
                throw new System.Exception("无法加载骨骼数据");
            }

            System.Console.WriteLine("SkeletonData 加载成功");

            // 创建 Skeleton 和设置位置
            System.Console.WriteLine("开始创建 Skeleton...");
            _skeleton = new Skeleton(skeletonData);
            _skeleton.X = GraphicsDevice.Viewport.Width / 2;
            _skeleton.Y = GraphicsDevice.Viewport.Height / 2;
            System.Console.WriteLine($"Skeleton 创建成功，位置: ({_skeleton.X}, {_skeleton.Y})");

            // 创建动画状态
            System.Console.WriteLine("开始创建 AnimationStateData...");
            AnimationStateData stateData = new AnimationStateData(skeletonData);
            System.Console.WriteLine("开始创建 AnimationState...");
            _animationState = new AnimationState(stateData);
            System.Console.WriteLine("AnimationState 创建成功");

            // 获取第一个动画名称并播放
            System.Console.WriteLine($"动画数量: {skeletonData.Animations.Count}");
            if (skeletonData.Animations.Count > 0)
            {
                System.Console.WriteLine("获取第一个动画名称...");
                string firstAnimation = skeletonData.Animations.Items[0].Name;
                System.Console.WriteLine($"设置动画: {firstAnimation}");
                _animationState.SetAnimation(0, firstAnimation, true);
                System.Console.WriteLine($"播放动画: {firstAnimation}");

                // 打印所有动画名称
                System.Console.WriteLine("所有动画:");
                for (int i = 0; i < skeletonData.Animations.Count; i++)
                {
                    System.Console.WriteLine($"  {i}: {skeletonData.Animations.Items[i].Name}");
                }
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
            try
            {
                _spriteBatch.Begin();

                try
                {
                    // 打印 Skeleton 状态
                    if (_currentTime % 5 < 0.1f) // 每 5 秒打印一次
                    {
                        System.Console.WriteLine($"Skeleton 状态: X={_skeleton.X}, Y={_skeleton.Y}");
                        System.Console.WriteLine($"Viewport: Width={GraphicsDevice.Viewport.Width}, Height={GraphicsDevice.Viewport.Height}");
                    }

                    _skeletonRenderer.Begin();
                    _skeletonRenderer.Draw(_skeleton);
                    _skeletonRenderer.End();
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine($"渲染 Spine 动画时出错: {ex.Message}");
                    if (ex.InnerException != null)
                        System.Console.WriteLine($"内部错误: {ex.InnerException.Message}");
                    System.Console.WriteLine($"堆栈跟踪: {ex.StackTrace}");
                }

                _spriteBatch.End();
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"SpriteBatch 操作时出错: {ex.Message}");
            }
        }
        else
        {
            if (_skeleton == null)
                System.Console.WriteLine("Skeleton 为 null");
            if (_skeletonRenderer == null)
                System.Console.WriteLine("SkeletonRenderer 为 null");
        }

        base.Draw(gameTime);
    }
}
