using System.IO;
using UnityEditor;
using UnityEngine;
using VirtualTexture;

namespace Demo
{
	public static class DemoMenu
	{
        private static int s_PageSize = 256;
        private static int s_PaddingSize = 4;
        private static int s_TableSize = 64;

        [MenuItem("Demo/Generate Virtual Texture")]
		public static void GenerateVirtualTexture()
        {
            Debug.Log("Generating VirtualTexture...");

            var saveDir = Application.streamingAssetsPath;
            if (!Directory.Exists(saveDir))
                Directory.CreateDirectory(saveDir);

            int maxLevel = (int)Mathf.Log(s_TableSize, 2);
            for (int mip = 0; mip <= maxLevel; mip++)
            {
                if (mip == 0)
                {
                    GenerateMip0Cache();
                }
                else
                {
                    GenerateCache(mip);
                }

                ExportCache(mip);
            }

            Debug.Log("GenerateVirtualTexture Done.");
            AssetDatabase.Refresh();
        }

        private static void GenerateMip0Cache()
        {
            int rowCount = 12;
            int columnCount = 10;
            int srcTextureWidth = 1024;
            int srcTextureHeight = 768;

            var cache = GetCache(0);
            for (var row = 0; row < rowCount; row++)
            {
                for (var col = 0; col < columnCount; col++)
                {
                    var inputFile = Path.Combine("Assets", "Demo", "Slides", 
                                                string.Format("slide_{0}.JPG", (row * columnCount + col + 1).ToString("D3")));
                    var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(inputFile);
                    if (texture == null)
                        continue;

                    cache.SetPixels(
                        col * srcTextureWidth,
                        row * srcTextureHeight,
						texture.width,
						texture.height,
						texture.GetRawTextureData());
                }
            }

            Debug.Log("Generate Mip0Cache Done.");
        }

        private static void GenerateCache(int mip)
        {
            var inputCache = GetCache(mip - 1);
            var outputCache = GetCache(mip);

            int patchSize = s_PageSize;
            int patchSizeDouble = patchSize * 2;
            int patchCount = outputCache.Size / patchSize;

			var inputTexture = new Texture2D(patchSizeDouble, patchSizeDouble, TextureFormat.RGB24, false);
			var outputTexture = new Texture2D(patchSize, patchSize, TextureFormat.RGB24, false);
			var renderTexture = RenderTexture.GetTemporary(patchSize, patchSize);
			var lastActiveRT = RenderTexture.active;
			RenderTexture.active = renderTexture;

            for (int row = 0; row < patchCount; row++)
            {
                for (int col = 0; col < patchCount; col++)
                {
                    var inputPixels = inputCache.GetPixels(col * patchSizeDouble, row * patchSizeDouble, patchSizeDouble, patchSizeDouble);
					inputTexture.LoadRawTextureData(inputPixels);
					inputTexture.Apply(false);

					Graphics.Blit(inputTexture, renderTexture);

					outputTexture.ReadPixels(new Rect(0, 0, patchSize, patchSize), 0, 0, false);
					outputTexture.Apply(false);

					var outputPixels = outputTexture.GetRawTextureData();
                    outputCache.SetPixels(col * patchSize, row * patchSize, patchSize, patchSize, outputPixels);
                }
            }

			RenderTexture.active = lastActiveRT;
			RenderTexture.ReleaseTemporary(renderTexture);
        }

        private static TextureCache GetCache(int mip)
        {
            var dir = Path.Combine(Application.dataPath, "Demo", "Cache~");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var size = (s_TableSize * s_PageSize) >> mip;
            var cache = new TextureCache(size, dir, "mip" + mip);
            return cache;
        }

        private static void ExportCache(int mip)
        {
            Debug.LogFormat("Exporting Mip{0}...", mip);

            var cache = GetCache(mip);

			int pageSizewithPadding = s_PageSize + s_PaddingSize * 2;

            int pageCount = cache.Size / s_PageSize;
            for (int row = 0; row < pageCount; row++)
			{
				for(int col = 0; col < pageCount; col++)
				{
                    var pixels = cache.GetPixels(
                        col * s_PageSize - s_PaddingSize, 
                        row * s_PageSize - s_PaddingSize, 
                        pageSizewithPadding, 
                        pageSizewithPadding);

					var texture = new Texture2D(pageSizewithPadding, pageSizewithPadding, TextureFormat.RGB24, false);
					texture.LoadRawTextureData(pixels);
					texture.Apply(false);

					var bytes = ImageConversion.EncodeToPNG(texture);
                    var file = Path.Combine(Application.streamingAssetsPath, string.Format("Slide_MIP{2}_Y{1}_X{0}.png", col, row, mip));
                    File.WriteAllBytes(file, bytes);
				}
            }

            Debug.LogFormat("Export Mip{0} Done.", mip);
        }
	}
}