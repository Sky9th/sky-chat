using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

public delegate void MessageSended();

public class GameController : MonoBehaviour
{

    public GameObject tcpObj;
    public GameObject player;
    public UIDocument uIDocument;

    TcpClient tcp;
    bool stop = false;

    bool isWaitingMessage = true;
    Message waitingMessage = new();

    public event MessageSended messageSended;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(PlayerPrefs.GetString(Store.SESSION_KEY));
        tcp = tcpObj.GetComponent<TcpClient>();
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
        if ((tcp.isConnected && !stop) || (tcp.isConnected && !isWaitingMessage))
        {
            TcpSendData tcpSendData = new TcpSendData();
            tcpSendData.ID = "testID";
            tcpSendData.SessionKey = PlayerPrefs.GetString(Store.SESSION_KEY);

            tcpSendData.Msg = waitingMessage;
            if (!isWaitingMessage)
            {
                Debug.Log(waitingMessage);

                isWaitingMessage = true;
                waitingMessage = new Message();
                messageSended();
                Debug.Log(11111111);
            }

            Player playerInfo = new();
            playerInfo.Mail = "testMail";
            playerInfo.PositionX = player.transform.position.x.ToString();
            playerInfo.PositionY = player.transform.position.y.ToString();

            tcpSendData.PlayerInfo = playerInfo;

            tcp.SocketSend(tcpSendData.ToJson());

            stop = true;
        }
    }
}
