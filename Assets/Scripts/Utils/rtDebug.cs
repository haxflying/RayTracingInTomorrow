using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Camera))]
public partial class MainLoop : MonoBehaviour
{
    public Material debugLine;

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            DebugDrawer.Instance.points.Clear();
            debugPoint = Input.mousePosition;
            StartCoroutine(Render());
        }

        if(Input.GetKeyDown(KeyCode.S))
        {
            StartCoroutine(Render());
        }


        if(debugMod && DebugDrawer.Instance == null)
        {
            gameObject.AddComponent<DebugDrawer>();
            DebugDrawer.Instance.isDebug = true;
        }

        if (DebugDrawer.Instance == null)
            return;

        if(!debugMod && DebugDrawer.Instance.isDebug)
        {
            DebugDrawer.Instance.isDebug = false;
        }
    }


    private void OnGUI()
    {

        if (!showGui)
            return;

        GUI.skin = skin;
        GUI.color = Color.green;
        GUILayout.Label(((progress / (float)(Screen.width * Screen.height)) * 100).ToString("0.000") + "%");
        if (!renderDone)
            GUILayout.Label("time:" + (time() - startTime) + "s");
        else
            GUILayout.Label("time:" + (timePassed - startTime) + "s");
    }

    private int time()
    {
        int time = (int)(DateTime.Now - new DateTime(2018, 12, 1)).TotalSeconds;
        return time;
    }
}
