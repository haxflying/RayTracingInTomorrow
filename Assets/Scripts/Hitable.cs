using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct hit_record
{
    public float t;
    public Vector3 p;
    public Vector3 normal;
}

public abstract class Hitable {

    public abstract bool hit(zRay r, float t_min, float t_max, ref hit_record rec);
}

public class sphere : Hitable
{
    public Vector3 center;
    public float radius;

    public sphere(Vector3 cen, float r)
    {
        center = cen;
        radius = r;
    }

    public override bool hit(zRay r, float t_min, float t_max, ref hit_record rec)
    {
        Vector3 oc = r.origin - center;
        float a = Vector3.Dot(r.direction, r.direction);
        float b = 2f * Vector3.Dot(oc, r.direction);
        float c = Vector3.Dot(oc, oc) - radius * radius;
        float discriminant = b * b - 4 * a * c;
        if(discriminant > 0)
        {
            float temp = (-b - Mathf.Sqrt(b * b - a * c)) / a;
            if(temp < t_max && temp > t_min)
            {
                rec.t = temp;
                rec.p = r.point_at_parameter(rec.t);
                rec.normal = (rec.p - center) / radius;
                return true;
            }
            temp = (-b + Mathf.Sqrt(b * b - a * c)) / a;
            if (temp < t_max && temp > t_min)
            {
                rec.t = temp;
                rec.p = r.point_at_parameter(rec.t);
                rec.normal = (rec.p - center) / radius;
                return true;
            }
        }
        return false;
    }
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
            }
        }
        return hit_anything;
    }
}
