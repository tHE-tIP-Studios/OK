using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Movement.Cameras;

[CustomEditor(typeof(CameraSwitch))]
public class CameraSwitchInspector : Editor
{
    private CameraSwitch _cameraSwitch;

    private void OnEnable() 
    {
        _cameraSwitch = target as CameraSwitch;
    }

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Switch to Walk Camera", GUILayout.Height(50)))
        {
            _cameraSwitch.TurnOnWalk();
        }

        if (GUILayout.Button("Switch to Fishing Camera", GUILayout.Height(50)))
        {
            _cameraSwitch.TurnOnFish();
        }

        if (GUILayout.Button("Switch to Crouch Camera", GUILayout.Height(50)))
        {
            _cameraSwitch.TurnOnCrouch();
        }
    }
}
