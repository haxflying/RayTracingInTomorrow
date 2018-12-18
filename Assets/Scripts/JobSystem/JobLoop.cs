using UnityEngine;
using System.Collections;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.Rendering;

public class JobLoop : MonoBehaviour
{
    [Range(1, 200)]
    public int ns = 50;
    [Range(1, 10)]
    public int maxBounce = 3;
    public Color env_Color;
    public Color matColor;
    
    private Camera cam;
    private Texture2D rtResult;
    private CommandBuffer cb_output;

    private NativeArray<jSphere> jobSpheres;
    private NativeArray<jRay> raysBuffer, raysBuffer1;
    private RaytracingJob raytracingJob;
    private ComputeBounceRayJob matJob;
    private JobHandle handleRaytracing, handleBounce;
    private bool bRenderDone;
    private bool isBouncing;

    private delegate void OnJobComplete();
    private event OnJobComplete OnRaytraceComplete, OnBounceComplete, OnRenderComplete;

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

        bRenderDone = false;
        InitRender();
        print("EnvColor " + env_Color);
        for (int i = 0; i < maxBounce; i++)
        {
            StepRender();
        }

        OutputRenderResult();
    }

    private void StepRender()
    {
        if(JobGlobal.currentDepth % 2 == 0)
        {
            Render(raysBuffer, raysBuffer1);
            handleRaytracing.Complete();
        }
        else
        {
            Render(raysBuffer1, raysBuffer);
            handleRaytracing.Complete();
        }
        JobGlobal.currentDepth++;
    }

    private void InitRender()
    {
        int nx = Screen.width;
        int ny = Screen.height;
        int rayCount = nx * ny * ns;
        raysBuffer = new NativeArray<jRay>(rayCount, Allocator.Persistent);
        raysBuffer1 = new NativeArray<jRay>(rayCount, Allocator.Persistent);       

        zCamera zcam = new zCamera(cam);
        uint index = 0;
        for (int j = ny - 1; j >= 0; j--)
        {
            for (int i = 0; i < nx; i++)
            {
                for (int s = 0; s < ns; s++)
                {
                    float u = (float)(i + zRandom.Halton5(index++)) / (float)(nx);
                    float v = (float)(j + zRandom.Halton5(index++)) / (float)(ny);
                    raysBuffer[(j * nx + i) * ns + s] = zcam.get_jobRay(u, v);
                    raysBuffer1[(j * nx + i) * ns + s] = zcam.get_jobRay(u, v);
                }
            }
        }

        JobGlobal.currentDepth = 0;
        JobGlobal.envColor = env_Color;

        OnRenderComplete += () => {
            OutputRenderResult();
        };
    }

    private void Render(NativeArray<jRay> src, NativeArray<jRay> dst)
    {
        raytracingJob = new RaytracingJob();
        raytracingJob.ns = ns;
        raytracingJob.t_min = 0f;
        raytracingJob.t_max = 300f;
        raytracingJob.objs = jobSpheres;
        raytracingJob.sourceRays = src;      
        raytracingJob.destRays = dst;

        handleRaytracing = raytracingJob.Schedule(raysBuffer.Length, 8);
    }


    private void OutputRenderResult()
    {
        int nx = Screen.width;
        int ny = Screen.height;
        for (int j = ny - 1; j >= 0; j--)
        {
            for (int i = 0; i < nx; i++)
            {
                Color col = Color.black;
                for (int s = 0; s < ns; s++)
                {
                    int index = (j * nx + i) * ns + s;
                    if (JobGlobal.currentDepth % 2 == 1)
                        col += raysBuffer1[index].color;
                    else
                        col += raysBuffer[index].color;
                }
                col /= ns;
                rtResult.SetPixel(nx - i, j, col);
            }
        }
        rtResult.Apply();
    }


    private void OnDisable()
    {
        jobSpheres.Dispose();
        raysBuffer.Dispose();     
        raysBuffer1.Dispose();
    }

    private void OnPreRender()
    {
        if (cb_output == null)
            return;

        cb_output.Clear();
        cb_output.Blit(rtResult, BuiltinRenderTextureType.CameraTarget);
    }

}
