using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(WeatherAudioTransitionZone))] 
public class WeatherAudioTransitionZoneEditor : Editor 
{
    public void OnSceneGUI()
    {
        WeatherAudioTransitionZone zone = (WeatherAudioTransitionZone)target;

        float handleSize = HandleUtility.GetHandleSize(zone.TransitionLocus) / 8.0f;
        Vector3 newLocus = Handles.Slider2D(zone.transform.position + zone.TransitionLocus, new Vector3(0.0f, 1.0f, 0.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), handleSize, Handles.SphereCap, new Vector2(0.1f, 0.1f));
        Handles.DrawWireDisc(zone.transform.position + zone.TransitionLocus, Vector3.up, zone.TransitionRadius);
        zone.TransitionLocus = newLocus - zone.transform.position;

    }
}
