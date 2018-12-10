using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Globalization;
using System;

[RequireComponent(typeof(Camera))]
public partial class MainLoop : MonoBehaviour {
    public GUISkin skin;
    public bool showGui;
    public bool useBVH;
    public bool debugMod;

    [Range(1, 200)]
    public int ns = 50;
    public Color env_Color = Color.black;

    private Camera cam;
    private CommandBuffer cb_output;
    private Texture2D rtResult;
    private float progress;
    private int startTime;
    private int timePassed;
    private bool renderDone;
    private  Vector2 debugPoint;

    private Hitable world;

    void Start () {
        cam = GetComponent<Camera>();

        cb_output = new CommandBuffer();
        cb_output.name = "RT Result";
        cam.AddCommandBuffer(CameraEvent.AfterEverything, cb_output);

        rtResult = new Texture2D(Screen.width, Screen.height);
        rtResult.name = "ResultTexture";

        rtObject[] objs = GameObject.FindObjectsOfType<rtObject>();
        List<Hitable> list = new List<Hitable>();
        foreach(var obj in objs)
        {
            list.Add(obj.toSphere());
        }

        world = new hitable_list(list, 0, 0, useBVH);
        print("World Count " + list.Count);

        debugPoint = Vector2.zero;
    }
    
    IEnumerator Render()
    {
        int nx = Screen.width;
        int ny = Screen.height;
        progress = 0f;
        startTime = time();
        renderDone = false;

        Vector3 lookFrom = new Vector3(1f, 2f, 2f);
        Vector3 lookAt = new Vector3(0f, 0f, -1f);
        float dist_to_focus = 2f;
        float aperture = 0f;
        zCamera zcam = new zCamera(cam);
        uint index = 0;

        if (debugMod)
        {
            Color col = Color.black;
            for (int s = 0; s < 1; s++)
            {
                float u = (float)((int)debugPoint.x + zRandom.Halton5(index++)) / (float)(nx);
                float v = (float)((int)debugPoint.y + zRandom.Halton5(index++)) / (float)(ny);
                zRay r = zcam.get_ray(u, v);
                DebugDrawer.Init(cam);
                col += color(r, world, 0);
            }
        }
        else
        {
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
                    rtResult.SetPixel(rtResult.width - i, j, col);
                }
                progress += nx;
                yield return null;
            }
            rtResult.Apply();
            renderDone = true;
            timePassed = time();
        }
        yield return null;
    }    

    Color color(zRay r, Hitable world, int depth)
    {
        
        hit_record rec = new hit_record();
        if(world.hit(r, 0.0f, float.MaxValue, ref rec))
        {
            zRay scattered = new zRay();
            Vector3 attenuation = Vector3.zero;
            Color emitted = rec.mat.emitted();
            if (depth < 30 && rec.mat.scatter(r, rec, ref attenuation, ref scattered))
            {                
                return emitted + new Color(attenuation.x, attenuation.y, attenuation.z) * color(scattered, world, depth + 1);
            }
            else
                return emitted;
        }
        else
        {
            //float t = 0.5f * (r.direction.normalized.y + 1f);
            //return (1f - t) * Color.white + t * new Color(0.5f, 0.7f, 1.0f);
            return env_Color;
        }
    }


    private void OnPreRender()
    {
        if (cb_output == null || !renderDone)
            return;

        cb_output.Clear();
        cb_output.Blit(rtResult, BuiltinRenderTextureType.CameraTarget);
    }

}

