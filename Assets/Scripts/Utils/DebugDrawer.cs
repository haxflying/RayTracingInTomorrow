using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
public class DebugDrawer : MonoBehaviour
{
    public static bool isDebug;
    static public rtObject[] objs;
    static public List<Vector3> points = new List<Vector3>();
    static public List<Vector3> normals = new List<Vector3>();

    public static void Clear()
    {
        points.Clear();
        normals.Clear();
    }

    public static void Init(Camera cam)
    {
        
        points.Add(cam.transform.position);
        normals.Add(cam.transform.up);
    }

    private void OnEnable()
    {
        SceneView.onSceneGUIDelegate += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
    }

    static void OnSceneGUI(SceneView sceneView)
    {
        if (isDebug)
            DrawRay();
    }

    static void DrawRay()
    {
        Handles.zTest = UnityEngine.Rendering.CompareFunction.Less;
        Handles.color = Color.green;
        if (objs != null)
        {
            foreach(var obj in objs)
            {
                Handles.Label(obj.transform.position, obj.instanceID.ToString());
                Bounds bd = obj.GetComponent<Renderer>().bounds;
                Handles.DrawWireCube(bd.center, bd.size);
            }
        }
        Handles.color = Color.red;
        for (int i = 0; i < points.Count - 1; i++)
        {
            Handles.DrawLine(points[i], points[i + 1]);
        }
        

        Handles.color = Color.blue;
        for (int i = 0; i < points.Count; i++)
        {
            Handles.DrawLine(points[i], points[i] + normals[i]);
        }


        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.green;

        for (int i = 0; i < points.Count; i++)
        {
            Handles.Label(points[i], i.ToString(), style);
        }

    }
}
