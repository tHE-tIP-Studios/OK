using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Fauna;
using Utilities;

public class FishInfoReaderWindow : EditorWindow
{
    private string _folderPath;
    private bool _fileExists;

    [MenuItem("Window/Information Reader")]
    public static void ShowWindow()
    {
        GetWindow<FishInfoReaderWindow>("Information Reader");
    }

    private void OnEnable()
    {
        _folderPath = FishInfoReader.fullPath;
    }

    void OnFocus()
    {
        _fileExists = FishInfoReader.fileExists;
    }

    private void OnGUI()
    {
        Space();

        if (_fileExists)
        {
            EditorGUILayout.HelpBox("Targeting file:\n" + _folderPath, MessageType.Info);
            Space();
            if (GUILayout.Button("Update Files", GUILayout.Height(40)))
            {
               CreateAllFound(FishInfoReader.Read());
            }
            if (GUILayout.Button("Open in Explorer", GUILayout.Height(40)))
            {
                FishInfoReader.OpenExplorer();
            }
        }
        else
        {
            EditorGUILayout.HelpBox("File not found:\n" + _folderPath, MessageType.Error);
        }
    }

    private void Space()
    {
        EditorGUILayout.LabelField("", GUILayout.Height(3));
    }

    private void CreateAllFound(IEnumerable<RawInfoHolder> raws)
    {
        foreach(RawInfoHolder r in raws)
            AnimalInfo.CreateNewAsset(r);
    }
}