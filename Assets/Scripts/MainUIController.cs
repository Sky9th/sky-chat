using RecEvent;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public delegate void SendMsg(Message msg);

public class MainUIController : MonoBehaviour
{

    private UIDocument uIDocument;

    private bool isInputChat = false;
    private bool isSending = false;

    private TextField inputField;
    private Button inputBtn;
    private ScrollView scrollView;
    private bool isScrollBottom = true;
    private bool isScrollUp = false;

    public event SendMsg sendMsg;

    // Start is called before the first frame update
    void Start()
    {
        uIDocument = GetComponent<UIDocument>();
        inputField = uIDocument.rootVisualElement.Query<TextField>("ChatInput").First();
        inputBtn = uIDocument.rootVisualElement.Query<Button>("ChatBtn").First();
        inputField.RegisterCallback<FocusEvent>(inputFocus);
        inputField.RegisterCallback<BlurEvent>(inputBlur);

        GameObject.Find("GameController").GetComponent<GameController>().messageSended += onMessageSended;
        GameObject.Find("NetworkController").GetComponent<NetworkController>().AllReceived += onAllReceived;
        scrollView = uIDocument.rootVisualElement.Query<ScrollView>("ChatList").First();

        scrollView.RegisterCallback<WheelEvent>(wheelScroll);
        scrollView.verticalScroller.valueChanged += scrollCheck;
    }

    private void wheelScroll(WheelEvent evt)
    {
        if (!isScrollBottom)
        {
            isScrollUp = true;
        } else {
            isScrollUp = false;
        }

    }

    private void scrollCheck(float obj)
    {
        if (scrollView.verticalScroller.highValue == obj)
        {
            isScrollBottom = true;
        } else
        {
            isScrollBottom = false;
        }
    }

    private void onAllReceived(All data)
    {
        for (int i = 0; i < data.msgList.Count; i++)
        {
            Message message = data.msgList[i];

            VisualElement newItem = new VisualElement();
            newItem.name = "ChatListItem";
            newItem.style.flexDirection = FlexDirection.Row;
            newItem.style.justifyContent = Justify.SpaceBetween;
            newItem.style.flexWrap = Wrap.Wrap;
            newItem.style.borderBottomWidth = 1;
            newItem.style.borderBottomColor = new Color(1f, 1f, 1f, 0.5f);
            newItem.style.paddingTop = 10;
            newItem.style.paddingBottom = 10;

            VisualElement content = new VisualElement();
            Label contentLabel = new Label();
            contentLabel.name = "ChatContent";
            contentLabel.text = message.content;
            contentLabel.style.fontSize = Length.Percent(12);
            contentLabel.style.color = new Color(1f, 1f, 1f, 1f);
            content.Add(contentLabel);
            content.style.width = Length.Percent(74);
            setPadding(contentLabel, 0);
            setPadding(content, 0);
            setMargin(content, 0);
            newItem.Add(content);

            VisualElement time = new VisualElement();
            Label timeLabel = new Label();
            timeLabel.name = "ChatTime";
            timeLabel.text = message.time;
            timeLabel.style.fontSize = Length.Percent(8);
            timeLabel.style.color = new Color(0.8f, 0.8f, 0.8f, 1f);
            time.Add(timeLabel);
            time.style.width = Length.Percent(25);
            time.style.unityTextAlign = TextAnchor.MiddleRight;
            setPadding(timeLabel, 0);
            setPadding(time, 0);
            setMargin(time, 0);
            newItem.Add(time);

            VisualElement name = new VisualElement();
            Label nameLabel = new Label();
            nameLabel.name = "ChatName";
            nameLabel.text = "@test";
            nameLabel.style.color = new Color(0.8f, 0.8f, 0.8f, 1f);
            nameLabel.style.fontSize = Length.Percent(9);
            nameLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            name.Add(nameLabel);
            name.style.width = Length.Percent(100);
            setPadding(nameLabel, 0);
            setPadding(name, 0);
            setMargin(name, 0);
            newItem.Add(name);

            scrollView.Add(newItem);
        }
        scrollToBottom(scrollView);
    }

    private async void scrollToBottom(ScrollView scrollView)
    {
        await Task.Delay(100);
        if (scrollView.verticalScroller.highValue > 0 && !isScrollUp)
        {
            scrollView.scrollOffset = new Vector2(0, scrollView.verticalScroller.highValue);
        }
    }


    private void setPadding(in VisualElement el, int p)
    {
        el.style.paddingBottom = p;
        el.style.paddingTop = p;
        el.style.paddingLeft = p;
        el.style.paddingRight = p;
    }

    private void setMargin(in VisualElement el, int p)
    {
        el.style.marginBottom = p;
        el.style.marginTop = p;
        el.style.marginLeft = p;
        el.style.marginRight = p;
    }

    private void onMessageSended()
    {
        isSending = false;
    }

    private void inputBlur(BlurEvent evt)
    {
        isInputChat = false;
    }

    private void inputFocus(FocusEvent evt)
    {
        isInputChat = true;
    }

    public bool getIsInputChat ()
    {
        return isInputChat;
    }

    public IEnumerator Focus()
    {
        inputField.focusable = true;
        inputBtn.focusable = true;
        yield return new WaitForSeconds(0.3f);
        inputField.Focus();
        isInputChat = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isInputChat && Input.GetKeyDown(KeyCode.Return) && !isSending)
        {
            string msgContent = inputField.value;
            inputField.value = "";

            Message msg = new Message
            {
                time = DateTime.UtcNow.ToString(),
                content = msgContent
            };
            isSending = true;
            sendMsg(msg);
            inputField.focusable = false;
            inputBtn.focusable = false;

        } else if (Input.GetKeyDown(KeyCode.Return))
        {
            StartCoroutine(Focus());
        }
    }
}
