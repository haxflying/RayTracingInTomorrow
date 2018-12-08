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
}



public abstract class Hitable {
    public zMaterial material;
    public abstract bool hit(zRay r, float t_min, float t_max, ref hit_record rec);
}



public class hitable_list : Hitable
{
    public List<Hitable> list;
    public hitable_list(List<Hitable> l)
    {
        list = l;
    }

    public override bool hit(zRay r, float t_min, float t_max, ref hit_record rec)
    {
        hit_record temp_rec = new hit_record();
        bool hit_anything = false;
        float closest_so_far = t_max;
        for (int i = 0; i < list.Count; i++)
        {
            if(list[i].hit(r, t_min, closest_so_far, ref temp_rec))
            {
                hit_anything = true;
                closest_so_far = temp_rec.t;
                rec = temp_rec;
                rec.mat = list[i].material;
            }
        }
        return hit_anything;
    }
}
