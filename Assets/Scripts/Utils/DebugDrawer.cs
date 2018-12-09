using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
public class DebugDrawer : MonoBehaviour
{

    public static DebugDrawer Instance;

    private void Awake()
    {
        Instance = this;
    }
    public bool isDebug;
    public zRay ray;
    public List<Vector3> points = new List<Vector3>();

    
}
