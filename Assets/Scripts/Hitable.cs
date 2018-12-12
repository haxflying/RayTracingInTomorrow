using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct hit_record
{
    public float t;
    public Vector3 p;
    public Vector3 normal;
    public zMaterial mat;
    public int lastHit;
    public Vector2 uv;
}



public abstract class Hitable{
    public int id;
    public zMaterial material;
    public abstract bool hit(zRay r, float t_min, float t_max, ref hit_record rec);
    public abstract bool bounding_box(float t0, float t1, ref rtAABB box);
    public virtual Vector3 origin() { return Vector3.one * 1000f; }
}



public class hitable_list : Hitable
{
    public List<Hitable> list;
    public BVH_Node bvh;
    public bool usBvh;
    public hitable_list(List<Hitable> l, float t0 = 0f, float t1 = 0f, bool useBvh = true)
    {
        list = l;
        bvh = new BVH_Node(l, t0, t1);
        this.usBvh = useBvh;
    }

    public override bool bounding_box(float t0, float t1, ref rtAABB box)
    {
        if (list.Count < 1) return false;

        rtAABB temp_box = new rtAABB();
        bool first_true = list[0].bounding_box(t0, t1, ref temp_box);
        if (!first_true)
            return false;
        else
            box = temp_box;

        for (int i = 0; i < list.Count; i++)
        {
            if (list[0].bounding_box(t0, t1, ref temp_box))
            {
                box = rtAABB.surrounding_box(box, temp_box);
            }
            else
                return false;
        }
        return true;
    }

    public override bool hit(zRay r, float t_min, float t_max, ref hit_record rec)
    {
        hit_record temp_rec = new hit_record();
        bool hit_anything = false;
        float closest_so_far = t_max;

        if (!usBvh)
        {
            float minDis = float.MaxValue;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].hit(r, t_min, closest_so_far, ref temp_rec))
                {
                    float dis = (temp_rec.p - r.origin).magnitude;
                    if (dis < minDis)
                    {
                        hit_anything = true;
                        closest_so_far = temp_rec.t;
                        rec = temp_rec;
                        rec.mat = list[i].material;
                        minDis = dis;
                    }
                }
            }
        }
        else
        {
            hit_anything = bvh.hit(r, t_min, t_max, ref rec);
        }
        return hit_anything;
    }
}
