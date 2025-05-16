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

    // 圆形纹理
    private Texture2D _circleTexture;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // 初始化 Spine 渲染器
        try
        {
            System.Console.WriteLine("初始化 SkeletonRenderer...");
            _skeletonRenderer = new SkeletonRenderer(GraphicsDevice);

            // 设置为 false，与示例代码保持一致
            _skeletonRenderer.PremultipliedAlpha = false;

            // 设置基本效果的投影矩阵
            BasicEffect effect = (BasicEffect)_skeletonRenderer.Effect;
            effect.World = Matrix.Identity;
            effect.View = Matrix.CreateLookAt(new Vector3(0.0f, 0.0f, 1.0f), Vector3.Zero, Vector3.Up);
            effect.TextureEnabled = true;
            effect.VertexColorEnabled = true;

            System.Console.WriteLine("SkeletonRenderer 初始化成功");
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine($"初始化 SkeletonRenderer 时出错: {ex.Message}");
            if (ex.InnerException != null)
                System.Console.WriteLine($"内部错误: {ex.InnerException.Message}");
        }

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

            // 设置位置在屏幕中央，但稍微向上一点，以便看到完整的角色
            _skeleton.X = GraphicsDevice.Viewport.Width / 2;
            _skeleton.Y = GraphicsDevice.Viewport.Height * 0.75f; // 将 Y 位置设置为屏幕高度的 75%

            // 设置缩放
            _skeleton.ScaleX = 0.5f;
            _skeleton.ScaleY = 0.5f;

            System.Console.WriteLine($"Skeleton 创建成功，位置: ({_skeleton.X}, {_skeleton.Y})，缩放: ({_skeleton.ScaleX}, {_skeleton.ScaleY})");

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
        // 将毫秒转换为秒
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;
        _currentTime += deltaTime;

        // 更新 Spine 动画，与示例代码保持一致
        if (_animationState != null && _skeleton != null)
        {
            // 更新动画状态并应用到骨骼
            _animationState.Update(deltaTime);
            _animationState.Apply(_skeleton);

            // 更新骨骼的世界变换
            _skeleton.UpdateWorldTransform();
        }

        base.Update(gameTime);
    }

    // 创建一个圆形纹理
    private Texture2D CreateCircleTexture(int radius)
    {
        int diameter = radius * 2;
        Texture2D texture = new Texture2D(GraphicsDevice, diameter, diameter);
        Color[] data = new Color[diameter * diameter];

        // 计算圆形
        int centerX = radius;
        int centerY = radius;

        for (int x = 0; x < diameter; x++)
        {
            for (int y = 0; y < diameter; y++)
            {
                int index = x + y * diameter;
                // 计算点到圆心的距离
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(centerX, centerY));

                // 如果距离小于半径，则为圆内部
                if (distance <= radius)
                {
                    // 圆内部为红色
                    data[index] = Color.Red;
                }
                else
                {
                    // 圆外部为透明
                    data[index] = Color.Transparent;
                }
            }
        }

        texture.SetData(data);
        return texture;
    }



    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // 创建圆形纹理（如果尚未创建）
        if (_circleTexture == null)
        {
            _circleTexture = CreateCircleTexture(50); // 50像素半径
        }

        // 绘制屏幕中心的圆形
        _spriteBatch.Begin();

        // 绘制圆形在屏幕中心
        int screenCenterX = GraphicsDevice.Viewport.Width / 2;
        int screenCenterY = GraphicsDevice.Viewport.Height / 2;

        // 绘制圆形
        _spriteBatch.Draw(_circleTexture,
            new Vector2(screenCenterX - _circleTexture.Width / 2, screenCenterY - _circleTexture.Height / 2),
            Color.White);

        // 绘制十字线
        Texture2D pixel = new Texture2D(GraphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.White });

        // 水平线
        _spriteBatch.Draw(pixel, new Rectangle(screenCenterX - 100, screenCenterY, 200, 1), Color.White);
        // 垂直线
        _spriteBatch.Draw(pixel, new Rectangle(screenCenterX, screenCenterY - 100, 1, 200), Color.White);

        // 显示坐标文本
        _spriteBatch.End();

        // 绘制 Spine 动画
        if (_skeleton != null && _skeletonRenderer != null)
        {
            try
            {
                // 打印 Skeleton 状态
                if (_currentTime % 5 < 0.1f) // 每 5 秒打印一次
                {
                    System.Console.WriteLine($"Skeleton 状态: X={_skeleton.X}, Y={_skeleton.Y}");
                    System.Console.WriteLine($"Viewport: Width={GraphicsDevice.Viewport.Width}, Height={GraphicsDevice.Viewport.Height}");
                }

                // 设置投影矩阵，与示例代码保持一致
                ((BasicEffect)_skeletonRenderer.Effect).Projection = Matrix.CreateOrthographicOffCenter(
                    0, GraphicsDevice.Viewport.Width,
                    GraphicsDevice.Viewport.Height, 0,
                    1, 0);

                // 使用与示例代码相同的渲染方式
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

                // 如果 Spine 渲染失败，绘制一个红色矩形来标记位置
                _spriteBatch.Begin();
                Texture2D redPixel = new Texture2D(GraphicsDevice, 1, 1);
                redPixel.SetData(new[] { Color.Red });
                _spriteBatch.Draw(redPixel, new Rectangle((int)_skeleton.X - 50, (int)_skeleton.Y - 50, 100, 100), Color.Red);
                _spriteBatch.End();
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
