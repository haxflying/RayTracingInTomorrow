using UnityEngine;
using System.Collections;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.Rendering;

public class JobLoop : MonoBehaviour
{
    [Range(1, 200)]
    public int ns = 50;
    public int batchCount = 100;
    
    private Camera cam;
    private Texture2D rtResult;
    private CommandBuffer cb_output;

    private NativeArray<jSphere> jobSpheres;
    private NativeArray<jRay> rays;
    private NativeArray<jHitRes> hits;
    private rtJob myJob;
    private JobHandle handle;

    private void Start()
    {
        cam = GetComponent<Camera>();

        cb_output = new CommandBuffer();
        cb_output.name = "RT Result";
        cam.AddCommandBuffer(CameraEvent.AfterEverything, cb_output);

        rtResult = new Texture2D(Screen.width, Screen.height);
        rtResult.name = "ResultTexture";

        rtObject[] objs = GameObject.FindObjectsOfType<rtObject>();
        jobSpheres = new NativeArray<jSphere>(objs.Length, Allocator.Persistent);
        for (int i = 0; i < jobSpheres.Length; i++)
        {
            jobSpheres[i] = objs[i].toJobSphere();
        }

        InitRender();
        Render();
    }

    private void InitRender()
    {
        int nx = Screen.width;
        int ny = Screen.height;
        int rayCount = nx * ny * ns;
        rays = new NativeArray<jRay>(rayCount, Allocator.Persistent);

        zCamera zcam = new zCamera(cam);
        uint index = 0;
        for (int j = 0; j < ny; j++)
        {
            for (int i = 0; i < nx; i++)
            {
                for (int s = 0; s < ns; s++)
                {
                    float u = (float)(i + zRandom.Halton5(index++)) / (float)(nx);
                    float v = (float)(j + zRandom.Halton5(index++)) / (float)(ny);
                    rays[(j * nx + i) * ns + s] = zcam.get_jobRay(u, v);
                }
            }
        }

        hits = new NativeArray<jHitRes>(nx * ny, Allocator.Persistent);
        myJob = new rtJob();
        myJob.batchHitCount = hits.Length / batchCount;
        myJob.ns = ns;
    }

    private void Render()
    {
        //temp version 
        handle = myJob.Schedule(hits.Length, batchCount);
    }

    private void Update()
    {
        if(handle.IsCompleted)
        {
            print("!!Done");
            int nx = Screen.width;
            int ny = Screen.height;
            for (int j = 0; j < ny; j++)
            {
                for (int i = 0; i < nx; i++)
                {
                    Vector3 n = hits[j * nx + i].normal;
                    Color col = new Color(n.x, n.y, n.z);
                    rtResult.SetPixel(i, j, col);
                }
            }
        }
    }

    private void OnDisable()
    {
        jobSpheres.Dispose();
        rays.Dispose();
        hits.Dispose();
    }

    private void OnPreRender()
    {
        if (cb_output == null)
            return;

        cb_output.Clear();
        cb_output.Blit(rtResult, BuiltinRenderTextureType.CameraTarget);
    }

}
