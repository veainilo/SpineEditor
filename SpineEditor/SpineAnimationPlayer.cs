using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spine;
using System;
using System.IO;

namespace SpineEditor
{
    /// <summary>
    /// Spine 动画播放器类，用于加载和渲染 Spine 动画
    /// </summary>
    public class SpineAnimationPlayer
    {
        // Spine 相关变量
        private SkeletonRenderer _skeletonRenderer;
        private Skeleton _skeleton;
        private AnimationState _animationState;
        private GraphicsDevice _graphicsDevice;

        // 动画信息
        private string _currentAnimation;
        private bool _loop;
        private float _scale = 1.0f;
        private Vector2 _position;

        /// <summary>
        /// 获取或设置动画的位置
        /// </summary>
        public Vector2 Position
        {
            get => _position;
            set
            {
                _position = value;
                if (_skeleton != null)
                {
                    _skeleton.X = _position.X;
                    _skeleton.Y = _position.Y;
                }
            }
        }

        /// <summary>
        /// 获取或设置动画的缩放比例
        /// </summary>
        public float Scale
        {
            get => _scale;
            set
            {
                _scale = value;
                if (_skeleton != null)
                {
                    _skeleton.ScaleX = _scale;
                    _skeleton.ScaleY = _scale;
                }
            }
        }

        /// <summary>
        /// 获取当前播放的动画名称
        /// </summary>
        public string CurrentAnimation => _currentAnimation;

        /// <summary>
        /// 获取骨架对象
        /// </summary>
        public Skeleton Skeleton => _skeleton;

        /// <summary>
        /// 获取动画状态对象
        /// </summary>
        public AnimationState AnimationState => _animationState;

        /// <summary>
        /// 获取所有可用的动画名称
        /// </summary>
        public string[] AnimationNames
        {
            get
            {
                if (_skeleton == null || _skeleton.Data == null || _skeleton.Data.Animations == null)
                    return new string[0];

                string[] names = new string[_skeleton.Data.Animations.Count];
                for (int i = 0; i < _skeleton.Data.Animations.Count; i++)
                {
                    names[i] = _skeleton.Data.Animations.Items[i].Name;
                }
                return names;
            }
        }

        /// <summary>
        /// 创建 Spine 动画播放器
        /// </summary>
        /// <param name="graphicsDevice">图形设备</param>
        public SpineAnimationPlayer(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            InitializeSkeletonRenderer();
        }

        /// <summary>
        /// 初始化 SkeletonRenderer
        /// </summary>
        private void InitializeSkeletonRenderer()
        {
            try
            {
                _skeletonRenderer = new SkeletonRenderer(_graphicsDevice);
                _skeletonRenderer.PremultipliedAlpha = false;

                // 设置基本效果的投影矩阵
                BasicEffect effect = (BasicEffect)_skeletonRenderer.Effect;
                effect.World = Matrix.Identity;
                effect.View = Matrix.CreateLookAt(new Vector3(0.0f, 0.0f, 1.0f), Vector3.Zero, Vector3.Up);
                effect.TextureEnabled = true;
                effect.VertexColorEnabled = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"初始化 SkeletonRenderer 时出错: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"内部错误: {ex.InnerException.Message}");
            }
        }

        /// <summary>
        /// 加载 Spine 动画
        /// </summary>
        /// <param name="atlasPath">Atlas 文件路径</param>
        /// <param name="skeletonPath">Skeleton 文件路径 (.skel 或 .json)</param>
        /// <param name="scale">缩放比例</param>
        /// <param name="position">位置</param>
        /// <returns>是否加载成功</returns>
        public bool LoadAnimation(string atlasPath, string skeletonPath, float scale = 1.0f, Vector2? position = null)
        {
            try
            {
                // 检查文件是否存在
                if (!File.Exists(atlasPath) || !File.Exists(skeletonPath))
                {
                    Console.WriteLine($"文件不存在: {atlasPath} 或 {skeletonPath}");
                    return false;
                }

                // 创建 Atlas
                XnaTextureLoader textureLoader = new XnaTextureLoader(_graphicsDevice);
                Atlas atlas = new Atlas(atlasPath, textureLoader);

                // 加载骨骼数据
                SkeletonData skeletonData = null;
                string extension = Path.GetExtension(skeletonPath).ToLower();

                if (extension == ".skel")
                {
                    // 使用二进制格式加载
                    SkeletonBinary binary = new SkeletonBinary(atlas);
                    binary.Scale = scale;
                    skeletonData = binary.ReadSkeletonData(skeletonPath);
                }
                else if (extension == ".json")
                {
                    // 使用 JSON 格式加载
                    SkeletonJson json = new SkeletonJson(atlas);
                    json.Scale = scale;
                    skeletonData = json.ReadSkeletonData(skeletonPath);
                }
                else
                {
                    Console.WriteLine($"不支持的文件格式: {extension}");
                    return false;
                }

                // 创建 Skeleton 和设置位置
                _skeleton = new Skeleton(skeletonData);
                _scale = scale;
                _skeleton.ScaleX = _scale;
                _skeleton.ScaleY = _scale;

                // 设置位置
                _position = position ?? new Vector2(_graphicsDevice.Viewport.Width / 2, _graphicsDevice.Viewport.Height / 2);
                _skeleton.X = _position.X;
                _skeleton.Y = _position.Y;

                // 创建动画状态
                AnimationStateData stateData = new AnimationStateData(skeletonData);
                _animationState = new AnimationState(stateData);

                // 打印所有动画名称
                Console.WriteLine("可用动画:");
                for (int i = 0; i < skeletonData.Animations.Count; i++)
                {
                    Console.WriteLine($"  {i}: {skeletonData.Animations.Items[i].Name}");
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"加载 Spine 动画时出错: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"内部错误: {ex.InnerException.Message}");
                return false;
            }
        }

        /// <summary>
        /// 播放指定的动画
        /// </summary>
        /// <param name="animationName">动画名称</param>
        /// <param name="loop">是否循环播放</param>
        /// <returns>是否成功播放</returns>
        public bool PlayAnimation(string animationName, bool loop = true)
        {
            if (_animationState == null || _skeleton == null)
                return false;

            try
            {
                _animationState.SetAnimation(0, animationName, loop);
                _currentAnimation = animationName;
                _loop = loop;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"播放动画 {animationName} 时出错: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 更新动画
        /// </summary>
        /// <param name="deltaTime">时间增量（秒）</param>
        public void Update(float deltaTime)
        {
            if (_animationState == null || _skeleton == null)
                return;

            // 更新动画状态并应用到骨骼
            _animationState.Update(deltaTime);
            _animationState.Apply(_skeleton);

            // 更新骨骼的世界变换
            _skeleton.UpdateWorldTransform();
        }

        /// <summary>
        /// 渲染动画
        /// </summary>
        public void Draw()
        {
            if (_skeleton == null || _skeletonRenderer == null)
                return;

            try
            {
                // 设置投影矩阵
                ((BasicEffect)_skeletonRenderer.Effect).Projection = Matrix.CreateOrthographicOffCenter(
                    0, _graphicsDevice.Viewport.Width,
                    _graphicsDevice.Viewport.Height, 0,
                    1, 0);

                // 渲染骨架
                _skeletonRenderer.Begin();
                _skeletonRenderer.Draw(_skeleton);
                _skeletonRenderer.End();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"渲染 Spine 动画时出错: {ex.Message}");
            }
        }
    }
}
