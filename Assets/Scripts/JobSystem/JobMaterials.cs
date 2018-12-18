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
        Vector3 point = hits[hitIndex].point;
        //Debug.Log("bounce" + hits[hitIndex].bHit);
        //Debug.Log(hits[hitIndex].bHit + " " + sourceRay[index].color);
        if (hits[hitIndex].bHit == 0)
        {
            Color col;
            //if (JobGlobal.envColor == sourceRay[index].color)
            //    col = sourceRay[index].color;
            //else
            col = JobGlobal.envColor;// * sourceRay[index].color;
            bounceRay[index] = new jRay()
            {
                origin = point,
                direction = sourceRay[index].direction,               
                bAlive = sourceRay[index].bAlive,
                color = col
            };
            
        }
        else
        {
            //Vector3 normal = hits[hitIndex].normal;          
            //Vector3 refl = JobGlobal.emit(sourceRay[index], hits[hitIndex], material.distance);
            //bounceRay[index] = new jRay()
            //{
            //    origin = point,
            //    direction = refl,
            //    bAlive = sourceRay[index].bAlive + 1,
            //    color = material.albedo * sourceRay[index].color
            //};
        }
    }
}
