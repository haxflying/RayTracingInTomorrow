using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zCamera {

    public Vector3 origin;
    public Vector3 lower_left_corner;
    public Vector3 horizontal;
    public Vector3 vertical;

    public zCamera()
    {
        lower_left_corner = new Vector3(-2f, -1f, -1f);
        horizontal = new Vector3(4f, 0f, 0f);
        vertical = new Vector3(0f, 2f, 0f);
        origin = Vector3.zero;
    }

    public zCamera(Vector3 lookFrom, Vector3 lookAt, Vector3 vup, float vfov, float aspect)
    {
        Vector3 u, v, w;
        float theta = vfov * Mathf.Deg2Rad;
        float half_height = Mathf.Tan(theta / 2f);
        float half_width = aspect * half_height;

        origin = lookFrom;
        w = (lookFrom - lookAt).normalized;
        u = Vector3.Cross(vup, w).normalized;
        v = Vector3.Cross(w, u);

        lower_left_corner = origin - half_width * u - half_height * v - w;
        horizontal = 2f * half_width * u;
        vertical = 2f * half_height * v;

        Debug.Log("w " + w);
        Debug.Log("Horizontal " + horizontal);
        Debug.Log("vertical " + vertical);
        Debug.Log("lb " + lower_left_corner);
        Debug.Log("origin " + origin);
    }

    public zRay get_ray(float u, float v)
    {
        return new zRay(origin, lower_left_corner + u * horizontal + v * vertical - origin);
    }
}
