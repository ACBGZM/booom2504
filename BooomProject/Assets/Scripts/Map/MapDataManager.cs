using System.Collections.Generic;

public class MapDataManager : Singleton<MapDataManager>
{
    // TODO: 待填
    public NodeAddressSO nodeAddressSO;

    // 节点编号 地址字典表
    public Dictionary<string, int> nodeAddress;

    protected override void init()
    {
        nodeAddress = nodeAddressSO.nodeAddress.ToDictionary();
    }
}
