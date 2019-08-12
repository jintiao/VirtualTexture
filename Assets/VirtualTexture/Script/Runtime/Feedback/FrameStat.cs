using UnityEngine;

namespace VirtualTexture
{
	/// <summary>
	/// 帧耗时统计类.
	/// </summary>
    public class FrameStat
	{
		// 总时间
		private float m_FrameTimeTotal;

		// 帧数量
		private int m_FrameCount;

		// 当前帧开始时间
		private float m_FrameBeginTime;

		/// <summary>
		/// 当前帧耗时
		/// </summary>
		public float CurrentTime { get; private set; }

		/// <summary>
		/// 平均耗时
		/// </summary>
		public float AverageTime { get; private set; }

		/// <summary>
		/// 最大耗时
		/// </summary>
		public float MaxTime { get; private set; }

		/// <summary>
		/// 开始统计
		/// </summary>
        public void BeginFrame()
        {
            m_FrameBeginTime = Time.realtimeSinceStartup;
        }

		/// <summary>
		/// 结束统计
		/// </summary>
        public void EndFrame()
        {
			// 只统计启动5秒后的数据, 避免收集程序刚启动时的异常数据.
			if(Time.realtimeSinceStartup < 5)
				return;

            var t = (Time.realtimeSinceStartup - m_FrameBeginTime) * 1000;
            m_FrameTimeTotal += t;
            m_FrameCount++;

            CurrentTime = t;
            MaxTime = Mathf.Max(MaxTime, t);
            AverageTime = m_FrameTimeTotal / m_FrameCount;
        }
    }
}
