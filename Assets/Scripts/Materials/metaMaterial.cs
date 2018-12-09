using UnityEngine;
using System.Collections;


[CreateAssetMenu(fileName = "metal", menuName = "RTMat/metal")]
public class metaMaterial : zMaterial
{
    [Range(0f, 1f)]
    public float smoothness;

    public metaMaterial(Vector3 a, float smoothness = 1f)
    {
        albedo = a.ToColor();
        this.smoothness = smoothness;
    }

    public override bool scatter(zRay r_in, hit_record rec, ref Vector3 attenuation, ref zRay scattered)
    {
        Vector3 reflected = Vector3.Reflect(r_in.direction.normalized, rec.normal);
        scattered = new zRay(rec.p, reflected + (1f - smoothness) * zRandom.random_in_unit_sphere(), r_in.time);
        attenuation = albedo.ToVector3();
        return (Vector3.Dot(scattered.direction, rec.normal) > 0);
    }
}