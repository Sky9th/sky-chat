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
        Debug.Log(isInputChat);
        if (isInputChat && Input.GetKeyDown(KeyCode.Return) && !isSending)
        {
            String msgContent = inputField.value;
            inputField.value = "";

            Message msg = new Message
            {
                Time = DateTime.UtcNow.ToString(),
                Content = msgContent
            };
            isSending = true;
            sendMsg(msg);
            Debug.Log(msg);

        }
    }
}
