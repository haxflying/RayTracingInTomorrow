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
        for (int i = 0; i < objs.Length; i++)
        {
            int rayIndex = index;
            jRay r = sourceRays[rayIndex];
            jCommonMaterial material = objs[i].mat;
            if (r.bAlive == 1)
            {
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
                        Vector3 n = (p - objs[i].center) / objs[i].radius;
                        Vector3 refl = JobGlobal.emit(r.direction, n, material.distance);

                        destRays[rayIndex] = new jRay()
                        {
                            bAlive = 1,
                            origin = p,
                            direction = refl,
                            color = r.color * material.albedo
                        };
                        break;                      

                    }

                    temp = (-b + Mathf.Sqrt(b * b - a * c)) / a;
                    if(temp < t_max && temp > t_min)
                    {
                        Vector3 p = r.origin + temp * r.direction;
                        Vector3 n = (p - objs[i].center) / objs[i].radius;
                        Vector3 refl = JobGlobal.emit(r.direction, n, material.distance);

                        destRays[rayIndex] = new jRay()
                        {
                            bAlive = 1,
                            origin = p,
                            direction = refl,
                            color = r.color * material.albedo
                        };
                        break;
                        
                    }
                }
                else
                {
                    destRays[rayIndex] = new jRay()
                    {
                        bAlive = 0,
                        color = r.color * JobGlobal.envColor
                    };
                }
            }
            else
            {
                destRays[rayIndex] = new jRay()
                {
                    bAlive = 0,
                    color = r.color
                };
            }

        }
        
    }


}



