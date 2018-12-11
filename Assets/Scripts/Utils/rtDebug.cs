using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Camera))]
public partial class MainLoop : MonoBehaviour
{
    public Material debugLine;

    private void Update()
    {
        if(Input.GetMouseButtonDown(0) && debugMod)
        {
            DebugDrawer.Clear();
            debugPoint = Input.mousePosition;
            debugPoint.x = Screen.width - debugPoint.x;
            StartCoroutine(Render());
        }

        if(Input.GetKeyDown(KeyCode.S) && !debugMod)
        {
            StartCoroutine(Render());
        }

        if (DebugDrawer.isDebug != debugMod)
            DebugDrawer.isDebug = debugMod;
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
