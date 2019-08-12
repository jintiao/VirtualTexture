using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace VirtualTexture
{
	/// <summary>
	/// 页表接口.
	/// </summary>
	public interface IPageTable
	{
		/// <summary>
		/// 页表尺寸.
		/// </summary>
		int TableSize { get; }

		/// <summary>
		/// 最大mipmap等级
		/// </summary>
		int MaxMipLevel { get; }
	}
}