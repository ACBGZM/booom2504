using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ά��������Ϣ
public class PlayerManager : Singleton<PlayerManager>
{
    // �����ٶ�
    public int speed;
    // ��������
    public int fame;
    // ���ֺ�����
    public int feedback;

    protected override void init()
    {
        speed = 1;
    }

    public void UpdateSpeed(int val)
    {
        speed += val;
        // �ٶȸ���ʱ�����¶���ʱ��;���
        //   EventHandlerManager.CallUpdateArriveDistAndTime(EventHandlerManager.CallGetCurrentNode(), speed);
    }
}
