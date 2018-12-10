using UnityEngine;
using System.Collections;
using System;

public abstract class zMaterial : ScriptableObject
{
    public Color albedo;
    public abstract bool scatter(zRay r_in, hit_record rec, ref Vector3 attenuation, ref zRay scattered);
    public virtual Color emitted(float u, float v)
    {
        return Color.black;
    }
    public virtual Color emitted()
    {
        return Color.black;
    }
}


