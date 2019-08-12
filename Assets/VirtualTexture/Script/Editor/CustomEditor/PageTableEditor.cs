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

            DrawStat(table.Stat, 11);
            DrawTexture(table.DebugTexture, "Lookup Texture");
        }

        private void DrawStat(PageTableStat stat, int maxLevel)
        {
            var requestTotal = 0;
            var requests = new Dictionary<int, int>();
            var hits = new Dictionary<int, int>();
            foreach (var kv in stat.RequestTables)
            {
                if (kv.Key > maxLevel)
                    continue;
                requestTotal += kv.Value.Count;
                requests[kv.Key] = kv.Value.Count;
                hits[kv.Key] = 0;
            }

            var hitTotal = 0;
            foreach (var kv in stat.HitTables)
            {
                if (kv.Key > maxLevel)
                    continue;
                hitTotal += kv.Value.Count;
                hits[kv.Key] = kv.Value.Count;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(string.Format("hits/requests: {0}/{1}({2}%)", hitTotal, requestTotal, (100.0f * hitTotal / requestTotal).ToString("N2")));
            foreach (var kv in requests.OrderBy(pair => pair.Key))
            {
                EditorGUILayout.LabelField(string.Format("    mip{0}: {1}/{2}({3}%)", kv.Key, hits[kv.Key], kv.Value, (100.0f * hits[kv.Key] / kv.Value).ToString("N2")));
            }

            DrawFrameStat(stat);
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