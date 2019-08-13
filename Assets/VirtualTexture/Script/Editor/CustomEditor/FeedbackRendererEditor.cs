using System.Diagnostics;
using UnityEditor;
using UnityEngine;

namespace VirtualTexture
{
    [CustomEditor(typeof(FeedbackRenderer))]
	public class FeedbackRendererEditor : EditorBase
    {
		protected override void OnPlayingInspectorGUI()
		{
			var renderer = (FeedbackRenderer)target;
			DrawFrameStat(renderer.Stat);
			DrawTexture(renderer.TargetTexture, "Feedback Texture");
        }
    }
}