using UnityEngine;
using System.Collections;
using Unity.Collections;
using Unity.Jobs;
using System;


[Serializable]
public struct jCommonMaterial
{
    public Color albedo;
    public float distance;
}

public struct ComputeBounceRayJob : IJobParallelFor
{
    [ReadOnly]
    public int ns;
    [ReadOnly]
    public jCommonMaterial material;
    [ReadOnly]
    public NativeArray<jRay> sourceRay;
    [ReadOnly]
    public NativeArray<jHitRes> hits;
    [WriteOnly]
    public NativeArray<jRay> bounceRay;

    public void Execute(int index)
    {
        int hitIndex = index / ns;
        if (hits[hitIndex].bHit == 0)
        {
            bounceRay[index] = new jRay()
            {
                color = JobGlobal.envColor
            };
        }
        else
        {
            Vector3 normal = hits[hitIndex].normal;
            Vector3 point = hits[hitIndex].point;
            Vector3 refl = JobGlobal.emit(sourceRay[index], hits[hitIndex], material.distance);
            bounceRay[index] = new jRay()
            {
                origin = point,
                direction = refl,
                bounceCount = sourceRay[index].bounceCount,
                color = material.albedo * sourceRay[index].color
            };
        }
    }
}
