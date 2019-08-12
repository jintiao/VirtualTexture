using System.Diagnostics;
using UnityEditor;
using UnityEngine;

namespace VirtualTexture
{
	public abstract class EditorBase : Editor
	{
		public override void OnInspectorGUI()
		{
			if(Application.isPlaying)
			{
				OnPlayingInspectorGUI();
			}
			else
			{
				DrawDefaultInspector();
				serializedObject.ApplyModifiedProperties();
			}
		}

		protected abstract void OnPlayingInspectorGUI();

		protected void DrawFrameStat(FrameStat stat)
		{
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Frame Stat: ");
			EditorGUILayout.LabelField(string.Format("    Current Time: {0} ms", stat.CurrentTime.ToString("N2")));
			EditorGUILayout.LabelField(string.Format("    Average Time: {0} ms", stat.AverageTime.ToString("N2")));
			EditorGUILayout.LabelField(string.Format("    Max Time    : {0} ms", stat.MaxTime.ToString("N2")));
		}

        [Conditional("ENABLE_DEBUG_TEXTURE")]
		protected void DrawTexture(Texture texture, string label = null)
		{
			if(texture == null)
				return;

			EditorGUILayout.Space();
			if (!string.IsNullOrEmpty(label))
			{
				EditorGUILayout.LabelField(label);
				EditorGUILayout.LabelField(string.Format("    Size: {0} X {1}", texture.width, texture.height));
			}
			else
			{
				EditorGUILayout.LabelField(string.Format("Size: {0} X {1}", texture.width, texture.height));
			}

			EditorGUI.DrawPreviewTexture(GUILayoutUtility.GetAspectRect((float)texture.width / texture.height), texture);
		}
	}
}