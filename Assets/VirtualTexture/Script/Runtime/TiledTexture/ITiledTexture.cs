using System;
using UnityEngine;

namespace VirtualTexture
{
	/// <summary>
	/// 平铺贴图接口.
	/// </summary>
    public interface ITiledTexture
	{
		/// <summary>
		/// Tile更新完成的事件回调.
		/// </summary>
		event Action<Vector2Int> OnTileUpdateComplete;

		/// <summary>
		/// 区域尺寸.
		/// 区域尺寸表示横竖两个方向上Tile的数量.
		/// </summary>
		Vector2Int RegionSize { get; }

		/// <summary>
		/// 单个Tile的尺寸.
		/// Tile是宽高相等的正方形.
		/// </summary>
		int TileSize { get; }

		/// <summary>
		/// 填充尺寸
		/// 每个Tile上下左右四个方向都要进行填充，用来支持硬件纹理过滤.
		/// </summary>
		int PaddingSize { get; }

		/// <summary>
		/// 层级数量.
		/// ITiledTexture可以同时支持多个层级(多张物理贴图)。
		/// 例如支持物理渲染我们可以使用3个层级，第0层是Diffuse，第1层是Normal，第2层保存Smooth/Specular等.
		/// </summary>
		int LayerCount { get; }

        /// <summary>
        /// 请求分配Tile
        /// </summary>
		Vector2Int RequestTile();

		/// <summary>
		/// 将Tile置为活跃状态
		/// </summary>
		bool SetActive(Vector2Int tile);

		/// <summary>
		/// 更新Tile内容
		/// </summary>
		void UpdateTile(Vector2Int tile, Texture2D[] textures);
    }
}