using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "dielc", menuName = "RTMat/dielc")]
public class dielectricMaterial : zMaterial
{
    [Range(1f, 2f)]
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

        if (Vector3.Dot(r_in.direction, rec.normal) > 0)
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

        if (r_in.direction.refract(outward_normal, ni_over_nt, ref refracted))
        {
            reflect_prob = rtUtils.schlick(cosine, ref_idx);
        }
        else
        {
            scattered = new zRay(rec.p, reflected, r_in.time);
            reflect_prob = 1f;
        }

        if (zRandom.drand() < reflect_prob)
        {
            scattered = new zRay(rec.p, reflected, r_in.time);
        }
        else
        {
            scattered = new zRay(rec.p, refracted, r_in.time);
        }

        return true;
    }
}

