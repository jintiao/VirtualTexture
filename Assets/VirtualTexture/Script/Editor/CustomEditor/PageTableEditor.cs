using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace VirtualTexture
{
    [CustomEditor(typeof(PageTable))]
	public class PageTableEditor : EditorBase
    {
        protected override void OnPlayingInspectorGUI()
        {
			var table = (PageTable)target;

			DrawFrameStat(table.Stat);
            DrawTexture(table.DebugTexture, "Lookup Texture");
        }

        [Conditional("ENABLE_DEBUG_TEXTURE")]
        private void DrawPreviewTexture(Texture texture)
        {
            if (texture == null)
                return;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(string.Format("Texture Size: {0} X {1}", texture.width, texture.height));
            EditorGUI.DrawPreviewTexture(GUILayoutUtility.GetAspectRect((float)texture.width / texture.height), texture);
        }
    }
}