using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace VirtualTexture
{
	/// <summary>
	/// 加载请求类.
	/// </summary>
    public class LoadRequest
    {
		/// <summary>
		/// 页表X坐标
		/// </summary>
        public int PageX { get; }

		/// <summary>
		/// 页表Y坐标
		/// </summary>
        public int PageY { get; }

		/// <summary>
		/// mipmap等级
		/// </summary>
		public int MipLevel { get; }

		/// <summary>
		/// 构造函数
		/// </summary>
        public LoadRequest(int x, int y, int mip)
        {
            PageX = x;
            PageY = y;
            MipLevel = mip;
        }
    }
}