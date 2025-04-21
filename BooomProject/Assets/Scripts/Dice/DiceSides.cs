using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceSides : MonoBehaviour
{
    public List<DiceSide> sides;

    public DiceSide GetDiceSide(int index) => sides[index];

    public Quaternion GetWorldRotationFor(int index)
    {
        // ��ø��淨���������緽��
        Vector3 worldNormalToMatch = transform.TransformDirection(GetDiceSide(index).normal);
        return Quaternion.FromToRotation(worldNormalToMatch, Vector3.up) * transform.rotation;
    }

    //���ڼ��㵱ǰ���ӳ��ϵ���������ĵ���
    public int GetMatch()
    {

        // �����泯��ת���ӱ��ط���
        Vector3 loaclVectorToMatch = transform.InverseTransformDirection(Vector3.up);
        // ��ƥ�䳯�ϵ���
        DiceSide closestSide = null;
        // ���õ������ƥ��
        float closestDot = -1;

        foreach (DiceSide side in sides)
        {
            float dot = Vector3.Dot(side.normal, loaclVectorToMatch);
            if (closestSide == null || dot > closestDot)
            {
                closestSide = side;
                closestDot = dot;
            }
            // ��ȷƥ��ֱ�ӷ���
            if (dot > GameplaySettings.exact_match_value)
            {
                return closestSide.val;
            }
        }
        // �Ǿ�ȷƥ�䷵�������
        return closestSide?.val ?? -1;
    }
}
