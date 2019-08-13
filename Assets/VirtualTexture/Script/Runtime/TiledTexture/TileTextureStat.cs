using System.Collections.Generic;
using UnityEngine;

namespace VirtualTexture
{
	/// <summary>
	/// 平铺贴图统计类.
	/// </summary>
    public class TileTextureStat
	{
		private Dictionary<int, int> m_Map = new Dictionary<int, int>();

		public int CurrentActive { get; private set; }
        public int MaxActive { get; private set; }

        public void Reset()
        {
			MaxActive = Mathf.Max(MaxActive, CurrentActive);
			CurrentActive = 0;
            m_Map.Clear();
        }

		public void AddActive(int id)
        {
			if(!m_Map.ContainsKey(id))
			{
				m_Map[id] = 1;
                CurrentActive++;
            }
        }
    }
}