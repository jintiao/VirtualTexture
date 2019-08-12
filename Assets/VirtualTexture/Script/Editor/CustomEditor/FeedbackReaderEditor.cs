using System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace VirtualTexture
{
    [CustomEditor(typeof(FeedbackReader))]
	public class FeedbackReaderEditor : EditorBase
	{
		protected override void OnPlayingInspectorGUI()
        {
			var reader = (FeedbackReader)target;

			DrawReadbackStat(reader.ReadbackStat);
			DrawFrameStat(reader.UpdateStat);
			DrawTexture(reader.DebugTexture, "Mipmap Level Debug Texture");
        }

		private void DrawReadbackStat(ReadbackStat stat)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Readback Stat: ");
            EditorGUILayout.LabelField(string.Format("    Current Latency: {0} frame(s)", stat.CurrentLatency));
            EditorGUILayout.LabelField(string.Format("    Average Latency: {0} frame(s)", stat.AverageLatency.ToString("N2")));
			EditorGUILayout.LabelField(string.Format("    Max Latency    : {0} frame(s)", stat.MaxLatency));
			EditorGUILayout.LabelField(string.Format("    Fails          : {0} time(s)", stat.FailedCount));
        }

    }
}