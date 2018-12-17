using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zRandom {

	static ushort[] FaurePermutation = new ushort[]{ 0, 75, 50, 25, 100, 15, 90, 65, 40, 115, 10, 85, 60, 35, 110, 5, 80, 55,
    30, 105, 20, 95, 70, 45, 120, 3, 78, 53, 28, 103, 18, 93, 68, 43, 118, 13, 88, 63, 38, 113, 8, 83, 58, 33, 108,
    23, 98, 73, 48, 123, 2, 77, 52, 27, 102, 17, 92, 67, 42, 117, 12, 87, 62, 37, 112, 7, 82, 57, 32, 107, 22, 97,
    72, 47, 122, 1, 76, 51, 26, 101, 16, 91, 66, 41, 116, 11, 86, 61, 36, 111, 6, 81, 56, 31, 106, 21, 96, 71, 46,
    121, 4, 79, 54, 29, 104, 19, 94, 69, 44, 119, 14, 89, 64, 39, 114, 9, 84, 59, 34, 109, 24, 99, 74, 49, 124 };

    public static double Halton5(uint Index)
    {
        return (FaurePermutation[Index % 125u] * 1953125u + FaurePermutation[(Index / 125u) % 125u] * 15625u +
        FaurePermutation[(Index / 15625u) % 125u] * 125u +
        FaurePermutation[(Index / 1953125u) % 125u]) * ((1u - 0.000000000000001f) / 244140625u);
    }

    static long seed = 1;
    public static float drand()
    {
        seed = (0x5DEECE66DL * seed + 0xB16) & 0xFFFFFFFFFFFFL;
        return (seed >> 16) / (float)0x100000000L;
    }


    public static Vector3 random_in_unit_sphere()
    {
        return new Vector3(drand(), drand(), drand()) * 2 - Vector3.one;
        //return Random.insideUnitSphere;
        //Vector3 p;
        //do
        //{
        //    p = 2f * new Vector3((float)zRandom.Halton5(ii++), (float)zRandom.Halton5(ii++), (float)zRandom.Halton5(ii++)) - Vector3.one;
        //    p = 2f * new Vector3(zRandom.drand(), zRandom.drand(), zRandom.drand()) - Vector3.one;
        //    p = 2f * Random.insideUnitSphere - Vector3.one;
        //} while (Vector3.Dot(p, p) >= 1f);
        //return p;
    }
}
