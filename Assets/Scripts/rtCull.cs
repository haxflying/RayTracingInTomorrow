using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rtCull {

    public static List<Hitable> cull(Camera cam, rtObject[] source)
    {
        List<Hitable> res = new List<Hitable>();
        foreach (var obj in source)
        {
            Bounds bd = obj.GetComponent<Renderer>().bounds;
            Vector3 min = cam.WorldToViewportPoint(bd.min);
            Vector3 max = cam.WorldToViewportPoint(bd.max);
            if (vp_check(min) || vp_check(max))
                res.Add(obj.toRtObject());
        }
        return res;
    }

    private static bool vp_check(Vector3 vp_pos)
    {
        return (vp_pos.x > 0 && vp_pos.x < 1 &&
                vp_pos.y > 0 && vp_pos.y < 1 &&
                vp_pos.z > 0);
 
    }

}




















