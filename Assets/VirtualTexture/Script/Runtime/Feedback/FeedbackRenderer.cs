using System;
using UnityEngine;

namespace VirtualTexture
{
	/// <summary>
	/// 预渲染器类.
	/// 预渲染器使用特定的着色器渲染场景，获取当前场景用到的的虚拟贴图相关信息(页表/mipmap等级等)
	/// </summary>
    public class FeedbackRenderer : MonoBehaviour, IFeedbackRenderer
	{
		/// <summary>
		/// 渲染完成的事件回调
		/// </summary>
		public event Action<RenderTexture> OnFeedbackRenderComplete;

		/// <summary>
		/// 渲染目标缩放比例
		/// </summary>
		[SerializeField]
		private ScaleFactor m_Scale = default;

		/// <summary>
		/// 预渲染着色器
		/// </summary>
		[SerializeField]
		private Shader m_FeedbackShader = default;

        /// <summary>
        /// mipmap层级偏移
        /// </summary>
        [SerializeField]
        private int m_MipmapBias = default;

        /// <summary>
        /// 预渲染摄像机
        /// </summary>
        private Camera m_FeedbackCamera;

		/// <summary>
		/// 获取预渲染的贴图
		/// </summary>
        public RenderTexture TargetTexture { get; private set; }

		/// <summary>
		/// 预渲染器统计数据
		/// </summary>
        public FrameStat Stat { get; private set; } = new FrameStat();

        private void Start()
		{
			var tileTexture = GetComponent(typeof(ITiledTexture)) as ITiledTexture;
			var virtualTable = GetComponent(typeof(IPageTable)) as IPageTable;

			// 设置预渲染着色器参数
			// x: 页表大小(单位: 页)
			// y: 虚拟贴图大小(单位: 像素)
			// z: 最大mipmap等级
            Shader.SetGlobalVector(
                "_VTFeedbackParam", 
				new Vector4(virtualTable.TableSize,
				            virtualTable.TableSize * tileTexture.TileSize,
				            virtualTable.MaxMipLevel,
                            m_MipmapBias));

			InitCamera();
        }

        private void OnPreCull()
        {
            var mainCamera = Camera.main;
            if (mainCamera == null)
                return;

			// 处理屏幕尺寸变换
			var width = (int)(mainCamera.pixelWidth * m_Scale.ToFloat());
			var height = (int)(mainCamera.pixelHeight * m_Scale.ToFloat());
			if (TargetTexture == null || TargetTexture.width != width || TargetTexture.height != height)
            {
				TargetTexture = new RenderTexture(width, height, 0);
                TargetTexture.useMipMap = false;
                TargetTexture.wrapMode = TextureWrapMode.Clamp;
                TargetTexture.filterMode = FilterMode.Point;

                m_FeedbackCamera.targetTexture = TargetTexture;
            }

			// 渲染前先拷贝主摄像机的相关参数
			CopyCamera(mainCamera);
        }

        private void OnPreRender()
		{
			Stat.BeginFrame();
        }

        private void OnPostRender()
        {
            if (TargetTexture == null)
                return;

			Stat.EndFrame();

            OnFeedbackRenderComplete?.Invoke(TargetTexture);
		}

		/// <summary>
		/// 初始化摄像机
		/// </summary>
		private void InitCamera()
		{
			m_FeedbackCamera = GetComponent<Camera>();
			if(m_FeedbackCamera == null)
				m_FeedbackCamera = gameObject.AddComponent<Camera>();

			m_FeedbackCamera.allowHDR = false;
			m_FeedbackCamera.allowMSAA = false;
			m_FeedbackCamera.renderingPath = RenderingPath.Forward;

			// 摄像机渲染前先将rt设为白色(255, 255, 255, 255)
			// 后续处理中，如果发现mipmap等级为255表示该像素可以跳过
			m_FeedbackCamera.clearFlags = CameraClearFlags.Color;
			m_FeedbackCamera.backgroundColor = Color.white;

			// 设置预渲染着色器
			// 我们在VT相关Shader中设置了一个"VirtualTextureType"的Tag
			// 这样在预渲染流程中可以自动剔除掉非VT材质的对象
			m_FeedbackCamera.SetReplacementShader(m_FeedbackShader, "VirtualTextureType");
		}

		/// <summary>
		/// 拷贝摄像机参数
		/// </summary>
		private void CopyCamera(Camera camera)
		{
			if(camera == null)
				return;

			// Unity的Camera.CopyFrom方法会拷贝全部摄像机参数，这不是我们想要的，所以要自己写.
			m_FeedbackCamera.transform.position = camera.transform.position;
			m_FeedbackCamera.transform.rotation = camera.transform.rotation;
			m_FeedbackCamera.cullingMask = camera.cullingMask;
			m_FeedbackCamera.projectionMatrix = camera.projectionMatrix;
			m_FeedbackCamera.fieldOfView = camera.fieldOfView;
			m_FeedbackCamera.nearClipPlane = camera.nearClipPlane;
			m_FeedbackCamera.farClipPlane = camera.farClipPlane;
		}
    }
}
