using System;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualTexture
{
    /// <summary>
    /// 页表节点
    /// </summary>
    public class TableNode
    {
        private TableNode[] m_Children;

        public int MipLevel { get; }
        public RectInt Rect { get; }

        public PagePayload Payload { get; } 

        public TableNode(int mip, int x, int y, int width, int height)
        {
            MipLevel = mip;
            Rect = new RectInt(x, y, width, height);
			Payload = new PagePayload();
        }

        // 取x/y/mip完全一致的node，没有就返回null
        public TableNode Get(int x, int y, int mip)
        {
            if (!Contains(x, y))
                return null;

            if (MipLevel == mip)
                return this;

            if (m_Children != null)
            {
                foreach(var child in m_Children)
                {
                    var item = child.Get(x, y, mip);
                    if (item != null)
                        return item;
                }
            }

            return null;
        }

        // 取最近激活的节点
        public void GetLatest(int x, int y, ref TableNode current)
        {
            if (!Contains(x, y))
                return;

            if (Payload.IsReady)
            {
                if (current == null ||
                    Payload.ActiveFrame > current.Payload.ActiveFrame ||
                    (Payload.ActiveFrame == current.Payload.ActiveFrame && MipLevel < current.MipLevel))
                {
                    current = this;
                }
            }

            if (m_Children != null)
            {
                foreach (var child in m_Children)
                {
                    child.GetLatest(x, y, ref current);
                }
            }
        }

        // 取当前已经就绪的节点
        public TableNode GetAvailable(int x, int y, int mip)
        {
            if (!Contains(x, y))
                return null;

            if (MipLevel > mip && m_Children != null)
            {
                foreach (var child in m_Children)
                {
                    var item = child.GetAvailable(x, y, mip);
                    if (item != null)
                        return item;
                }
            }

            return (Payload.IsReady ? this : null);
        }

        /// <summary>
        /// 取子页表
        /// </summary>
        public TableNode GetChild(int x, int y)
        {
            if (!Contains(x, y))
                return null;

            if (MipLevel == 0)
                return null;

            if (m_Children == null)
            {
                m_Children = new TableNode[4];
				m_Children[0] = new TableNode(MipLevel - 1, Rect.x, Rect.y, Rect.width / 2, Rect.height / 2);
				m_Children[1] = new TableNode(MipLevel - 1, Rect.x + Rect.width / 2, Rect.y, Rect.width / 2, Rect.height / 2);
				m_Children[2] = new TableNode(MipLevel - 1, Rect.x, Rect.y + Rect.height / 2, Rect.width / 2, Rect.height / 2);
				m_Children[3] = new TableNode(MipLevel - 1, Rect.x + Rect.width / 2, Rect.y + Rect.height / 2, Rect.width / 2, Rect.height / 2);
            }

            foreach (var child in m_Children)
            {
                if (child.Contains(x, y))
                    return child;
            }

            return null;
        }

        private bool Contains(int x, int y)
        {
            if (x < Rect.x || x >= Rect.xMax)
                return false;

            if (y < Rect.y || y >= Rect.yMax)
                return false;

            return true;
        }
    }
}
