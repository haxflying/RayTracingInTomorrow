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

public class zRay
{
    private Vector3 A, B;

    public zRay() { }

    public zRay(Vector3 a, Vector3 b)
    {
        A = a;
        B = b;
    }

    public Vector3 origin
    {
        get
        {
            return A;
        }
    }

    public Vector3 direction
    {
        get
        {
            return B.normalized;
        }
    }

    public Vector3 point_at_parameter(float t)
    {
        return A + t * B.normalized;
    }
}

public abstract class Hitable {
    public zMaterial material;
    public abstract bool hit(zRay r, float t_min, float t_max, ref hit_record rec);
}

public class sphere : Hitable
{
    public Vector3 center;
    public float radius; 

    public sphere(Vector3 cen, float r, zMaterial mat)
    {
        center = cen;
        radius = r;
        material = mat;
    }

    public override bool hit(zRay r, float t_min, float t_max, ref hit_record rec)
    {
        Vector3 oc = r.origin - center;
        float a = Vector3.Dot(r.direction, r.direction);
        float b = Vector3.Dot(oc, r.direction);
        float c = Vector3.Dot(oc, oc) - radius * radius;
        float discriminant = b * b - a * c;
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
                rec.mat = list[i].material;
            }
        }
        return hit_anything;
    }
}
