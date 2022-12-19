using System.Runtime.InteropServices;
using UnityEngine;

public delegate void BoardcastReceived(TcpRecData tcpRecData);
public delegate void MessageReceived(TcpRecDataPer tcpRecData);

public class NetworkController : MonoBehaviour
{
    private TcpClient tcpClient = new TcpClient();
    private WsClient wsClient = new WsClient();

    private RuntimePlatform platform = Application.platform;

    private string wsServerUrl = "ws://127.0.0.1";
    private int wsServerPort = 6667;
    private string tcpServerUrl = "127.0.0.1";
    private int tcpServerPort = 6666;

    public BoardcastReceived boardcastReceived;
    public MessageReceived messageReceived;

    private bool isConnected = false;

    [DllImport("__Internal")]
    private static extern void jSLibWebSocketInit();
    [DllImport("__Internal")]
    private static extern void jSLibWebSocketSend(string msg);

    // Start is called before the first frame update
    void Start()
    {
        switch (platform)
        {
            case RuntimePlatform.WebGLPlayer:
                jSLibWebSocketInit();
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
                if (wsClient.receiveQueue.Count > 0) { 
                    wsClient.receiveQueue.TryDequeue(out msg);
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
            Debug.Log("receive type£º" + typeNum);
            Debug.Log("receive data£º" + data);
            if (typeNum == (int)SendType.ALL)
            {
                TcpRecData tcpRecData = Newtonsoft.Json.JsonConvert.DeserializeObject<TcpRecData>(data);
                boardcastReceived(tcpRecData);
            } else if (typeNum == (int)SendType.PERSONAL)
            {
                TcpRecDataPer tcpRecData = Newtonsoft.Json.JsonConvert.DeserializeObject<TcpRecDataPer>(data);
                Debug.Log(tcpRecData.ToString()) ;
                messageReceived(tcpRecData);
            } else
            {
                Debug.LogError("error msg type");
            }
        }
    }

    public void Send (string msg)
    {
        Debug.Log("Send:" + msg);
        switch (platform)
        {
            case RuntimePlatform.WebGLPlayer:
                Debug.Log("Platform:WebGLPlayer");
                wsClient.Send(msg);
                break;
            default:
                Debug.Log("Platform:WindowsPlayer");
                tcpClient.SocketSend(msg);
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
}
