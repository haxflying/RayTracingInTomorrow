using UnityEngine;
using System.Collections;
using Unity.Collections;
using Unity.Jobs;
using System;

[Serializable]
public struct jRay
{
    public byte bAlive;
    public Vector3 origin;
    public Vector3 direction;
    public Color color;
}

[Serializable]
public struct jHitRes
{
    public byte bHit;
    public Vector3 point;
    public Vector3 normal;
}

[Serializable]
public struct jSphere
{
    public Vector3 center;
    public float radius;
    public jCommonMaterial mat;

    public bool HitSphere(jRay r, float tMin, float tMax, jCommonMaterial mat, out jRay or, out float t)
    {
        or = new jRay()
        {
            bAlive = 0,
            color = r.color * JobGlobal.envColor
        };
        t = tMax;
        Vector3 oc = r.origin - center;
        float a = Vector3.Dot(r.direction, r.direction);
        float b = Vector3.Dot(oc, r.direction);
        float c = Vector3.Dot(oc, oc) - radius * radius;
        float discriminant = b * b - a * c;
        if (discriminant <= 0)
            return false;

        float temp = (-b - Mathf.Sqrt(b * b - a * c)) / a;
        if(temp < tMax && temp > tMin)
        {
            Vector3 p = r.origin + temp * r.direction;
            Vector3 n = (p - center) / radius;
            Vector3 refl = JobGlobal.emit(r.direction, n, mat.distance);
            or.bAlive = 1;
            or.origin = p;
            or.direction = refl;
            or.color = r.color * mat.albedo;
            t = temp;
            return true;
        }

        temp = (-b + Mathf.Sqrt(b * b - a * c)) / a;
        if (temp < tMax && temp > tMin)
        {
            Vector3 p = r.origin + temp * r.direction;
            Vector3 n = (p - center) / radius;
            Vector3 refl = JobGlobal.emit(r.direction, n, mat.distance);
            or.bAlive = 1;
            or.origin = p;
            or.direction = refl;
            or.color = r.color * mat.albedo;
            t = temp;
            return true;
        }

        return false;
    }
}

public static class JobGlobal
{
    public static Color envColor;
    public static int currentDepth = 0;

    public static Vector3 emit(Vector3 r, Vector3 n,  float dist = 0f)
    {
        //Vector3 refl = Vector3.Reflect(r, n);
        //return refl + dist * zRandom.random_in_unit_sphere();
        return n + zRandom.random_in_unit_sphere();
    }
}



