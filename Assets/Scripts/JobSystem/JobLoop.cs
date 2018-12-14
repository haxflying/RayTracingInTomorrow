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
    
    private Camera cam;
    private Texture2D rtResult;
    private CommandBuffer cb_output;

    private NativeArray<jSphere> jobSpheres;
    private NativeArray<jRay> raysBuffer, raysBuffer1;
    private NativeArray<jHitRes> hits;
    private RaytracingJob initRtJob;
    private ComputeBounceRayJob matJob;
    private JobHandle handleRaytracing, handleComputeRay;
    private bool bResPrinted;

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

        bResPrinted = false;
        InitRender();
        Render(raysBuffer);
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

        JobGlobal.currentDepth = 0;
    }

    private void Render(NativeArray<jRay> buffer)
    {
        initRtJob = new RaytracingJob();
        initRtJob.ns = ns;
        initRtJob.t_min = 0f;
        initRtJob.t_max = 300f;
        initRtJob.rays = buffer;
        initRtJob.objs = jobSpheres;
        initRtJob.res = hits;
        handleRaytracing = initRtJob.Schedule(hits.Length, 8);
    }

    private void Bounce(NativeArray<jRay> src, NativeArray<jRay> dst)
    {
        matJob = new ComputeBounceRayJob();
        matJob.sourceRay = src;
        matJob.hits = hits;
        matJob.bounceRay = dst;
        handleComputeRay = matJob.Schedule(raysBuffer1.Length, 8);
    }

    private void OutputRenderResult()
    {
        int nx = Screen.width;
        int ny = Screen.height;
        for (int j = 0; j < ny; j++)
        {
            for (int i = 0; i < nx; i++)
            {
                if (hits[j * nx + i].bHit == 1)
                {
                    Vector3 n = hits[j * nx + i].normal;
                    Color col = new Color(n.x, n.y, n.z);
                    rtResult.SetPixel(i, j, col);
                }
                else
                {
                    rtResult.SetPixel(i, j, Color.cyan);
                }
            }
        }
        rtResult.Apply();
    }   

    private void Update()
    {
        if(handleRaytracing.IsCompleted && JobGlobal.currentDepth++ < maxBounce)
        {
            handleRaytracing.Complete();
            if(JobGlobal.currentDepth % 2 == 0)
                Bounce(raysBuffer, raysBuffer1);
            else
                Bounce(raysBuffer1, raysBuffer);
        }

        if(handleComputeRay.IsCompleted)
        {
            handleComputeRay.Complete();
        }
    }



    private void OnDisable()
    {
        jobSpheres.Dispose();
        raysBuffer.Dispose();
        hits.Dispose();
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
