using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualTexture
{
    public class PageTableStat : FrameStat
    {
        private int m_Size;

        public int PageSize { get { return m_Size * m_Size; } }
        public Dictionary<int, Dictionary<int, int>> RequestTables { get; } = new Dictionary<int, Dictionary<int, int>>();
        public Dictionary<int, Dictionary<int, int>> HitTables { get; } = new Dictionary<int, Dictionary<int, int>>();

        public PageTableStat(int s)
        {
            m_Size = s;
        }

        public void Reset()
        {
            RequestTables.Clear();
            HitTables.Clear();
        }

        public void AddRequest(int x, int y, int mip, bool hit)
        {
            Add(x, y, mip, RequestTables);

            if (hit)
                Add(x, y, mip, HitTables);
        }

        private void Add(int x, int y, int mip, Dictionary<int, Dictionary<int, int>> dict)
        {
            Dictionary<int, int> table = null;
            if (!dict.TryGetValue(mip, out table))
            {
                table = new Dictionary<int, int>();
                dict[mip] = table;
            }

            var id = y * (m_Size >> mip) + x;
            table[id] = table.ContainsKey(id) ? table[id] + 1 : 1;
        }
    }
}
