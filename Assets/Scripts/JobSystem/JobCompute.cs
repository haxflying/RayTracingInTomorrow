using UnityEngine;
using System.Collections;
using Unity.Collections;
using Unity.Jobs;
using System;

public struct RaytracingJob : IJobParallelFor
{
    public int ns;
    public float t_min, t_max;
    [ReadOnly]
    public NativeArray<jSphere> objs;
    [ReadOnly]
    public NativeArray<jRay> sourceRays;
    [WriteOnly]
    public NativeArray<jRay> destRays;

    public void Execute(int index)
    {
        if (sourceRays[index].bAlive == 0)
        {
            destRays[index] = new jRay()
            {
                bAlive = 0,
                color = sourceRays[index].color
            };
            return;
        }

        float closestSoFar = t_max;
        bool hitAnything = false;
        for (int i = 0; i < objs.Length; i++)
        {
            jRay r = sourceRays[index];
            jCommonMaterial material = objs[i].mat;
            float t = t_max;
            jRay or = new jRay();
            if(objs[i].HitSphere(r, t_min, closestSoFar, material, out or, out t))
            {
                hitAnything = true;
                closestSoFar = t;
                destRays[index] = or;
            }
            
        }
       
        if(!hitAnything)
        {
            destRays[index] = new jRay()
            {
                bAlive = 0,
                color = sourceRays[index].color * JobGlobal.envColor
            };
        }
        
    }

}



