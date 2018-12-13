using UnityEngine;
using System.Collections;
using Unity.Collections;
using Unity.Jobs;
using System;

[Serializable]
public struct jRay
{
    public Vector3 origin;
    public Vector3 direction;
}

[Serializable]
public struct jHitRes
{
    public byte bHit;
    public Vector3 point;
    public Vector3 normal;
}

[Serializable]
public struct jSphere
{
    public Vector3 center;
    public float radius;
}

public struct rtJob : IJobParallelFor
{
    public int batchHitCount;
    public int ns;
    public float t_min, t_max;
    [ReadOnly]
    public NativeArray<jRay> rays;
    [ReadOnly]
    public NativeArray<jSphere> objs;
    public NativeArray<jHitRes> res;

    public void Execute(int index)
    {
        //每一个index对应batchHitCount个hit结果
        for (int k = index * batchHitCount; k < (index + 1) * batchHitCount; k++)
        {
            for (int s = 0; s < ns; s++)
            {
                for (int i = 0; i < objs.Length; i++)
                {
                    jRay r = rays[k];
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
                            res[k] = new jHitRes()
                            {
                                point = p,
                                normal = (p - objs[i].center) / objs[i].radius,
                                bHit = 1
                            };
                        }
                        temp = (-b + Mathf.Sqrt(b * b - a * c)) / a;
                        if (temp < t_max && temp > t_min)
                        {
                            Vector3 p = r.origin + temp * r.direction;
                            res[k] = new jHitRes()
                            {
                                point = p,
                                normal = (p - objs[i].center) / objs[i].radius,
                                bHit = 1
                            };
                        }
                        break;
                    }
                    else
                    {
                        res[k] = new jHitRes()
                        {
                            bHit = 0
                        };
                    }
                }
            }
        }
        
    }
}




