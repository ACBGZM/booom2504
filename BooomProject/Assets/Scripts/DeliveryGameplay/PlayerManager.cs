using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 维护骑手信息
public class PlayerManager : Singleton<PlayerManager>
{
    // 骑手速度
    public int speed;
    // 骑手声誉
    public int fame;
    // 骑手好评率
    public int feedback;

    protected override void init()
    {
        speed = 1;
    }

    public void UpdateSpeed(int val)
    {
        speed += val;
        // 速度更新时，更新订单时间和距离
        //   EventHandlerManager.CallUpdateArriveDistAndTime(EventHandlerManager.CallGetCurrentNode(), speed);
    }
}
