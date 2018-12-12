using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(rtObject))]
[CanEditMultipleObjects]
public class rtObjectEditor : Editor {

    SerializedProperty mat, type;

    private void OnEnable()
    {
        mat = serializedObject.FindProperty("material");
        type = serializedObject.FindProperty("type");
    }

    public override void OnInspectorGUI()
    {
        rtObject me = target as rtObject;
        GUILayout.Space(20);
        EditorGUILayout.PropertyField(mat, new GUIContent("mat"));
        EditorGUILayout.PropertyField(type, new GUIContent("type"));
        if(me.type == rtObject.ObjType.Sphere)
        {
            me.isPlane = GUILayout.Toggle(me.isPlane, "isPlane");
            me.isMoving = GUILayout.Toggle(me.isMoving, "isMoving");
        }
        else
        {

        }

        if(GUILayout.Button("preview"))
        {
            me.DoPreview();
        }

        //if(GUILayout.Button("Test"))
        //{
        //    me.test();
        //}

        serializedObject.ApplyModifiedProperties();
    }
}
