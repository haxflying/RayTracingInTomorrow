using UnityEngine;
using System.Collections;
using Unity.Collections;
using Unity.Jobs;
using System;

[Serializable]
public struct jRay
{
    public int bounceCount;
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
}

public static class JobGlobal
{
    public static Color envColor;
    public static int currentDepth = 0;

    public static Vector3 emit(jRay r, jHitRes p, float dist = 0f)
    {
        Vector3 refl = Vector3.Reflect(r.direction, p.normal);
        return refl + dist * zRandom.random_in_unit_sphere();
    }
}



