using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera))]
public class MainLoop : MonoBehaviour {

    private Camera cam;
    private CommandBuffer cb_output;
    private Texture2D rtResult;
	void Start () {
        cam = GetComponent<Camera>();

        cb_output = new CommandBuffer();
        cb_output.name = "RT Result";
        cam.AddCommandBuffer(CameraEvent.AfterEverything, cb_output);

        rtResult = new Texture2D(Screen.width, Screen.height);
        rtResult.name = "ResultTexture";

        int nx = Screen.width;
        int ny = Screen.height;
        int ns = 100;
        
        var s1 = new sphere(new Vector3(0f, 0f, -1f), 0.5f, new lambertMaterial(new Vector3(0.8f, 0.3f, 0.3f)));
        var s2 = new sphere(new Vector3(0f, -100.5f, -1f), 100f, new lambertMaterial(new Vector3(0.8f, 0.8f, 0.0f)));
        var s3 = new sphere(new Vector3(1f, 0f, -1f), 0.5f, new metaMaterial(new Vector3(0.8f, 0.6f, 0.2f), 0.3f));
        var s4 = new sphere(new Vector3(-1f, 0f, -1f), 0.5f, new dielectricMaterial(1.5f));
        var s5 = new sphere(new Vector3(-1f, 0f, -1f), -0.45f, new dielectricMaterial(1.5f));
        List<Hitable> list = new List<Hitable>() { s1, s2, s3, s4, s5};
        Hitable world = new hitable_list(list);
        zCamera zcam = new zCamera(new Vector3(-2f, 2f, 1f), new Vector3(0f, 0f, -1f), new Vector3(0f, 1f, 0f), 90, (float)nx / (float)ny);
        //zCamera zcam = new zCamera();
        uint index = 0;
        for (int j = ny - 1; j >= 0; j--)
        {
            Color col = Color.black;
            for (int i = 0; i < nx; i++)
            {
                for (int s = 0; s < ns; s++)
                {
                    float u = (float)(i + zRandom.Halton5(index++)) / (float)(nx);
                    float v = (float)(j + zRandom.Halton5(index++)) / (float)(ny);
                    zRay r = zcam.get_ray(u, v);
                    col += color(r, world, 0);
                }
                col /= (float)ns;
                col = col.gamma;
                rtResult.SetPixel(i, j, col);
            }
        }
        rtResult.Apply();
    }


    private void OnPreRender()
    {
        if (cb_output == null)
            return;

        cb_output.Clear();
        cb_output.Blit(rtResult, BuiltinRenderTextureType.CameraTarget);
    }    

    Color color(zRay r, Hitable world, int depth)
    {
        hit_record rec = new hit_record();
        if(world.hit(r, 0.0f, float.MaxValue, ref rec))
        {
            zRay scattered = new zRay();
            Vector3 attenuation = Vector3.zero;
            if (depth < 50 && rec.mat.scatter(r, rec, ref attenuation, ref scattered))
            {
                return new Color(attenuation.x, attenuation.y, attenuation.z) * color(scattered, world, depth + 1);
            }
            else
                return Color.black;
        }
        else
        {
            float t = 0.5f * (r.direction.normalized.y + 1f);
            return (1f - t) * Color.white + t * new Color(0.5f, 0.7f, 1.0f);
        }
    }

}

