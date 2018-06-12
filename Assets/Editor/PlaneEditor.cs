#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(Plane))]

public class PlaneEditor : Editor {
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Plane myScript = (Plane)target;
        if (GUILayout.Button("Build Plane"))
        {
            myScript.BuildPlane();
        }
    }
}
#endif