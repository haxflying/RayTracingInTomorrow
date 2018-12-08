﻿using System.Collections;
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

    public zCamera()
    {
        lower_left_corner = new Vector3(-2f, -1f, -1f);
        horizontal = new Vector3(4f, 0f, 0f);
        vertical = new Vector3(0f, 2f, 0f);
        origin = Vector3.zero;
        useDoF = false;
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
        useDoF = false;
    }

    public zCamera(Vector3 lookFrom, Vector3 lookAt, Vector3 vup, float vfov, float aspect, float aperture, float focus_dist, float t0, float t1)
    {
        lens_radius = aperture / 2f;
        
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

    public zRay get_ray(float s, float t)
    {
        Vector3 rd = lens_radius * Random.insideUnitCircle;
        Vector3 offset = useDoF? u * rd.x + v * rd.y : Vector3.zero;
        float time = time0 + zRandom.drand() * (time1 - time0);
        return new zRay(origin + offset, lower_left_corner + s * horizontal + t * vertical - origin - offset, time);
    }
}
