using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BVH_Node : Hitable
{
    public Hitable left, right;
    public rtAABB box;
    private List<Hitable> Objlist;

    public BVH_Node() { }
    
    public BVH_Node(List<Hitable> list, float time0, float time1)
    {
        Objlist = list;
        Debug.Log("new bvh node " + list.Count);
        int axis = Mathf.FloorToInt(zRandom.drand() * 3f);

        if (axis == 0)
            list.Sort((a, b) =>
            {
                rtAABB box_left = new rtAABB(), box_right = new rtAABB();
                if (!a.bounding_box(0, 0, ref box_left) || !b.bounding_box(0, 0, ref box_right))
                {
                    Debug.LogError(" no bounding box in bvh_node constructor");
                }
                if (box_left._min.x - box_right._min.x < 0f)
                    return -1;
                else
                    return 1;
            });
        else if(axis == 1)
            list.Sort((a, b) =>
            {
                rtAABB box_left = new rtAABB(), box_right = new rtAABB();
                if (!a.bounding_box(0, 0, ref box_left) || !b.bounding_box(0, 0, ref box_right))
                {
                    Debug.LogError(" no bounding box in bvh_node constructor");
                }
                if (box_left._min.y - box_right._min.y < 0f)
                    return -1;
                else
                    return 1;
            });
        else
            list.Sort((a, b) =>
            {
                rtAABB box_left = new rtAABB(), box_right = new rtAABB();
                if (!a.bounding_box(0, 0, ref box_left) || !b.bounding_box(0, 0, ref box_right))
                {
                    Debug.LogError(" no bounding box in bvh_node constructor");
                }
                if (box_left._min.z - box_right._min.z < 0f)
                    return -1;
                else
                    return 1;
            });

        int n = list.Count;
        if(n == 1)
        {
            left = right = list[0];
        }
        else if(n == 2)
        {
            left = list[0];
            right = list[1];
        }
        else
        {
            Debug.Log("lr " + n / 2);
            left = new BVH_Node(list.GetRange(0, n / 2), time0, time1);
            right = new BVH_Node(list.GetRange(n / 2, n -  n / 2), time0, time1);
        }
        rtAABB box_l = new rtAABB(), box_r = new rtAABB();
        if(!left.bounding_box(time0, time1, ref box_l) || !right.bounding_box(time0, time1, ref box_r))
        {
            Debug.LogError(" no bounding box in bvh_node constructor");
        }
        box = rtAABB.surrounding_box(box_l, box_r);
    }

    public override bool bounding_box(float t0, float t1, ref rtAABB box)
    {
        box = this.box;
        return true;
    }

    public override bool hit(zRay r, float t_min, float t_max, ref hit_record rec)
    {
        if (box.hit(r, t_min, t_max))
        {
            hit_record left_rec = new hit_record(), right_rec = new hit_record();
            bool hit_left = left.hit(r, t_min, t_max, ref left_rec);
            bool hit_right = right.hit(r, t_min, t_max, ref right_rec);
            if (hit_left && hit_right)
            {
                if (left_rec.t < right_rec.t)
                    rec = left_rec;
                else
                    rec = right_rec;
                return true;
            }
            else if (hit_left)
            {
                rec = left_rec;
                return true;
            }
            else if (hit_right)
            {
                rec = right_rec;
                return true;
            }
            else
                return false;
        }
        else
            return false;
    }
}
