using UnityEngine;

public delegate void MessageReceived(TcpRecData tcpRecData);

public class NetworkController : MonoBehaviour
{
    private TcpClient tcpClient = new TcpClient();
    private WsClient wsClient = new WsClient();

    private RuntimePlatform platform = Application.platform;

    private string wsServerUrl = "ws://127.0.0.1";
    private int wsServerPort = 6667;
    private string tcpServerUrl = "127.0.0.1";
    private int tcpServerPort = 6666;

    public MessageReceived messageReceived;

    private bool isConnected = false;

    // Start is called before the first frame update
    void Start()
    {
        switch (platform)
        {
            case RuntimePlatform.WebGLPlayer:
                wsClient.init(wsServerUrl + ':' + wsServerPort);
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
            Debug.Log("receive£º" + msg);
            TcpRecData tcpRecData = Newtonsoft.Json.JsonConvert.DeserializeObject<TcpRecData>(msg);
            messageReceived(tcpRecData);
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
