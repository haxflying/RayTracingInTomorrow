using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "diffuseLight", menuName = "RTLight/diffuse")]
public class diffuseLight : zMaterial
{
    public Texture2D emitTex;
    [ColorUsage(false, true)]
    public Color color;

    public diffuseLight(Color col)
    {
        color = col;
    }

    public diffuseLight(Texture2D tex)
    {
        emitTex = tex;
    }

    public override bool scatter(zRay r_in, hit_record rec, ref Vector3 attenuation, ref zRay scattered)
    {
        return false;
    }

    public override Color emitted(float u, float v)
    {
        if(emitTex != null)
        {
            return emitTex.GetPixel((int)u, (int)v) * color;
        }
        else
        {
            return color;
        }
    }

    public override Color emitted()
    {
        return color;
    }
}
