using UnityEngine;
using System.Collections;
using System;

public abstract class zMaterial
{
    public Vector3 albedo;
    public abstract bool scatter(zRay r_in, hit_record rec, ref Vector3 attenuation, ref zRay scattered);
}

public class lambertMaterial : zMaterial
{
    public lambertMaterial(Vector3 a)
    {
        albedo = a;
    }

    public override bool scatter(zRay r_in, hit_record rec, ref Vector3 attenuation, ref zRay scattered)
    {
        Vector3 target = rec.p + rec.normal + zRandom.random_in_unit_sphere();
        scattered = new zRay(rec.p, target - rec.p);
        attenuation = albedo;
        return true;
    }
}

public class metaMaterial : zMaterial
{  
    public float smoothness;

    public metaMaterial(Vector3 a, float smoothness = 1f)
    {
        albedo = a;
        this.smoothness = smoothness;
    }

    public override bool scatter(zRay r_in, hit_record rec, ref Vector3 attenuation, ref zRay scattered)
    {
        Vector3 reflected = Vector3.Reflect(r_in.direction.normalized, rec.normal);
        scattered = new zRay(rec.p, reflected + (1f - smoothness) * zRandom.random_in_unit_sphere());
        attenuation = albedo;
        return (Vector3.Dot(scattered.direction, rec.normal) > 0);
    }
}

public class dielectricMaterial : zMaterial
{
    public float ref_idx;
    public dielectricMaterial(float ri)
    {
        ref_idx = ri;
    }

    public override bool scatter(zRay r_in, hit_record rec, ref Vector3 attenuation, ref zRay scattered)
    {
        Vector3 outward_normal = Vector3.zero;
        Vector3 reflected = Vector3.Reflect(r_in.direction, rec.normal);
        float ni_over_nt = 0;
        attenuation = Vector3.one;
        Vector3 refracted = Vector3.zero;
        float reflect_prob = 0f;
        float cosine = 0f;

        if(Vector3.Dot(r_in.direction, rec.normal) > 0)
        {
            outward_normal = -rec.normal;
            ni_over_nt = ref_idx;
            cosine = ref_idx * Vector3.Dot(r_in.direction, rec.normal);
        }
        else
        {
            outward_normal = rec.normal;
            ni_over_nt = 1f / ref_idx;
            cosine = -Vector3.Dot(r_in.direction, rec.normal);
        }

        if(r_in.direction.refract(outward_normal, ni_over_nt, ref refracted))
        {
            reflect_prob = rtUtils.schlick(cosine, ref_idx);
        }
        else
        {
            scattered = new zRay(rec.p, reflected);
            reflect_prob = 1f;
        }

        if(zRandom.drand() < reflect_prob)
        {
            scattered = new zRay(rec.p, reflected);
        }
        else
        {
            scattered = new zRay(rec.p, refracted);
        }

        return true;
    }
}
