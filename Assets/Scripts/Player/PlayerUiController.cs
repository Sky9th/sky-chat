using RecEvent;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerUiController : MonoBehaviour
{
    private UIDocument uIDocument;
    private VisualElement messagePopContainer;
    private VisualElement messagePop;
    private float playerHeight;
    private Message msg;
    private Coroutine hideMessgePopCoroutine;
    private string networkIdentify;

    [SerializeField]
    private VisualTreeAsset visualTreeAsset;

    // Start is called before the first frame update
    void Start()
    {
        networkIdentify = GetComponent<NetworkPlayer>().networkIdentify;

        //VisualTreeAsset visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/Component/PlayerChatmessagePopContainer.uxml");
        uIDocument = GameObject.Find("UIDocument").GetComponent<UIDocument>();
        messagePopContainer = visualTreeAsset.Instantiate();
        messagePopContainer.style.position = Position.Absolute;
        messagePopContainer.style.left = 0;
        messagePopContainer.style.top = 0;
        messagePopContainer.style.visibility = Visibility.Hidden;


        playerHeight = GetComponent<SpriteRenderer>().sprite.rect.size.y * transform.localScale.y;
        uIDocument.rootVisualElement.Add(messagePopContainer);
        messagePop = messagePopContainer.Q<VisualElement>("PlayerMessagePop");
        messagePop.style.backgroundColor = new Color(0, 0, 0, .5f);

        messagePop.RegisterCallback<MouseOverEvent>(onMouseEnterEvent);
        messagePop.RegisterCallback<MouseOutEvent>(onMouseOutEvent);

        GameObject.Find("NetworkController").GetComponent<NetworkController>().AllReceived += onAllReceived;
    }

    private void onMouseOutEvent(MouseOutEvent evt)
    {
        hideMessgePopCoroutine = StartCoroutine(hideMessagePop());
    }

    private void onMouseEnterEvent(MouseOverEvent evt)
    {
        StopCoroutine(hideMessgePopCoroutine);
    }

    private void onAllReceived(All obj)
    {
        if (obj.msgList.ContainsKey(networkIdentify))
        {
            List<Message> inMsg;
            obj.msgList.TryGetValue(networkIdentify, out inMsg);
            StartCoroutine(setMessage(inMsg));
        }
    }

    private IEnumerator setMessage(List<Message> msgList)
    {
        do
        {
            msg = msgList[0];
            msgList.RemoveAt(0);
            yield return new WaitForSecondsRealtime(0.5f);
        } while (msgList.Count > 0);
    }

    private IEnumerator hideMessagePop ()
    {
        Debug.Log("hideMessagePop start" + networkIdentify);
        yield return new WaitForSecondsRealtime(3);
        messagePopContainer.style.visibility = Visibility.Hidden;
        Debug.Log("hideMessagePop end" + networkIdentify);
    }

    void OnGUI()
    {
        Vector3 namePos = Camera.main.WorldToScreenPoint(transform.position);
        float msgPopWidth = messagePop.resolvedStyle.width;
        float msgPopHeight = messagePop.resolvedStyle.height;
        messagePop.style.left = namePos.x - msgPopWidth / 2;
        messagePop.style.top = (Screen.height - namePos.y - playerHeight - msgPopHeight - 50f);
        if (msg != null)
        {
            if (hideMessgePopCoroutine != null)  StopCoroutine(hideMessgePopCoroutine);
            messagePopContainer.style.visibility = Visibility.Visible;
            messagePop.Q<Label>().text = msg.content;
            msg = null;
            hideMessgePopCoroutine = StartCoroutine(hideMessagePop());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
