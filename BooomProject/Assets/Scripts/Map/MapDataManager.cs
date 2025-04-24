using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDataManager : Singleton<MapDataManager>
{
    public NodeAddressSO nodeAddressSO;
    // �ڵ��� ��ַ�ֵ��
    public Dictionary<string, int> nodeAddress;
   
    protected override void init()
    {
        nodeAddress = nodeAddressSO.nodeAddress.ToDictionary();
       
    }
  
}
