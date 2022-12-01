using UnityEngine;
using UnityEngine.Networking;

public class Controls : NetworkBehaviour
{
    void Update()
    {
        if (!isLocalPlayer)
        {
            // 如果不是本地玩家，则退出更新
            return;
        }

        // 处理玩家的移动输入
    }
}