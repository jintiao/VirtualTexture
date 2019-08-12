using System.Diagnostics;
using UnityEditor;
using UnityEngine;

namespace VirtualTexture
{
    [CustomEditor(typeof(TiledTexture))]
	public class TileTextureEditor : EditorBase
	{
		protected override void OnPlayingInspectorGUI()
        {
            var tileTexture = (TiledTexture)target;

			DrawStat(tileTexture.Stat, tileTexture.RegionSize.x * tileTexture.RegionSize.y);

			for(int i = 0; i < tileTexture.Textures.Length; i++)
			{
				DrawTexture(tileTexture.Textures[i], string.Format("Layer{0}", i));
			}
        }

        private void DrawStat(TileTextureStat stat, int total)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Stat: ");
			EditorGUILayout.LabelField(string.Format("    Max Tiles Used In One Frame: {0}({1}%)", stat.MaxActive, (100.0f * stat.MaxActive / total).ToString("N2")));
        }
    }
}