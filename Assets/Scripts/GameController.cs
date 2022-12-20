using Cinemachine;
using RecEvent;
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

    private NetworkController networkController;
    private GameObject player;

    bool isWaitingMessage = true;
    Message waitingMessage = new();

    public event MessageSended messageSended;
    public Dictionary<String, GameObject> onlinePlayerList;

    // Start is called before the first frame update
    void Start()
    {
        networkController = GameObject.Find("NetworkController").GetComponent<NetworkController>();
        GameObject.Find("UIDocument").GetComponent<MainUIController>().sendMsg += getWaitingMsg;
        GameObject.Find("NetworkController").GetComponent<NetworkController>().ActiveReceived += onActiveReceived;
        GameObject.Find("NetworkController").GetComponent<NetworkController>().InActiveReceived += onInActiveReceived;
        GameObject.Find("NetworkController").GetComponent<NetworkController>().AllReceived += onAllReceived;
        GameObject.Find("NetworkController").GetComponent<NetworkController>().MsgReceived += onMsgReceived;
        onlinePlayerList = new Dictionary<string, GameObject>();
        StartCoroutine(sendTcpData());
    }

    private void onMsgReceived(Msg obj)
    {
    }

    private void onActiveReceived(Active data)
    {
        PlayerPrefs.SetString(Store.NETWORK_IDENTIFY, data.id);
        player = Instantiate(playerPrefab, spwanPoint.transform.position, Quaternion.identity);
        player.GetComponent<NetworkPlayer>().networkIdentify = data.id;
    }

    private void onInActiveReceived(InActive data)
    {
        foreach (KeyValuePair<String, GameObject> p in onlinePlayerList)
        {
            if (p.Key == data.id)
            {
                Destroy(p.Value);
                return;
            }
        }
    }

    private void onAllReceived(All data)
    {
        Dictionary<String, Player> playerList = data.playerList;
        foreach (KeyValuePair<string, Player> p in playerList)
        {
            if (p.Key == PlayerPrefs.GetString(Store.NETWORK_IDENTIFY)) continue;
            if (!onlinePlayerList.ContainsKey(p.Key))
            {
                GameObject insertPlayer = Instantiate(playerPrefab, spwanPoint.transform.position, Quaternion.identity);
                NetworkPlayer networkPlayer = insertPlayer.GetComponent<NetworkPlayer>();
                networkPlayer.networkIdentify = p.Key;
                onlinePlayerList.Add(p.Key, insertPlayer);
            }
        }
    }

    private void getWaitingMsg(Message msg)
    {
        waitingMessage = msg;
        isWaitingMessage = false;
    }

    private IEnumerator sendTcpData ()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(0.05f);
            if (player)
            {
                TcpSendData tcpSendData = new TcpSendData();
                tcpSendData.id = "testID";
                tcpSendData.sessionKey = PlayerPrefs.GetString(Store.SESSION_KEY);

                tcpSendData.msg = waitingMessage;
                if (!isWaitingMessage)
                {
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

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {
    }

    private void OnApplicationQuit()
    {
        networkController.Disconnect();
    }
}
