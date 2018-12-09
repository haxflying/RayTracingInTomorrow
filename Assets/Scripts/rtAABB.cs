using UnityEngine;
using System.Collections;

public class rtAABB 
{
    public Vector3 _min, _max;

    public rtAABB() { }

    public rtAABB(Vector3 a, Vector3 b)
    {
        _min = a;
        _max = b;
    }

    public bool hit(zRay r, float tmin, float tmax)
    {
        for (int a = 0; a < 3; a++)
        {
            float invD = 1f / r.direction[a];
            float t0 = (_min[a] - r.origin[a]) * invD;
            float t1 = (_max[a] - r.origin[a]) * invD;
            if (invD < 0f)
                swap(ref t0, ref t1);

            tmin = t0 > tmin ? t0 : tmin;
            tmax = t1 < tmax ? t1 : tmax;
            if (tmax <= tmin)
                return false;//no overlap
        }
        return true;
    }

    public static rtAABB surrounding_box(rtAABB box0, rtAABB box1)
    {
        Vector3 small = Vector3.Min(box0._min, box1._min);
        Vector3 big = Vector3.Max(box0._max, box1._max);
        return new rtAABB(small, big);
    }

    void swap(ref float a, ref float b)
    {
        float temp = a;
        a = b;
        b = temp;
    }
}
