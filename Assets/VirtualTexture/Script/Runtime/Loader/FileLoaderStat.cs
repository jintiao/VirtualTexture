using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace VirtualTexture
{
	/// <summary>
	/// 文件加载器统计类.
	/// </summary>
	public class FileLoaderStat
	{
		/// <summary>
		/// 当前请求数量
		/// </summary>
		public int CurrentRequestCount { get; set; }

		/// <summary>
		/// 总请求数量
		/// </summary>
		public int TotalRequestCount { get; set; }

		/// <summary>
		/// 总失败请求数量
		/// </summary>
		public int TotalFailCount { get; set; }

		/// <summary>
		/// 总下载量
		/// </summary>
		public float TotalDownladSize { get; set; }
	}
}