using UnityEngine;
using UnityEngine.Networking;

public class Controls : NetworkBehaviour
{
    void Update()
    {
        if (!isLocalPlayer)
        {
            // ������Ǳ�����ң����˳�����
            return;
        }

        // ������ҵ��ƶ�����
    }
}