using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SceneGenerator : MonoBehaviour {

    public List<zMaterial> materials;
    public Vector3 origin = new Vector3(0f, 0f, -4f);
    [Range(1, 100)]
    public int ObjCount = 1;
    [Range(5f, 100f)]
    public float range = 10f;


    public void Generate()
    {
        for (int i = 0; i < ObjCount; i++)
        {
            int index = Random.Range(0, materials.Count);
            zMaterial mat = materials[index];

            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.AddComponent<rtObject>().material = mat;
            Vector3 pos = origin;
            do
            {
                pos = Random.insideUnitSphere * range;
            } while (pos.y < 0f);
            go.transform.position = pos;
            go.transform.localScale = Vector3.one * Random.Range(0.2f, 3f);
            go.transform.parent = transform;
            go.GetComponent<rtObject>().DoPreview();
        }
    }

    public void Clear()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject.DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
}
