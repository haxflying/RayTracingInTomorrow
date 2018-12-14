using UnityEngine;
using System.Collections;
using Unity.Collections;
using Unity.Jobs;
using System;


[Serializable]
public struct jlambertMaterial
{
    public Color albedo;
}

public struct ComputeBounceRayJob : IJobParallelFor
{
    [ReadOnly]
    public NativeArray<jRay> sourceRay;
    [ReadOnly]
    public NativeArray<jHitRes> hits;
    public NativeArray<jRay> bounceRay;

    public void Execute(int index)
    {                     

    }
}
