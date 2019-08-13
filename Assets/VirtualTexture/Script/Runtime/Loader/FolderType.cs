using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace VirtualTexture
{
	public enum FolderType
	{
		ApplicationData,
		StreamingAssets,
		None,
	}

	public static class FolderTypeExtensions
	{
		public static string ToStr(this FolderType folder)
		{
			switch(folder)
			{
			case FolderType.ApplicationData:
				return Application.dataPath;
			case FolderType.StreamingAssets:
				return Application.streamingAssetsPath;
			}
			return "";
		}
	}
}