using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class rtObject : MonoBehaviour {

    public enum ObjType
    {
        Sphere, TriangleMesh
    }

    public ObjType type;

    public zMaterial material;
    public bool isPlane;
    public bool isMoving;
    public int instanceID;

    private Material mat;

    public Hitable toRtObject()
    {
        instanceID = gameObject.GetInstanceID();
        if (type == ObjType.Sphere)
        {
            float radius = isPlane ? 1000f : transform.lossyScale.x / 2f;
            Vector3 center = isPlane ? -1000f * Vector3.up : transform.position;
            return new sphere(center, radius, material, instanceID);
        }
        else
        { 
            Bounds bd = GetComponent<Renderer>().bounds;
            Vector3[] vertices = GetComponent<MeshFilter>().sharedMesh.vertices;
            int[] indices = GetComponent<MeshFilter>().sharedMesh.GetIndices(0);
            List<triangle> list = new List<triangle>();
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = transform.localToWorldMatrix.MultiplyPoint(vertices[i]);
            }

            for (int i = 0; i < indices.Length; i += 3)
            {
                triangle tri = new triangle(vertices[indices[i]], vertices[indices[i + 1]], vertices[indices[i + 2]], material, instanceID);
                list.Add(tri);
            }
            return new triangle_list(list, bd, instanceID);
        }
        
    }

    public void DoPreview()
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
        else if (material is metaMaterial)
        {
            mat.SetColor("_Color", material.albedo);
            mat.SetFloat("_Metallic", 1f);
            mat.SetFloat("_Glossiness", (material as metaMaterial).smoothness);
        }
        else if (material is dielectricMaterial)
        {

        }
        else if(material is diffuseLight)
        {

        }
    }

    public void test()
    {
        Vector3[] vertices = GetComponent<MeshFilter>().sharedMesh.vertices;
        foreach(var v in vertices)
        {
            print(v);
        }
    }
}
