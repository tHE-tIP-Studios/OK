using System.Collections;
using System.Collections.Generic;
using Fauna;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AnimalInfo))]
public class AnimalInfoInspector : Editor
{
    AnimalInfo info;
    Texture2D gfxTex;
    private void OnEnable()
    {
        info = target as AnimalInfo;
        GetTex();
    }

    public override void OnInspectorGUI()
    {
        GetTex();
        if (gfxTex != null)
        {
            GUILayout.Label(gfxTex);
        }
        EditorGUILayout.LabelField("", GUILayout.Height(3));
        DrawDefaultInspector();
    }

    void GetTex()
    {
        if (info.FishGFX != null)
        {
            gfxTex = AssetPreview.GetAssetPreview(info.FishGFX);
        }
    }

}