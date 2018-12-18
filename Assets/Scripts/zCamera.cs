using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zCamera {

    public Vector3 origin;
    public Vector3 lower_left_corner;
    public Vector3 horizontal;
    public Vector3 vertical;
    public float lens_radius;
    public float time0, time1;
    Vector3 u, v, w;
    bool useDoF;

    public zCamera(Camera cam, float aperture, float focus_dist, float t0, float t1)
    {
        lens_radius = aperture / 2f;

        Vector3 lookFrom = cam.transform.position;
        Vector3 lookAt = cam.transform.forward;
        Vector3 vup = cam.transform.up;
        float vfov = cam.fieldOfView;
        float aspect = (float)Screen.width / (float)Screen.height;

        float theta = vfov * Mathf.Deg2Rad;
        float half_height = Mathf.Tan(theta / 2f);
        float half_width = aspect * half_height;

        origin = lookFrom;
        w = (lookFrom - lookAt).normalized;
        u = Vector3.Cross(vup, w).normalized;
        v = Vector3.Cross(w, u);

        lower_left_corner = origin - half_width * focus_dist * u - half_height * focus_dist * v - focus_dist * w;
        horizontal = 2f * half_width * u * focus_dist;
        vertical = 2f * half_height * v * focus_dist;
        useDoF = true;

        time0 = t0;
        time1 = t1;
    }

    public zCamera(Camera cam)
    {
        Vector3 lookFrom = cam.transform.position;
        Vector3 lookAt = -cam.transform.forward;
        Vector3 vup = cam.transform.up;
        float vfov = cam.fieldOfView;
        float aspect = (float)Screen.width / (float)Screen.height;

        float theta = vfov * Mathf.Deg2Rad;
        float half_height = Mathf.Tan(theta / 2f);
        float half_width = aspect * half_height;

        origin = lookFrom;
        w = lookAt.normalized;
        u = Vector3.Cross(vup, w).normalized;
        v = Vector3.Cross(w, u);

        lower_left_corner = origin - half_width * u - half_height * v - w;
        horizontal = 2f * half_width * u;
        vertical = 2f * half_height * v;
        useDoF = false;

        time0 = time1 = 0f;
        lens_radius = 0f;
    }

    public zRay get_ray(float s, float t)
    {
        Vector3 rd = lens_radius * Random.insideUnitCircle;
        Vector3 offset = useDoF? u * rd.x + v * rd.y : Vector3.zero;
        float time = time0 + zRandom.drand() * (time1 - time0);
        return new zRay(origin + offset, lower_left_corner + s * horizontal + t * vertical - origin - offset, time);
    }

    public jRay get_jobRay(float s, float t)
    {
        return new jRay()
        {
            origin = origin,
            direction = lower_left_corner + s * horizontal + t * vertical - origin,
            bAlive = 1,
            color = Color.white
        };
    }
}
