using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class quad : Hitable
{
    public float x0, x1, y0, y1, k;
    public Vector3 normal;

    private Transform trans;
    private rtAABB aabb;

    public quad() { }

    public quad(zMaterial mat, Transform transform, int instanceID = 0)
    {
        k = 0;
        //Vector3 worldmin = transform.localToWorldMatrix.MultiplyPoint(new Vector3(-0.5f, -0.5f, 0f));
        //Vector3 worldmax = transform.localToWorldMatrix.MultiplyPoint(new Vector3(0.5f, 0.5f, 0f));
        //x0 = Mathf.Min(worldmin.x, worldmax.x);
        //x1 = Mathf.Max(worldmin.x, worldmax.x);
        //z0 = Mathf.Min(worldmin.z, worldmax.z);
        //z1 = Mathf.Max(worldmin.z, worldmax.z);
        x0 = -0.5f;
        x1 = 0.5f;
        y0 = -0.5f;
        y1 = 0.5f;
        material = mat;
        normal = -transform.forward;
        id = instanceID;
        trans = transform;
        Bounds bd = trans.GetComponent<Renderer>().bounds;
        aabb = new rtAABB(bd.min, bd.max);
    }

    public override bool bounding_box(float t0, float t1, ref rtAABB box)
    {
        box = aabb;
        return true;
    }

    public override bool hit(zRay r, float t_min, float t_max, ref hit_record rec)
    {
        if (Vector3.Dot(r.direction, normal) > 0)
            return false;
        r = new zRay(trans.worldToLocalMatrix.MultiplyPoint(r.origin), trans.worldToLocalMatrix.MultiplyVector(r.direction));
        float t = (k - r.origin.z) / r.direction.z;
        if (t < t_min || t > t_max)
            return false;
        float x = r.origin.x + t * r.direction.x;
        float y = r.origin.y + t * r.direction.y;
        if (x < x0 || x > x1 || y < y0 || y > y1)
            return false;
        rec.uv = new Vector2((x - x0) / (x1 - x0), (y - y0) / (y1 - y0));
        rec.t = t * trans.lossyScale.x;
        rec.mat = material;
        rec.p = trans.localToWorldMatrix.MultiplyPoint(r.point_at_parameter(t));
        rec.normal = normal;
        rec.lastHit = id;
        if (DebugDrawer.isDebug)
        {
            DebugDrawer.points.Add(rec.p);
            DebugDrawer.normals.Add(rec.normal);
            //Debug.Log(r.origin + " " + r.direction);
            //Debug.Log("rec.p " + rec.p + " local " + r.point_at_parameter(t));
            //Debug.Log("id " + id);
        }
        return true;
    }
}
