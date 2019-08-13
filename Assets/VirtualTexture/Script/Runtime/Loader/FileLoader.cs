using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace VirtualTexture
{
	/// <summary>
	/// 文件加载器类.
	/// </summary>
	public class FileLoader : MonoBehaviour, ILoader
	{
		/// <summary>
		/// 加载完成的事件回调.
		/// </summary>
		public event Action<LoadRequest, Texture2D[]> OnLoadComplete;

		/// <summary>
		/// 允许同时进行加载的文件数量.
		/// </summary>
		[SerializeField]
		private int m_ThreadLimit = 1;

		/// <summary>
		/// 文件根目录.
		/// </summary>
		[SerializeField]
		private FolderType m_FileRoot = default;

		/// <summary>
		/// 文件路径格式化文本
		/// </summary>
		[SerializeField]
		private string[] m_FilePathStrs = default;

		/// <summary>
		/// 正在加载的请求.
		/// </summary>
        private List<LoadRequest> m_RuningRequests = new List<LoadRequest>();

		/// <summary>
		/// 等待处理的请求.
		/// </summary>
		private List<LoadRequest> m_PendingRequests = new List<LoadRequest>();

		public FileLoaderStat Stat { get; } = new FileLoaderStat();

		private void Start()
		{
			// 初始化下载路径
			for(int i = 0; i < m_FilePathStrs.Length; i++)
			{
                //m_FilePathStrs[i] = Path.Combine(m_FileRoot.ToStr(), m_FilePathStrs[i]);
                m_FilePathStrs[i] = "file:///" + Path.Combine(m_FileRoot.ToStr(), m_FilePathStrs[i]);
            }
		}

		private void Update()
		{
			Stat.CurrentRequestCount = m_PendingRequests.Count + m_RuningRequests.Count;

			if(m_PendingRequests.Count <= 0)
				return;

			if(m_RuningRequests.Count > m_ThreadLimit)
				return;

			// 优先处理mipmap等级高的请求
			m_PendingRequests.Sort((x, y) => { return x.MipLevel.CompareTo(y.MipLevel); });

			// 将第一个请求从等待队列移到运行队列
			var req = m_PendingRequests[m_PendingRequests.Count - 1];
			m_PendingRequests.RemoveAt(m_PendingRequests.Count - 1);
			m_RuningRequests.Add(req);

			// 开始加载
			StartCoroutine(Load(req));
		}

		/// <summary>
		/// 开始加载
		/// </summary>
		private IEnumerator Load(LoadRequest request)
		{
			Texture2D[] textures = new Texture2D[m_FilePathStrs.Length];

			for(int i = 0; i < m_FilePathStrs.Length; i++)
			{
				var file = string.Format(m_FilePathStrs[i], request.PageX >> request.MipLevel, request.PageY >> request.MipLevel, request.MipLevel);
                var www = UnityWebRequestTexture.GetTexture(file);
				yield return www.SendWebRequest();

				if(!www.isNetworkError && !www.isHttpError)
				{
					textures[i] = ((DownloadHandlerTexture)www.downloadHandler).texture;
					Stat.TotalDownladSize += (float)www.downloadedBytes / 1024.0f / 1024.0f;
                }
				else
				{
					Debug.LogWarningFormat("Load file({0}) failed: {1}", file, www.error);
					Stat.TotalFailCount++;
				}
			}

			m_RuningRequests.Remove(request);
			OnLoadComplete?.Invoke(request, textures);
		}

		/// <summary>
		/// 新建加载请求
		/// </summary>
		public LoadRequest Request(int x, int y, int mip)
		{
			// 是否已经在请求队列中
			foreach(var r in m_RuningRequests)
			{
				if(r.PageX == x && r.PageY == y && r.MipLevel == mip)
					return null;
			}
			foreach(var r in m_PendingRequests)
			{
				if(r.PageX == x && r.PageY == y && r.MipLevel == mip)
					return null;
			}

			// 加入待处理列表
			var request = new LoadRequest(x, y, mip);
			m_PendingRequests.Add(request);

			Stat.TotalRequestCount++;

			return request;
		}
    }
}
