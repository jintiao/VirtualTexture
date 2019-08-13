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
		private Queue<int> m_Requests;

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

			if (m_Requests == null)
				m_Requests = new Queue<int>();

			m_Requests.Enqueue(Time.frameCount);
		}

		/// <summary>
		/// 结束统计
		/// </summary>
		public void EndRequest(AsyncGPUReadbackRequest request, bool success)
		{
			if(m_Requests == null || m_Requests.Count == 0)
				return;
			
			if (!success)
				FailedCount++;

			CurrentLatency = Time.frameCount - m_Requests.Dequeue();
			MaxLatency = Mathf.Max(MaxLatency, CurrentLatency);

			m_LatencyTotal += CurrentLatency;
			m_RequestCount++;

			AverageLatency = (float)m_LatencyTotal / m_RequestCount;
		}
	}
}