using UnityEngine;

namespace VirtualTexture
{
    /// <summary>
    /// 页表数据
    /// </summary>
    public class PagePayload
	{
		private static Vector2Int s_InvalidTileIndex = new Vector2Int(-1, -1);

        /// <summary>
        /// 对应平铺贴图中的id
        /// </summary>
		public Vector2Int TileIndex = s_InvalidTileIndex;

        /// <summary>
        /// 激活的帧序号
        /// </summary>
        public int ActiveFrame;

        /// <summary>
        /// 加载请求
        /// </summary>
		public LoadRequest LoadRequest;

        /// <summary>
        /// 是否处于可用状态
        /// </summary>
		public bool IsReady { get { return (TileIndex != s_InvalidTileIndex); } }

        /// <summary>
        /// 重置页表数据
        /// </summary>
        public void ResetTileIndex()
        {
			TileIndex = s_InvalidTileIndex;
        }
    }
}
