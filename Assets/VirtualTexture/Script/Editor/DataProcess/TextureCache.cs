using System;
using System.IO;
using UnityEngine;

namespace VirtualTexture
{
    public class TextureCache
    {
        private string m_FilePath;
        private string m_FileName;

        private const int ComponentCount = 3;
        private const int ComponentSize = 1;
        private const int PixelSize = ComponentSize * ComponentCount;

        public int Size { get; private set; }

        private string GetFilePath(int row)
        {
            return Path.Combine(m_FilePath, string.Format("{0}_{1}", m_FileName, row));
        }

        public TextureCache(int size, string path, string name)
        {
            Size = size;
            m_FilePath = path;
            m_FileName = name;

            var fileSize = Size * PixelSize;
            for(int row = 0; row < Size; row++)
            {
                using (var file = File.Open(GetFilePath(row), FileMode.OpenOrCreate, FileAccess.Write))
                {
                    if (file.Length != fileSize)
                    {
                        file.SetLength(fileSize);
                    }
                }
            }
        }

		public void SetPixels(int x, int y, int blockWidth, int blockHeight, byte[] colors)
        {
			for (int row = 0; row < blockHeight; row++)
            {
                using (var writer = new BinaryWriter(File.Open(GetFilePath(row + y), FileMode.Open, FileAccess.Write)))
                {
                    writer.Seek(x * PixelSize, SeekOrigin.Begin);
					writer.Write(colors, row * blockWidth * PixelSize, blockWidth * PixelSize);
                }
            }
        }

		public byte[] GetPixels(int x, int y, int blockWidth, int blockHeight)
        {
			var pixels = new byte[blockWidth * blockHeight * PixelSize];

            for (int i = 0; i < blockHeight; i++)
            {
                var row = Mathf.Clamp(i + y, 0, Size - 1);
                using (var reader = new BinaryReader(File.Open(GetFilePath(row), FileMode.Open, FileAccess.Read)))
                {
					GetPixels(x, blockWidth, reader, pixels, i * blockWidth * PixelSize);
                }
            }

            return pixels;
        }

		private void GetPixels(int x, int blockWidth, BinaryReader reader, byte[] pixels, int pixelOffset)
        {
			var begin = Mathf.Clamp(x, 0, Size - 1);
			var width = Mathf.Min(blockWidth + x - begin, Size - begin);

			reader.BaseStream.Seek(begin * PixelSize, SeekOrigin.Begin);
			var buf = reader.ReadBytes(width * PixelSize);
			Buffer.BlockCopy(buf, 0, pixels, pixelOffset + (begin - x) * PixelSize, buf.Length);

			if(x < 0)
			{
				for(int i = 0; i < begin - x; i++)
				{
					Buffer.BlockCopy(buf, 0, pixels, pixelOffset + i * PixelSize, PixelSize);
				}
			}

			if(width < blockWidth)
			{
				for(int i = 0; i < blockWidth - width; i++)
				{
					Buffer.BlockCopy(buf, buf.Length - PixelSize, pixels, pixelOffset + (width + i) * PixelSize, PixelSize);
				}
			}
        }
    }
}
