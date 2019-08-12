using System;
using UnityEngine;

namespace VirtualTexture
{
	/// <summary>
	/// 预渲染贴图回读接口.
	/// </summary>
    public interface IFeedbackReader
    {
		/// <summary>
		/// 回读完成的事件回调.
		/// </summary>
        event Action<Texture2D> OnFeedbackReadComplete;
    }
}
