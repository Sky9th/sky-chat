using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TcpSendData
{
    public string id { get; set; }
    public string sessionKey { get; set; }
    public long time;
    public Message msg { get; set; }
    public Player playerInfo { get; set; }

    public string ToJson ()
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        time = Convert.ToInt64(ts.TotalSeconds);

        return Newtonsoft.Json.JsonConvert.SerializeObject(this);
    }

}

public class TcpRecData
{
    public string type;
    public string msg;
    public Dictionary<String, Player> playerList { get; set; }
    public List<Message> msgList { get; set; }
}

public class TcpRecDataPer
{
    public string type;
    public string id;
}

public class Player
{
    public string mail { get; set; }
    public string positionX { get; set; }
    public string positionY { get; set; }
}
public class Message
{
    public string time { get; set; }
    public string content { get; set; }
}

enum SendType
{
    ALL,
    PERSONAL
}