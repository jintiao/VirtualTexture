using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace VirtualTexture
{
	public class PageTable : MonoBehaviour, IPageTable
	{
		/// <summary>
		/// 页表尺寸.
		/// </summary>
		[SerializeField]
		private int m_TableSize = default;

		/// <summary>
		/// 最大mipmap等级限制
		/// </summary>
		[SerializeField]
		private int m_MipLevelLimit = default;

        /// <summary>
        /// 调试着色器.
        /// 用于在编辑器中显示贴图mipmap等级
        /// </summary>
        [SerializeField]
        private Shader m_DebugShader = default;

        /// <summary>
        /// 页表层级结构
        /// </summary>
        private TableNode m_PageTable;

        /// <summary>
        /// 当前活跃的页表
        /// </summary>
        private Dictionary<Vector2Int, TableNode> m_ActivePages = new Dictionary<Vector2Int, TableNode>();

        /// <summary>
        /// 导出的页表寻址贴图
        /// </summary>
        private Texture2D m_LookupTexture;

        /// <summary>
        /// 是否需要更新页表
        /// </summary>
        private Color32[] m_LookupTexturePixels;

        /// <summary>
        /// 文件加载器对象
        /// </summary>
        private ILoader m_Loader;

        /// <summary>
        /// 平铺贴图对象
        /// </summary>
        private ITiledTexture m_TileTexture;

        /// <summary>
        /// 调试材质.
        /// 用于在编辑器中显示贴图mipmap等级
        /// </summary>
        private Material m_DebugMaterial;

		/// <summary>
		/// 统计数据
		/// </summary>
		public FrameStat Stat { get; } = new FrameStat();

        /// <summary>
        /// 调试贴图
        /// </summary>
        public RenderTexture DebugTexture { get; private set; }

		/// <summary>
		/// 页表尺寸.
		/// </summary>
		public int TableSize { get { return m_TableSize; } }

		/// <summary>
		/// 最大mipmap等级
		/// </summary>
		public int MaxMipLevel { get { return Mathf.Min(m_MipLevelLimit, (int)Mathf.Log(TableSize, 2)); } }

		private void Start()
        {
			m_PageTable = new TableNode(MaxMipLevel, 0, 0, TableSize, TableSize);

			m_LookupTexture = new Texture2D(TableSize, TableSize, TextureFormat.RGBA32, false);
			m_LookupTexture.filterMode = FilterMode.Point;
			m_LookupTexture.wrapMode = TextureWrapMode.Clamp;

			m_LookupTexturePixels = m_LookupTexture.GetPixels32();

			Shader.SetGlobalTexture(
				"_VTLookupTex",
				m_LookupTexture);
			Shader.SetGlobalVector(
				"_VTPageParam",
				new Vector4(
					TableSize,
					1.0f / TableSize,
					MaxMipLevel,
					0));

			InitDebugTexture(TableSize, TableSize);

            m_Loader = (ILoader)GetComponent(typeof(ILoader));
            m_Loader.OnLoadComplete += OnLoadComplete;
            m_TileTexture = (ITiledTexture)GetComponent(typeof(ITiledTexture));
            m_TileTexture.OnTileUpdateComplete += InvalidatePage;
            ((IFeedbackReader)GetComponent(typeof(IFeedbackReader))).OnFeedbackReadComplete += ProcessFeedback;
        }

        /// <summary>
        /// 处理回读数据
        /// </summary>
        private void ProcessFeedback(Texture2D texture)
        {
            Stat.BeginFrame();

            // 激活对应页表
            foreach (var c in texture.GetRawTextureData<Color32>())
            {
                ActivatePage(c.r, c.g, c.b);
            }

            // 将页表数据写入页表贴图
            var currentFrame = (byte)Time.frameCount;
            var pixels = m_LookupTexture.GetRawTextureData<Color32>();
            foreach (var kv in m_ActivePages)
            {
                var page = kv.Value;

                // 只写入当前帧活跃的页表
                if (page.Payload.ActiveFrame != Time.frameCount)
                    continue;

                // a位保存写入frame序号，用于检查pixels是否为当前帧写入的数据(避免旧数据残留)
                var c = new Color32((byte)page.Payload.TileIndex.x, (byte)page.Payload.TileIndex.y, (byte)page.MipLevel, currentFrame);
                for (int y = page.Rect.y; y < page.Rect.yMax; y++)
                {
                    for (int x = page.Rect.x; x < page.Rect.xMax; x++)
                    {
                        var id = y * TableSize + x;
                        if (pixels[id].b > c.b ||  // 写入mipmap等级最小的页表
                            pixels[id].a != currentFrame) // 当前帧还没有写入过数据
                            pixels[id] = c;
                    }
                }
            }
            m_LookupTexture.Apply(false);

            Stat.EndFrame();

            UpdateDebugTexture();
        }

        /// <summary>
        /// 激活页表
        /// </summary>
        private TableNode ActivatePage(int x, int y, int mip)
        {
            if (mip > m_PageTable.MipLevel)
                return null;

            // 找到当前可用的页表
            var page = m_PageTable.GetAvailable(x, y, mip);
            if (page == null)
			{
				// 没有可用页表，加载根节点
                LoadPage(x, y, m_PageTable);
                return null;
            }
            else if (page.MipLevel > mip)
            {
                // 可用页表不是请求的mipmap等级，请求加载下一级
                LoadPage(x, y, page.GetChild(x, y));
            }

            // 激活对应的平铺贴图块
            m_TileTexture.SetActive(page.Payload.TileIndex);

            page.Payload.ActiveFrame = Time.frameCount;
            return page;
        }

        /// <summary>
        /// 加载页表
        /// </summary>
        private void LoadPage(int x, int y, TableNode node)
        {
            if (node == null)
                return;

            // 正在加载中,不需要重复请求
			if(node.Payload.LoadRequest != null)
				return;

            // 新建加载请求
			node.Payload.LoadRequest = m_Loader.Request(x, y, node.MipLevel);
        }

        /// <summary>
        /// 加载完成回调
        /// </summary>
        private void OnLoadComplete(LoadRequest request, Texture2D[] textures)
        {
            // 找到对应页表
            var node = m_PageTable.Get(request.PageX, request.PageY, request.MipLevel);
			if (node == null || node.Payload.LoadRequest != request)
                return;

			node.Payload.LoadRequest = null;

            var id = m_TileTexture.RequestTile();
            m_TileTexture.UpdateTile(id, textures);

            node.Payload.TileIndex = id;
            m_ActivePages[id] = node;
        }

        /// <summary>
        /// 将页表置为非活跃状态
        /// </summary>
		private void InvalidatePage(Vector2Int id)
        {
            TableNode node = null;
            if (!m_ActivePages.TryGetValue(id, out node))
                return;

            node.Payload.ResetTileIndex();
            m_ActivePages.Remove(id);
        }

        [Conditional("ENABLE_DEBUG_TEXTURE")]
        private void InitDebugTexture(int w, int h)
        {
            DebugTexture = new RenderTexture(w, h, 0);
            DebugTexture.wrapMode = TextureWrapMode.Clamp;
            DebugTexture.filterMode = FilterMode.Point;
        }

        [Conditional("ENABLE_DEBUG_TEXTURE")]
        private void UpdateDebugTexture()
        {
            if (m_LookupTexture == null || m_DebugShader == null)
                return;

            if (m_DebugMaterial == null)
                m_DebugMaterial = new Material(m_DebugShader);

            Graphics.Blit(m_LookupTexture, DebugTexture, m_DebugMaterial);
        }
    }
}
