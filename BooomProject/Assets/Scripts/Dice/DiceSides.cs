using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceSides : MonoBehaviour
{
    public List<DiceSide> sides;

    public DiceSide GetDiceSide(int index) => sides[index];

    public Quaternion GetWorldRotationFor(int index)
    {
        // 获得该面法向量的世界方向
        Vector3 worldNormalToMatch = transform.TransformDirection(GetDiceSide(index).normal);
        return Quaternion.FromToRotation(worldNormalToMatch, Vector3.up) * transform.rotation;
    }

    //用于计算当前骰子朝上的面所代表的点数
    public int GetMatch()
    {

        // 世界面朝向转骰子本地方向
        Vector3 loaclVectorToMatch = transform.InverseTransformDirection(Vector3.up);
        // 最匹配朝上的面
        DiceSide closestSide = null;
        // 利用点积计算匹配
        float closestDot = -1;

        foreach (DiceSide side in sides)
        {
            float dot = Vector3.Dot(side.normal, loaclVectorToMatch);
            if (closestSide == null || dot > closestDot)
            {
                closestSide = side;
                closestDot = dot;
            }
            // 精确匹配直接返回
            if (dot > GameplaySettings.exact_match_value)
            {
                return closestSide.val;
            }
        }
        // 非精确匹配返回最大面
        return closestSide?.val ?? -1;
    }
}
