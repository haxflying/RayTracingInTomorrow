using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(rtObject))]
[CanEditMultipleObjects]
public class rtObjectEditor : Editor {

    public override void OnInspectorGUI()
    {
        rtObject me = target as rtObject;
        GUILayout.Space(20);
        me.material = (zMaterial)EditorGUILayout.ObjectField("mat", me.material, typeof(zMaterial), false);
        me.type = (rtObject.ObjType)EditorGUILayout.EnumPopup("Type", me.type);
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

        if(GUILayout.Button("Test"))
        {
            me.test();
        }
    }
}
