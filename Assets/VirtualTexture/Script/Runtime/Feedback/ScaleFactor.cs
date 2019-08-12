namespace VirtualTexture
{
	/// <summary>
	/// 缩放因子
	/// </summary>
	public enum ScaleFactor
	{
		// 原始尺寸
		One,

		// 1/2尺寸
        Half,

		// 1/4尺寸
        Quarter,

		// 1/8尺寸
        Eighth,
	}

	public static class ScaleModeExtensions
	{
		public static float ToFloat(this ScaleFactor mode)
		{
			switch(mode)
			{
			case ScaleFactor.Eighth:
				return 0.125f;
			case ScaleFactor.Quarter:
				return 0.25f;
			case ScaleFactor.Half:
				return 0.5f;
			}
			return 1;
		}
	}
}