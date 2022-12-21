using RecEvent;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class NetworkController : MonoBehaviour
{
    private TcpClient tcpClient = new TcpClient();
    private WsClient wsClient = new WsClient();

    private RuntimePlatform platform = Application.platform;

    private string wsServerUrl = "ws://127.0.0.1";
    private int wsServerPort = 6667;
    private string tcpServerUrl = "127.0.0.1";
    private int tcpServerPort = 6666;

    public Action<All> AllReceived;
    public Action<Active> ActiveReceived;
    public Action<InActive> InActiveReceived;
    public Action<Msg> MsgReceived;

    public int receivePackCount = 0;
    public int sendPackCount = 0;
    public long lastReceiveTime = ((DateTime.UtcNow.ToUniversalTime().Ticks - 621355968000000000) / 10000);
    public float receiveSpendTime = 0;

    private bool isConnected = false;

    /*[DllImport("__Internal")]
    private static extern void jSLibWebSocketInit(string url, int port);
    [DllImport("__Internal")]
    private static extern void jSLibWebSocketSend(string msg);
    [DllImport("__Internal")]
    private static extern void jSLibWebSocketClose();*/

    private ConcurrentQueue<string> recDatas;

    // Start is called before the first frame update
    void Start()
    {

        switch (platform)
        {
            case RuntimePlatform.WebGLPlayer:
                //jSLibWebSocketInit(wsServerUrl, wsServerPort);
                //wsClient.init(wsServerUrl + ':' + wsServerPort);
                break;
            default:
                tcpClient.InitSocket(tcpServerUrl, tcpServerPort);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        string msg = "";
        switch (platform)
        {
            case RuntimePlatform.WebGLPlayer:
                if (recDatas.Count > 0)
                {
                    recDatas.TryDequeue(out msg);
                }
                break;
            default:
                if (tcpClient.recDatas.Count > 0)
                {
                    tcpClient.recDatas.TryDequeue(out msg);
                }
                break;
        }
        if (msg.Length > 0)
        {
            int typeNum = int.Parse(msg.Substring(0, 2));
            string data = msg.Substring(2);
            switch (typeNum)
            {
                case (int)RecType.ALL:
                    All rec = Newtonsoft.Json.JsonConvert.DeserializeObject<All>(data);
                    AllReceived(rec);
                    break;
                case (int)RecType.ACTIVE:
                    ActiveReceived(Newtonsoft.Json.JsonConvert.DeserializeObject<Active>(data));
                    break;
                case (int)RecType.INACTIVE:
                    InActiveReceived(Newtonsoft.Json.JsonConvert.DeserializeObject<InActive>(data));
                    break;
                case (int)RecType.MSG:
                    Msg msssg = Newtonsoft.Json.JsonConvert.DeserializeObject<Msg>(data);
                    MsgReceived(msssg);
                    break;
                default:
                    throw new Exception("error msg type");
            }
            receivePackCount++;
            receiveSpendTime = ((DateTime.UtcNow.ToUniversalTime().Ticks - 621355968000000000) / 10000) - lastReceiveTime;
            lastReceiveTime = ((DateTime.UtcNow.ToUniversalTime().Ticks - 621355968000000000) / 10000);
        }
    }

    public void Send (string msg)
    {
        switch (platform)
        {
            case RuntimePlatform.WebGLPlayer:
                //wsClient.Send(msg);
                //jSLibWebSocketSend(msg);
                break;
            default:
                tcpClient.SocketSend(msg);
                break;
        }
        sendPackCount++;
    }

    public void Disconnect ()
    {
        switch (platform)
        {
            case RuntimePlatform.WebGLPlayer:
                //jSLibWebSocketClose();
                break;
            default:
                tcpClient.SocketQuit();
                break;
        }
    }

    public bool CheckConnected ()
    {
        switch (platform)
        {
            case RuntimePlatform.WebGLPlayer:
                return isConnected;
            default:
                isConnected = tcpClient.isConnected;
                return tcpClient.isConnected;
        }
    }

    public void wsConnectedCallback ()
    {
        isConnected = true;
    }

    public void wsReceiveCallback (string data)
    {
        recDatas.Enqueue(data);
    }
}
