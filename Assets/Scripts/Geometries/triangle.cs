using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class triangle : Hitable
{
    public Vector3 a, b, c;
    public Vector3 normal;

    public triangle() { }

    public triangle(Vector3 a, Vector3 b, Vector3 c, zMaterial mat, int instanceId = 0)
    {
        this.a = a;
        this.b = b;
        this.c = c;
        normal = Vector3.Cross(b - a, c - a).normalized;
        material = mat;
        id = instanceId;
    }

    public override bool bounding_box(float t0, float t1, ref rtAABB box)
    {
        Vector3 min = Vector3.Min(Vector3.Min(a, b), c);
        Vector3 max = Vector3.Max(Vector3.Max(a, b), c);
        box = new rtAABB(min, max);
        return true;
    }

    public override bool hit(zRay r, float t_min, float t_max, ref hit_record rec)
    {
        if (Vector3.Dot(normal, r.direction) > 0)
            return false;

        Vector3 e1 = b - a;
        Vector3 e2 = c - a;
        Vector3 p = Vector3.Cross(r.direction, e2);
        float det = Vector3.Dot(e1, p);
        Vector3 T;
        if(det > 0)
        {
            T = r.origin - a;
        }
        else
        {
            T = a - r.origin;
            det = -det;
        }

        if (det < 0.0001f)
            return false;

        float u = Vector3.Dot(T, p);
        if (u < 0f || u > det)
            return false;

        Vector3 Q = Vector3.Cross(T, e1);
        float v = Vector3.Dot(r.direction, Q);
        if (v < 0 || u + v > det)
            return false;

        float t = Vector3.Dot(e2, Q);
        float invDet = 1 / det;
        t *= invDet;
        u *= invDet;
        v *= invDet;

        rec.mat = material;
        rec.t = t;
        rec.p = r.point_at_parameter(t);
        rec.normal = normal;
        rec.lastHit = id;
        if (DebugDrawer.isDebug)
        {
            DebugDrawer.points.Add(rec.p);
            DebugDrawer.normals.Add(rec.normal);
        }
        return true;
    }
}

