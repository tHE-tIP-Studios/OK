using System.Collections;
using System.Collections.Generic;
using Fishing.Area;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FishingMarker)), CanEditMultipleObjects]
public class FishingMarkerInspector : Editor
{
    private FishingMarker _marker;
    private void OnEnable()
    {
        _marker = target as FishingMarker;
    }

    public override void OnInspectorGUI()
    {

        if (GUILayout.Button("Select Fishing Area", GUILayout.Height(40)))
        {
            Selection.activeGameObject = _marker.ContainingArea.gameObject;
        }

        EditorGUILayout.LabelField("", GUILayout.Height(3));

        if (GUILayout.Button("Delete Marker", GUILayout.Height(30)))
        {
            if (Selection.gameObjects.Length > 0)
                foreach (GameObject m in Selection.gameObjects)
                    m.GetComponent<FishingMarker>()?.Delete();
            else
                _marker.Delete();

            Selection.activeGameObject = _marker.ContainingArea.gameObject;
        }
    }
}