using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "lambert", menuName = "RTMat/lambert")]
public class lambertMaterial : zMaterial
{
    Texture2D mainTex;

    public lambertMaterial(Vector3 a)
    {
        albedo = a.ToColor();
    }

    public override bool scatter(zRay r_in, hit_record rec, ref Vector3 attenuation, ref zRay scattered)
    {
        Vector3 target = rec.p + rec.normal + zRandom.random_in_unit_sphere();
        scattered = new zRay(rec.p, target - rec.p, r_in.time);
        attenuation = albedo.ToVector3();
        return true;
    }
}
