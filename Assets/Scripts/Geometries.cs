using UnityEngine;
using System.Collections;

public class sphere : Hitable
{
    public Vector3 center;
    public float radius;

    public sphere(Vector3 cen, float r, zMaterial mat, int instanceID = -1)
    {
        center = cen;
        radius = r;
        material = mat;
        id = instanceID;
    }

    public override bool bounding_box(float t0, float t1, ref rtAABB box)
    {
        box = new rtAABB(center - Vector3.one * radius, center + Vector3.one * radius);
        return true;
    }

    public override bool hit(zRay r, float t_min, float t_max, ref hit_record rec)
    {
        if (rec.lastHit == id)
            return false;
        Vector3 oc = r.origin - center;
        float a = Vector3.Dot(r.direction, r.direction);
        float b = Vector3.Dot(oc, r.direction);
        float c = Vector3.Dot(oc, oc) - radius * radius;
        float discriminant = b * b - a * c;
        if (discriminant > 0)
        {
            rec.mat = material;          
            float temp = (-b - Mathf.Sqrt(b * b - a * c)) / a;
            if (temp < t_max && temp > t_min)
            {
                rec.t = temp;
                rec.p = r.point_at_parameter(rec.t);
                rec.normal = (rec.p - center) / radius;
                rec.lastHit = id;

                if (DebugDrawer.isDebug)
                {
                    DebugDrawer.points.Add(rec.p);
                    DebugDrawer.normals.Add(rec.normal);
                }
                return true;
            }
            temp = (-b + Mathf.Sqrt(b * b - a * c)) / a;
            if (temp < t_max && temp > t_min)
            {
                rec.t = temp;
                rec.p = r.point_at_parameter(rec.t);
                rec.normal = (rec.p - center) / radius;
                rec.lastHit = id;

                if (DebugDrawer.isDebug)
                {
                    DebugDrawer.points.Add(rec.p);
                    DebugDrawer.normals.Add(rec.normal);
                }
                return true;
            }
        }
        return false;
    }
}

public class moving_sphere : Hitable
{
    public Vector3 center0, center1;
    public float time0, time1;
    public float radius;

    public moving_sphere() { }

    public moving_sphere(Vector3 cen0, Vector3 cen1, float t0, float t1, float r, zMaterial m)
    {
        center0 = cen0;
        center1 = cen1;
        time0 = t0;
        time1 = t1;
        material = m;
        radius = r;
    }

    public override bool hit(zRay r, float t_min, float t_max, ref hit_record rec)
    {
        if (rec.lastHit == id)
            return false;
        Vector3 oc = r.origin - center(r.time);
        float a = Vector3.Dot(r.direction, r.direction);
        float b = Vector3.Dot(oc, r.direction);
        float c = Vector3.Dot(oc, oc) - radius * radius;
        float discriminant = b * b - a * c;
        if(discriminant > 0)
        {
            rec.mat = material;
            float temp = (-b - Mathf.Sqrt(discriminant)) / a;
            if(temp < t_max && temp > t_min)
            {
                rec.t = temp;
                rec.p = r.point_at_parameter(rec.t);
                rec.normal = (rec.p - center(r.time)) / radius;
                rec.mat = material;
                rec.lastHit = id;
                return true;
            }
            temp = (-b + Mathf.Sqrt(discriminant)) / a;
            if (temp < t_max && temp > t_min)
            {
                rec.t = temp;
                rec.p = r.point_at_parameter(rec.t);
                rec.normal = (rec.p - center(r.time)) / radius;
                rec.mat = material;
                rec.lastHit = id;
                return true;
            }
        }
        return false;
    }

    public Vector3 center(float time)
    {
        return center0 + (time - time0) / (time1 - time0) * (center1 - center0);
    }

    

    public override bool bounding_box(float t0, float t1, ref rtAABB box)
    {
        Vector3 radiuVec = Vector3.one * radius;
        rtAABB box0 = new rtAABB(center0 - radiuVec, center0 + radiuVec);
        rtAABB box1 = new rtAABB(center1 - radiuVec, center1 + radiuVec);
        box = rtAABB.surrounding_box(box0, box1);
        return true;
    }
}