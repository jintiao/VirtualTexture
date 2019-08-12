using System;
using UnityEngine;

namespace VirtualTexture
{
	/// <summary>
	/// 预渲染器类接口
	/// </summary>
    public interface IFeedbackRenderer
	{
		/// <summary>
		/// 预渲染完成后的事件回调
		/// </summary>
        event Action<RenderTexture> OnFeedbackRenderComplete;
    }
}
