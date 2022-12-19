using Cinemachine;
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

    public UIDocument uIDocument;
    public GameObject spwanPoint;
    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private GameObject cinemaCamera;

    private NetworkController networkController;
    private GameObject player;

    bool isWaitingMessage = true;
    Message waitingMessage = new();

    public event MessageSended messageSended;
    private Dictionary<String, GameObject> onlinePlayerList;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(PlayerPrefs.GetString(Store.SESSION_KEY));
        networkController = GameObject.Find("NetworkController").GetComponent<NetworkController>();
        GameObject.Find("UIDocument").GetComponent<MainUIController>().sendMsg += getWaitingMsg;
        GameObject.Find("NetworkController").GetComponent<NetworkController>().boardcastReceived += onBoardcastReceived;
        GameObject.Find("NetworkController").GetComponent<NetworkController>().messageReceived += onMessageReceived;
        onlinePlayerList = new Dictionary<string, GameObject>();
    }

    private void onMessageReceived(TcpRecDataPer tcpRecData)
    {
        PlayerPrefs.SetString(Store.NETWORK_IDENTIFY, tcpRecData.id);
        player = Instantiate(playerPrefab, spwanPoint.transform.position, Quaternion.identity);
        player.GetComponent<NetworkPlayer>().networkIdentify = tcpRecData.id;
        cinemaCamera.GetComponent<CinemachineVirtualCamera>().m_Follow = player.transform;
    }

    private void onBoardcastReceived(TcpRecData tcpRecData)
    {
        Dictionary<String, Player> playerList = tcpRecData.playerList;
        foreach (KeyValuePair<String, Player> p in playerList) {
            if (p.Key == PlayerPrefs.GetString(Store.NETWORK_IDENTIFY)) continue;
            if (onlinePlayerList.ContainsKey(p.Key))
            {
                onlinePlayerList[p.Key].transform.position = new Vector2(float.Parse(p.Value.positionX), float.Parse(p.Value.positionY));
            } else
            {
                GameObject insertPlayer = Instantiate(playerPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
                NetworkPlayer networkPlayer = insertPlayer.GetComponent<NetworkPlayer>();
                networkPlayer.networkIdentify = p.Key;
                onlinePlayerList.Add(p.Key, insertPlayer);
            }
        }
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
        if (player)
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

            if (player)
            {
                Player playerInfo = new();
                playerInfo.mail = "testMail";
                playerInfo.positionX = player.transform.position.x.ToString();
                playerInfo.positionY = player.transform.position.y.ToString();
                tcpSendData.playerInfo = playerInfo;
            }

            string sendStr = tcpSendData.ToJson();
            networkController.Send(sendStr);
        }
    }
}
