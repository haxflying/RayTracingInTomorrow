using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class triangle_list : Hitable
{
    public List<triangle> list;
    rtAABB aabb;

    public triangle_list(List<triangle> list, Bounds bd, int instanceID = 0)
    {
        this.list = list;
        aabb = new rtAABB(bd.min, bd.max);
        id = instanceID;
    }

    public override bool bounding_box(float t0, float t1, ref rtAABB box)
    {
        box = aabb;
        return true;
    }

    public override bool hit(zRay r, float t_min, float t_max, ref hit_record rec)
    {
        if (rec.lastHit == id)
            return false;
        List<hit_record> recs = new List<hit_record>();
        foreach(var tri in list)
        {
            if(tri.hit(r, t_min, t_max, ref rec))
            {
                recs.Add(rec);
            }
        }

        if (recs.Count == 0)
            return false;

        float tmin = float.MaxValue;
        int tminIndex = 0;
        for (int i = 0; i < recs.Count; i++)
        {
            if(recs[i].t < tmin)
            {
                tmin = recs[i].t;
                tminIndex = i;
            }
        }

        rec = recs[tminIndex];
        return true;
    }
}
