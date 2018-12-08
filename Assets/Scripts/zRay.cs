using UnityEngine;
using System.Collections;

public class zRay
{
    private Vector3 A, B;
    private float _time;

    public zRay() { }

    public zRay(Vector3 a, Vector3 b, float ti = 0)
    {
        A = a;
        B = b;
        _time = ti;
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
            return B.normalized;
        }
    }

    public float time
    {
        get
        {
            return _time;
        }
    }

    public Vector3 point_at_parameter(float t)
    {
        return A + t * B.normalized;
    }
}