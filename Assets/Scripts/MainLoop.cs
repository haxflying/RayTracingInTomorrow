using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Globalization;
using System;

[RequireComponent(typeof(Camera))]
public class MainLoop : MonoBehaviour {
    public GUISkin skin;
    private Camera cam;
    private CommandBuffer cb_output;
    private Texture2D rtResult;
    private float progress;
    private int startTime;
    private int timePassed;
    private bool renderDone;
	void Start () {
        cam = GetComponent<Camera>();

        cb_output = new CommandBuffer();
        cb_output.name = "RT Result";
        cam.AddCommandBuffer(CameraEvent.AfterEverything, cb_output);

        rtResult = new Texture2D(Screen.width, Screen.height);
        rtResult.name = "ResultTexture";

        StartCoroutine(Render());
    }
    
    IEnumerator Render()
    {
        int nx = Screen.width;
        int ny = Screen.height;
        int ns = 100;
        progress = 0f;
        startTime = time();
        renderDone = false;

        Hitable world = zScene.moving_obj();
        Vector3 lookFrom = new Vector3(1f, 2f, 2f);
        Vector3 lookAt = new Vector3(0f, 0f, -1f);
        float dist_to_focus = 2f;
        float aperture = 0f;
        zCamera zcam = new zCamera(lookFrom, lookAt, Vector3.up, 90, (float)nx / (float)ny, aperture, dist_to_focus, 0f, 1f);
        //zCamera zcam = new zCamera();
        uint index = 0;
        for (int j = ny - 1; j >= 0; j--)
        {
            Color col = Color.black;
            for (int i = 0; i < nx; i++)
            {
                for (int s = 0; s < ns; s++)
                {
                    float u = (float)(i + zRandom.Halton5(index++)) / (float)(nx);
                    float v = (float)(j + zRandom.Halton5(index++)) / (float)(ny);
                    zRay r = zcam.get_ray(u, v);
                    col += color(r, world, 0);
                }
                col /= (float)ns;
                col = col.gamma;
                rtResult.SetPixel(i, j, col);
                
            }
            progress += nx;
            yield return null;
        }
        rtResult.Apply();
        renderDone = true;
        timePassed = time();
        yield return null;
    }

    private void OnGUI()
    {
        GUI.skin = skin;
        GUI.color = Color.green;
        GUILayout.Label(((progress / (float)(Screen.width * Screen.height)) * 100).ToString("0.000") + "%");
        if(!renderDone)
            GUILayout.Label("time:" + (time() - startTime) + "s");
        else
            GUILayout.Label("time:" + (timePassed - startTime) + "s");
    }

    private int time()
    {
        int time = (int)(DateTime.Now - new DateTime(2018, 12, 1)).TotalSeconds;
        return time;
    }

    private void OnPreRender()
    {
        if (cb_output == null)
            return;

        cb_output.Clear();
        cb_output.Blit(rtResult, BuiltinRenderTextureType.CameraTarget);
    }    

    Color color(zRay r, Hitable world, int depth)
    {
        hit_record rec = new hit_record();
        if(world.hit(r, 0.0f, float.MaxValue, ref rec))
        {
            zRay scattered = new zRay();
            Vector3 attenuation = Vector3.zero;
            if (depth < 30 && rec.mat.scatter(r, rec, ref attenuation, ref scattered))
            {
                return new Color(attenuation.x, attenuation.y, attenuation.z) * color(scattered, world, depth + 1);
            }
            else
                return Color.black;
        }
        else
        {
            float t = 0.5f * (r.direction.normalized.y + 1f);
            return (1f - t) * Color.white + t * new Color(0.5f, 0.7f, 1.0f);
        }
    }

}

