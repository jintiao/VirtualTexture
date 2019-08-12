using System;
using System.Diagnostics;
using UnityEngine;

namespace VirtualTexture
{
    public class TiledTexture : MonoBehaviour, ITiledTexture
	{
		/// <summary>
		/// Tile更新完成的事件回调.
		/// </summary>
		public event Action<Vector2Int> OnTileUpdateComplete;

		/// <summary>
		/// 区域尺寸.
		/// </summary>
		[SerializeField]
		private Vector2Int m_RegionSize = default;

		/// <summary>
		/// 单个Tile的尺寸.
		/// </summary>
		[SerializeField]
		private int m_TileSize = 256;

		/// <summary>
		/// 填充尺寸
		/// </summary>
		[SerializeField]
		private int m_PaddingSize = 4;

		/// <summary>
		/// 层级数量.
		/// </summary>
		[SerializeField]
		private int m_LayerCount = 1;

		/// <summary>
		/// Tile缓存池.
		/// </summary>
		private LruCache m_TilePool = new LruCache();

		/// <summary>
		/// 物理贴图数组.
		/// </summary>
        public RenderTexture[] Textures { get; private set; }

		/// <summary>
		/// 数据统计.
		/// </summary>
        public TileTextureStat Stat { get; } = new TileTextureStat();

		/// <summary>
		/// 区域尺寸.
		/// 区域尺寸表示横竖两个方向上Tile的数量.
		/// </summary>
		public Vector2Int RegionSize { get { return m_RegionSize; } }

		/// <summary>
		/// 单个Tile的尺寸.
		/// Tile是宽高相等的正方形.
		/// </summary>
		public int TileSize { get { return m_TileSize; } }

		/// <summary>
		/// 填充尺寸
		/// 每个Tile上下左右四个方向都要进行填充，用来支持硬件纹理过滤.
		/// 所以Tile有效尺寸为(TileSize - PaddingSize * 2)
		/// </summary>
		public int PaddingSize { get { return m_PaddingSize; } }

		/// <summary>
		/// 层级数量.
		/// ITiledTexture可以同时支持多个层级(多张物理贴图)。
		/// 例如支持物理渲染我们可以使用3个层级，第0层是Diffuse，第1层是Normal，第2层保存Smooth/Specular等.
		/// </summary>
		public int LayerCount { get { return m_LayerCount; } }

		public int TileSizeWithPadding { get { return TileSize + PaddingSize * 2; } }

        private void Start()
        {
            for (int i = 0; i < RegionSize.x * RegionSize.y; i++)
                m_TilePool.Add(i);

			Textures = new RenderTexture[LayerCount];
			for(int i = 0; i < LayerCount; i++)
			{
				Textures[i] = new RenderTexture(RegionSize.x * TileSizeWithPadding, RegionSize.y * TileSizeWithPadding, 0);
				Textures[i].useMipMap = false;
				Textures[i].wrapMode = TextureWrapMode.Clamp;

				Shader.SetGlobalTexture(
					string.Format("_VTTiledTex{0}", i),
					Textures[i]);
			}

			// 设置Shader参数
			// x: padding偏移量
			// y: tile有效区域的尺寸
			// zw: 1/区域尺寸
            Shader.SetGlobalVector(
                "_VTTileParam", 
                new Vector4(
					(float)PaddingSize / TileSizeWithPadding,
					(float)TileSize / TileSizeWithPadding,
                    1.0f / RegionSize.x,
                    1.0f / RegionSize.y));
        }

        private void Update()
		{
			Stat.Reset();
        }

		public Vector2Int RequestTile()
        {
			return IdToPos(m_TilePool.First);
		}

		public bool SetActive(Vector2Int tile)
        {
			bool success = m_TilePool.SetActive(PosToId(tile));

			if (success)
				Stat.AddActive(PosToId(tile));

			return success;
        }

		public void UpdateTile(Vector2Int tile, Texture2D[] textures)
        {
			if (!SetActive(tile))
                return;

			if(textures == null)
				return;

			for(int i = 0; i < textures.Length; i++)
			{
				if (textures[i] != null)
				{
					TextureUtil.DrawTexture(textures[i], Textures[i], new RectInt(tile.x * TileSizeWithPadding, tile.y * TileSizeWithPadding, TileSizeWithPadding, TileSizeWithPadding));
				}
			}

			OnTileUpdateComplete?.Invoke(tile);
		}

		private Vector2Int IdToPos(int id)
		{
			return new Vector2Int(id % RegionSize.x, id / RegionSize.x);
		}

		private int PosToId(Vector2Int tile)
		{
			return (tile.y * RegionSize.x + tile.x);
		}
    }
}
