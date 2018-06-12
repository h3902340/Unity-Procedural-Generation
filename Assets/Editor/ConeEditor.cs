#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(Cone))]

public class ConeEditor : Editor {
    SerializedProperty HeightSegment;
    SerializedProperty bottomRadius;
    SerializedProperty topRadius;

    private void OnEnable()
    {
        HeightSegment = serializedObject.FindProperty("HeightSegment");
        bottomRadius = serializedObject.FindProperty("bottomRadius");
        topRadius = serializedObject.FindProperty("topRadius");
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Cone myScript = (Cone)target;
        if (GUILayout.Button("Build Cone"))
        {
            myScript.BuildCone(myScript.gameObject, myScript.HeightSegment, myScript.bottomRadius, myScript.topRadius);
        }
        if (GUILayout.Button("Grow Branch"))
        {
            myScript.GrowBranch();
        }
        if (GUILayout.Button("Clear Branches"))
        {
            myScript.ClearChildren();
        }
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            // Block of code with controls
            // that may set GUI.changed to true

            EditorGUILayout.IntSlider(HeightSegment, 8, 30, new GUILayoutOption[] { GUILayout.Height(20), GUILayout.ExpandHeight(false) });
            EditorGUILayout.Slider(bottomRadius, 0, 1f, new GUILayoutOption[] { GUILayout.Height(20), GUILayout.ExpandHeight(false) });
            EditorGUILayout.Slider(topRadius, 0, 0.5f, new GUILayoutOption[] { GUILayout.Height(20), GUILayout.ExpandHeight(false) });
            if (check.changed)
            {
                myScript.BuildCone(myScript.gameObject, myScript.HeightSegment, myScript.bottomRadius, myScript.topRadius);
                // Code to execute if GUI.changed
                // was set to true inside the block of code above.
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif