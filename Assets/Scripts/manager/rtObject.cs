using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class rtObject : MonoBehaviour {

    public bool preview;
    public zMaterial material;
    public bool isPlane;

    public bool isMoving;

    private Material mat;

    public sphere toSphere()
    {
        float radius = isPlane ? 1000f : transform.localScale.x / 2f;
        Vector3 center = isPlane ? -1000f * Vector3.up : transform.position;
        return new sphere(center, radius, material);
    }

    private void Update()
    {
        if(preview)
        {
            if (gameObject.name != material.name)
                gameObject.name = material.name;

            if (mat == null)
            {
                mat = new Material(Shader.Find("Custom/Mark"));
                GetComponent<Renderer>().material = mat;
            }

            if (material is lambertMaterial)
            {
                mat.SetColor("_Color", material.albedo);
                mat.SetFloat("_Metallic", 0f);
                mat.SetFloat("_Glossiness", 0f);
            }
            else if(material is metaMaterial)
            {
                mat.SetColor("_Color", material.albedo);
                mat.SetFloat("_Metallic", 1f);
                mat.SetFloat("_Glossiness", (material as metaMaterial).smoothness);
            }
            else if(material is dielectricMaterial)
            {

            }
        }
    }
}
