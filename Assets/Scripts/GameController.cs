using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

public delegate void MessageSended();

public class GameController : MonoBehaviour
{
    public NetworkController networkController;
    public GameObject player;
    public UIDocument uIDocument;

    bool isWaitingMessage = true;
    Message waitingMessage = new();

    public event MessageSended messageSended;

    [DllImport("__Internal")]
    private static extern void jSLibWebSocketInit();
    [DllImport("__Internal")]
    private static extern void jSLibWebSocketSend(string msg);

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(PlayerPrefs.GetString(Store.SESSION_KEY));
        networkController = GameObject.Find("NetworkController").GetComponent<NetworkController>();
        GameObject.Find("UIDocument").GetComponent<MainUIController>().sendMsg += getWaitingMsg;
    }

    private void getWaitingMsg(Message msg)
    {
        Debug.Log("getWaitingMsg");
        waitingMessage = msg;
        isWaitingMessage = false;
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {
        if (!isWaitingMessage)
        {
            TcpSendData tcpSendData = new TcpSendData();
            tcpSendData.id = "testID";
            tcpSendData.sessionKey = PlayerPrefs.GetString(Store.SESSION_KEY);

            tcpSendData.msg = waitingMessage;
            if (!isWaitingMessage)
            {
                Console.WriteLine();
                Debug.Log(waitingMessage);

                isWaitingMessage = true;
                waitingMessage = new Message();
                messageSended();
            }

            Player playerInfo = new();
            playerInfo.mail = "testMail";
            playerInfo.positionX = player.transform.position.x.ToString();
            playerInfo.positionY = player.transform.position.y.ToString();

            tcpSendData.playerInfo = playerInfo;
            string sendStr = tcpSendData.ToJson();
            networkController.Send(sendStr);
        }
    }
}
