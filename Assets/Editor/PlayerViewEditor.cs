using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(PlayerView))] 
public class PlayerViewEditor : Editor 
{
	public override void OnInspectorGUI()
	{
		PlayerView view = (PlayerView)target;
		
		view.Dynamic = GUILayout.Toggle(view.Dynamic, "Dynamic");
		
		GUILayout.BeginVertical((GUIStyle)("Box"));
		view.ShowCandidateRays = GUILayout.Toggle(view.ShowCandidateRays, "Candidate Rays");
		view.ShowSucceededRays = GUILayout.Toggle(view.ShowSucceededRays, "Succeeded Rays");
		view.ShowExtrusionRays = GUILayout.Toggle(view.ShowExtrusionRays, "Extrusion Rays");
		view.ShowFailedRays = GUILayout.Toggle(view.ShowFailedRays, "Failed Rays");
		view.collisionLayer = EditorGUILayout.LayerField("Collision Layer", view.collisionLayer);
		GUILayout.EndVertical();
		
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("Nudge Magnitude", GUILayout.Width(100));
		GUILayout.Label(view.m_nudgeMagnitude.ToString("0.00"), GUILayout.Width(40));
		view.m_nudgeMagnitude = GUILayout.HorizontalSlider(view.m_nudgeMagnitude, 0.001f, 0.999f);
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("Centre Nudge", GUILayout.Width(100));
		GUILayout.Label(view.m_centreNudge.ToString("0.00"), GUILayout.Width(40));
		view.m_centreNudge = GUILayout.HorizontalSlider(view.m_centreNudge, 0.001f, 0.999f);
		GUILayout.EndHorizontal();
		
	}
}
