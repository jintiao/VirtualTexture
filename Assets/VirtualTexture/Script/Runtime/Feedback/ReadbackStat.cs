using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace VirtualTexture
{
	/// <summary>
	/// 回读统计类.
	/// </summary>
	public class ReadbackStat
	{
		// 请求记录
		private Dictionary<AsyncGPUReadbackRequest, int> m_Requests = new Dictionary<AsyncGPUReadbackRequest, int>();

		// 总延迟数量
		private int m_LatencyTotal;

		// 总请求数量
		private int m_RequestCount;

		/// <summary>
		/// 当前延迟
		/// </summary>
		public int CurrentLatency { get; private set; }

		/// <summary>
		/// 平均延迟
		/// </summary>
		public float AverageLatency { get; private set; }

		/// <summary>
		/// 最大延迟
		/// </summary>
		public int MaxLatency { get; private set; }

		/// <summary>
		/// 出错次数
		/// </summary>
		public int FailedCount { get; private set; }

		/// <summary>
		/// 开始统计
		/// </summary>
		public void BeginRequest(AsyncGPUReadbackRequest request)
		{
			// 只统计启动5秒后的数据, 避免收集程序刚启动时的异常数据.
			if(Time.realtimeSinceStartup < 5)
				return;

			m_Requests[request] = Time.frameCount;
		}

		/// <summary>
		/// 结束统计
		/// </summary>
		public void EndRequest(AsyncGPUReadbackRequest request, bool success)
		{
			int frame = 0;
			if(!m_Requests.TryGetValue(request, out frame))
				return;

			if (!success)
				FailedCount++;

			CurrentLatency = Time.frameCount - frame;
			MaxLatency = Mathf.Max(MaxLatency, CurrentLatency);

			m_LatencyTotal += CurrentLatency;
			m_RequestCount++;

			AverageLatency = (float)m_LatencyTotal / m_RequestCount;
		}
	}
}