using UnityEditor;
using UnityEngine;

namespace VirtualTexture
{
	[CustomEditor(typeof(FileLoader))]
	public class FileLoaderEditor : EditorBase
	{
		protected override void OnPlayingInspectorGUI()
		{
			var loader = (FileLoader)target;
			DrawFileLoaderStat(loader.Stat);
		}

		private void DrawFileLoaderStat(FileLoaderStat stat)
		{
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("File Loader Stat: ");
			EditorGUILayout.LabelField(string.Format("    Current Requests: {0}", stat.CurrentRequestCount));
			EditorGUILayout.LabelField(string.Format("    Total Requests: {0}", stat.TotalRequestCount));
			EditorGUILayout.LabelField(string.Format("    Total Download: {0} MB", stat.TotalDownladSize.ToString("N2")));
			EditorGUILayout.LabelField(string.Format("    Total Failed: {0}", stat.TotalFailCount));
		}
	}
}