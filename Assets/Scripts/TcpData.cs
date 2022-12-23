using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TcpSendData
{
    public string id;
    public string sessionKey;
    public long time;
    public Message msg;
    public Player playerInfo;

    public string ToJson ()
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        time = Convert.ToInt64(ts.TotalSeconds);
        return Newtonsoft.Json.JsonConvert.SerializeObject(this);
    }
}

public class Player
{
    public string mail;
    public float positionX;
    public float positionY;
    public float moveDirX;
    public float moveDirY;
}
public class Message
{
    public string time;
    public string content;
}