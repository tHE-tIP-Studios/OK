using System.Collections;
using System.Collections.Generic;
using Fishing.Area;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FishingArea))]
public class FishingAreaInspector : Editor
{
    private const int MAX_MARKER_BUTTONS_PER_LINE = 5;

    private FishingArea _fishingArea;
    private SerializedProperty _gizmosColor;

    private void OnEnable()
    {
        _fishingArea = target as FishingArea;
        _gizmosColor = serializedObject.FindProperty("_gizmosColor");

        if (!_fishingArea.MarkersParentExists())
            _fishingArea.CreateMarkersParent();
    }

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Add New Marker", GUILayout.Height(50)))
        {
            Selection.activeGameObject = _fishingArea.NewMarker().gameObject;
        }

        //! Destructive action, only enable in case of errors
        // if (GUILayout.Button("Reset", GUILayout.Height(30)))
        // {
        //     _fishingArea.ResetArea();
        // }
        //! ///

        DrawMarkerButtons();

        EditorGUILayout.PropertyField(_gizmosColor);
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawMarkerButtons()
    {
        if (_fishingArea.Markers == null || _fishingArea.Markers.Length == 0) return;

        EditorGUILayout.LabelField("", GUILayout.Height(3));
        EditorGUILayout.LabelField("Marker Selection", GUILayout.Height(20));

        EditorGUILayout.BeginHorizontal();

        for (int i = 0; i < _fishingArea.Markers.Length; i++)
        {
            if (i != 0 && i % MAX_MARKER_BUTTONS_PER_LINE == 0)
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
            }
            DrawSelectButton(_fishingArea.Markers[i].gameObject, i);
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.LabelField("", GUILayout.Height(3));

        void DrawSelectButton(GameObject target, int index)
        {
            if (GUILayout.Button((index + 1).ToString(), GUILayout.Height(20)))
                Selection.activeGameObject = target;
        }

    }
}