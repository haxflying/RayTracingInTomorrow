using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SceneGenerator))]
public class SceneGeneratorEditor : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        SceneGenerator sg = target as SceneGenerator;

        GUILayout.Space(20);

        if(GUILayout.Button("Clear"))
        {
            sg.Clear();
        }

        if(GUILayout.Button("Generate"))
        {
            sg.Generate();
        }
    }
}
