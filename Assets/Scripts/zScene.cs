using UnityEngine;
using System.Collections.Generic;

public class zScene
{
    public static Hitable random_Scene()
    {
        int n = 500;
        List<Hitable> list = new List<Hitable>();
        list.Add(new sphere(new Vector3(0, -1000f, 0f), 1000f, new lambertMaterial(new Vector3(0.5f, 0.5f, 0.5f))));
        //for (int a = -11; a < 11; a++)
        //{
        //    for (int b = -11; b < 11; b++)
        //    {
        //        float choose_mat = zRandom.drand();
        //        Vector3 center = new Vector3(a + 0.9f * zRandom.drand(), 0.2f, b + 0.9f * zRandom.drand());
        //        if ((center - new Vector3(4f, 0.2f, 0f)).magnitude > 0.9)
        //        {
        //            if (choose_mat < 0.8)
        //            {
        //                list.Add(new sphere(center, 0.2f, new lambertMaterial(Random.insideUnitSphere)));
        //            }
        //            else if (choose_mat < 0.95)
        //            {
        //                list.Add(new sphere(center, 0.2f, new metaMaterial(Random.insideUnitSphere, 0.5f * zRandom.drand())));
        //            }
        //            else
        //            {
        //                list.Add(new sphere(center, 0.2f, new dielectricMaterial(1.5f)));
        //            }
        //        }
        //    }
        //}

        list.Add(new sphere(new Vector3(0f, 1f, 0f), 1f, new dielectricMaterial(1.5f)));
        list.Add(new sphere(new Vector3(-4f, 1f, 0f), 1f, new lambertMaterial(new Vector3(0.4f, 0.2f, 0.1f))));
        list.Add(new sphere(new Vector3(4, 1, 0), 1f, new metaMaterial(new Vector3(0.7f, 0.6f, 0.5f), 0f)));

        return new hitable_list(list);
    }

    public static Hitable moving_obj()
    {
        var s1 = new sphere(new Vector3(0, -1000f, 0f), 1000f, new lambertMaterial(new Vector3(0.5f, 0.5f, 0.5f)));
        var s2 = new sphere(new Vector3(-1f, 1f, 0f), 1f, new lambertMaterial(new Vector3(0.4f, 0.2f, 0.1f)));
        var s3 = new moving_sphere(new Vector3(0f, 1f, 0f), new Vector3(0.5f, 1f, 0f), 0f, 1f, 1f, new lambertMaterial(new Vector3(0.4f, 0.8f, 0.1f)));
        return new hitable_list(new List<Hitable>() { s1, s2, s3 });
    }
}
