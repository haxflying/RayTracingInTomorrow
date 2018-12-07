using UnityEngine;
using System.Collections;

public static class rtUtils 
{
    public static Color ToColor(this Vector3 vec3)
    {
        return new Color(vec3.x, vec3.y, vec3.z);
    }

    public static bool refract(this Vector3 v, Vector3 n, float ni_over_nt, ref Vector3 refracted)
    {
        Vector3 uv = v.normalized;
        float dt = Vector3.Dot(uv, n);
        float discriminant = 1f - ni_over_nt * ni_over_nt * (1 - dt * dt);
        if (discriminant > 0)
        {
            refracted = ni_over_nt * (v - n * dt) - n * Mathf.Sqrt(discriminant);
            return true;
        }
        else
            return false;
    }

    public static float schlick(float cos, float ref_idx)
    {
        float r0 = (1 - ref_idx) / (1 + ref_idx);
        r0 = r0 * r0;
        return r0 + (1 - r0) * Mathf.Pow((1 - cos), 5);
    }
}
