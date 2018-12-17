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
    
    private Camera cam;
    private Texture2D rtResult;
    private CommandBuffer cb_output;

    private jCommonMaterial mat;

    private NativeArray<jSphere> jobSpheres;
    private NativeArray<jRay> raysBuffer, raysBuffer1;
    private NativeArray<Color> colorBuffer, colorBuffer1;
    private NativeArray<jHitRes> hits;
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
            Render(raysBuffer);
            Bounce(raysBuffer, raysBuffer1, colorBuffer, colorBuffer1);
            handleBounce.Complete();
            JobGlobal.currentDepth++;
        }
        else
        {
            Render(raysBuffer1);
            Bounce(raysBuffer1, raysBuffer, colorBuffer1, colorBuffer);
            handleBounce.Complete();
            JobGlobal.currentDepth++;
        }
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
                }
            }
        }
        hits = new NativeArray<jHitRes>(nx * ny, Allocator.Persistent);
        colorBuffer = new NativeArray<Color>(nx * ny, Allocator.Persistent);
        colorBuffer1 = new NativeArray<Color>(nx * ny, Allocator.Persistent);

        mat = new jCommonMaterial()
        {
            albedo = Color.yellow,
            distance = 0.2f
        };

        JobGlobal.currentDepth = 0;
        JobGlobal.envColor = env_Color;

        OnRenderComplete += () => {
            OutputRenderResult();
        };
    }

    private void Render(NativeArray<jRay> buffer)
    {
        raytracingJob = new RaytracingJob();
        raytracingJob.ns = ns;
        raytracingJob.t_min = 0f;
        raytracingJob.t_max = 300f;
        raytracingJob.sourceRays = buffer;
        raytracingJob.objs = jobSpheres;
        raytracingJob.res = hits;

        handleRaytracing = raytracingJob.Schedule(hits.Length, 8);

        //OnRaytraceComplete += () =>
        //{
        //    if (JobGlobal.currentDepth % 2 == 0)
        //        Bounce(raysBuffer, raysBuffer1, colorBuffer, colorBuffer1);
        //    else
        //        Bounce(raysBuffer1, raysBuffer, colorBuffer1, colorBuffer);
        //};

        print("Raytracing " + JobGlobal.currentDepth);
    }

    private void Bounce(NativeArray<jRay> src, NativeArray<jRay> dst,
        NativeArray<Color> colorSrc, NativeArray<Color> colorDst)
    {
        matJob = new ComputeBounceRayJob();
        matJob.ns = ns;
        matJob.material = mat;
        matJob.sourceRay = src;
        matJob.hits = hits;
        matJob.bounceRay = dst;

        handleBounce = matJob.Schedule(dst.Length, 8, handleRaytracing);

        //OnBounceComplete += () =>
        //{
        //    if (JobGlobal.currentDepth % 2 == 1)
        //        Render(raysBuffer1);
        //    else
        //        Render(raysBuffer);
        //};
        //isBouncing = true;

        print("Bouncing " + JobGlobal.currentDepth);
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
                rtResult.SetPixel(i, j, col);
            }
        }
        rtResult.Apply();
    }

    private void Update()
    {
        
    }



    private void OnDisable()
    {
        jobSpheres.Dispose();
        hits.Dispose();
        raysBuffer.Dispose();     
        raysBuffer1.Dispose();
        colorBuffer.Dispose();
        colorBuffer1.Dispose();
    }

    private void OnPreRender()
    {
        if (cb_output == null)
            return;

        cb_output.Clear();
        cb_output.Blit(rtResult, BuiltinRenderTextureType.CameraTarget);
    }

}
