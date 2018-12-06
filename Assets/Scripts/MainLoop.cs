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
        Vector3 lower_left_corner = new Vector3(-2f, -1f, -1f);
        Vector3 horizontal = new Vector3(4f, 0f, 0f);
        Vector3 vertical = new Vector3(0f, 2f, 0f);
        Vector3 origin = new Vector3(0f, 0f, 0f);
        
        var s1 = new sphere(new Vector3(0f, 0f, -1f), 0.5f);
        var s2 = new sphere(new Vector3(0f, -100.5f, -1f), 100f);
        List<Hitable> list = new List<Hitable>() { s1, s2};
        Hitable world = new hitable_list(list);

        for (int j = ny - 1; j >= 0; j--)
        {
            for (int i = 0; i < nx; i++)
            {
                float u = (float)(i) / (float)(nx);
                float v = (float)(j) / (float)(ny);
                zRay r = new zRay(origin, lower_left_corner + u * horizontal + v * vertical);
                Color col = color(r, world);
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

    Color color(zRay r, Hitable world)
    {
        hit_record rec = new hit_record();
        if(world.hit(r, 0.0f, float.MaxValue, ref rec))
        {
            return 0.5f * new Color(rec.normal.x + 1, rec.normal.y + 1, rec.normal.z + 1);
        }
        else
        {
            float t = 0.5f * (r.direction.normalized.y + 1f);
            return (1f - t) * Color.white + t * new Color(0.5f, 0.7f, 1.0f);
        }
    }

}

public class zRay
{
    private Vector3 A, B;

    public zRay(Vector3 a, Vector3 b)
    {
        A = a;
        B = b;
    }

    public Vector3 origin
    {
        get
        {
            return A;
        }
    }

    public Vector3 direction
    {
        get
        {
            return B;
        }
    }

    public Vector3 point_at_parameter(float t)
    {
        return A + t * B;
    }
}
