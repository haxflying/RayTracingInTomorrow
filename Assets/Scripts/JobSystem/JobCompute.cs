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
    public NativeArray<jRay> rays;
    public NativeArray<jHitRes> res;

    public void Execute(int index)
    {
        Vector3 sum_p = Vector3.zero;
        Vector3 sum_n = Vector3.zero;
        byte hitAnything = 0;
        for (int s = 0; s < ns; s++)
        {
            for (int i = 0; i < objs.Length; i++)
            {
                jRay r = rays[index * ns + s];
                Vector3 oc = r.origin - objs[i].center;
                float a = Vector3.Dot(r.direction, r.direction);
                float b = Vector3.Dot(oc, r.direction);
                float c = Vector3.Dot(oc, oc) - objs[i].radius * objs[i].radius;
                float discriminant = b * b - a * c;
                if (discriminant > 0)
                {
                    float temp = (-b - Mathf.Sqrt(b * b - a * c)) / a;
                    if (temp < t_max && temp > t_min)
                    {
                        Vector3 p = r.origin + temp * r.direction;
                        sum_p += p;
                        sum_n += (p - objs[i].center) / objs[i].radius;
                        hitAnything = 1;
                        r.bounceCount++;
                    }
                    temp = (-b + Mathf.Sqrt(b * b - a * c)) / a;
                    if (temp < t_max && temp > t_min)
                    {
                        Vector3 p = r.origin + temp * r.direction;
                        sum_p += p;
                        sum_n += (p - objs[i].center) / objs[i].radius;
                        hitAnything = 1;
                        r.bounceCount++;
                    }
                    break;
                }
            }
        }
        res[index] = new jHitRes()
        {
            point = sum_p / (float)ns,
            normal = sum_n / (float)ns,
            bHit = hitAnything
        };
    }


}



