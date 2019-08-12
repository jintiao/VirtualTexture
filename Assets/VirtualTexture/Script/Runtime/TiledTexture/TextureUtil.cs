using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualTexture
{
	/// <summary>
	/// 贴图工具类
	/// </summary>
	public static class TextureUtil
	{
		// 贴图绘制材质
		private static Material uploadMateral;

		/// <summary>
		/// 将source贴图绘制到target贴图的指定区域position中
		/// </summary>
		public static void DrawTexture(Texture source, RenderTexture target, RectInt position)
		{
			if(source == null || target == null)
				return;

			// 初始化绘制材质
			if(uploadMateral == null)
			{
				var shader = Shader.Find("Hidden/VirtualTexture/DrawTexture");
				if(shader == null)
					return;
				uploadMateral = new Material(shader);
			}

			// 构建变换矩阵
			float l = position.x * 2.0f / target.width - 1;
			float r = (position.x + position.width) * 2.0f / target.width - 1;
			float b = position.y * 2.0f / target.height - 1;
			float t = (position.y + position.height) * 2.0f / target.height - 1;
			var mat = new Matrix4x4();
			mat.m00 = r - l;
			mat.m03 = l;
			mat.m11 = t - b;
			mat.m13 = b;
			mat.m23 = -1;
			mat.m33 = 1;

			// 绘制贴图
			uploadMateral.SetMatrix(Shader.PropertyToID("_ImageMVP"), GL.GetGPUProjectionMatrix(mat, true));
			Graphics.Blit(source, target, uploadMateral);
		}
	}
}
