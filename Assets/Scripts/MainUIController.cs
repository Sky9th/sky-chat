using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public delegate void SendMsg(Message msg);

public class MainUIController : MonoBehaviour
{

    private UIDocument uIDocument;

    private bool isInputChat = false;
    private bool isSending = false;

    private TextField inputField;

    public event SendMsg sendMsg;

    // Start is called before the first frame update
    void Start()
    {
        uIDocument = GetComponent<UIDocument>();
        inputField = uIDocument.rootVisualElement.Query<TextField>("ChatInput").First();
        inputField.RegisterCallback<FocusEvent>(inputFocus);
        inputField.RegisterCallback<BlurEvent>(inputBlur);

        GameObject.Find("GameController").GetComponent<GameController>().messageSended += onMessageSended;
        GameObject.Find("NetworkController").GetComponent<NetworkController>().messageReceived += onMessageReceived;
    }

    private void onMessageReceived(TcpRecData tcpRecData)
    {
        Debug.Log("MainUI Receive msg");
        Debug.Log(tcpRecData);
        ScrollView scrollView = uIDocument.rootVisualElement.Query<ScrollView>("ChatList").First();
        VisualElement item = uIDocument.rootVisualElement.Query<VisualElement>("ChatListItem").First();

        VisualElement newItem = new VisualElement();
        newItem.style.height = new StyleLength(60);
        newItem.style.backgroundColor = Color.black;

        scrollView.Add(newItem);
    }

    private void onMessageSended()
    {
        Debug.Log("onMessageSended");
        isSending = false;
    }

    private void inputBlur(BlurEvent evt)
    {
        Debug.Log("blur");
        isInputChat = false;
    }

    private void inputFocus(FocusEvent evt)
    {
        Debug.Log("Focus");
        isInputChat = true;
    }

    public bool getIsInputChat ()
    {
        return isInputChat;
    }

    // Update is called once per frame
    void Update()
    {
        if (isInputChat && Input.GetKeyDown(KeyCode.Return) && !isSending)
        {
            String msgContent = inputField.value;
            inputField.value = "";

            Message msg = new Message
            {
                time = DateTime.UtcNow.ToString(),
                content = msgContent
            };
            isSending = true;
            sendMsg(msg);
            Debug.Log(msg);

        }
    }
}
