using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TcpSendData
{
    public string ID { get; set; }
    public string SessionKey { get; set; }
    public long Time;
    public Message Msg { get; set; }
    public Player PlayerInfo { get; set; }

    public string ToJson ()
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        Time = Convert.ToInt64(ts.TotalSeconds);

        return Newtonsoft.Json.JsonConvert.SerializeObject(this);
    }

}

public class TcpRecData
{
    public List<Player> PlayerList { get; set; }
    public List<Message> MsgList { get; set; }
}

public class Player
{
    public string Mail { get; set; }
    public string PositionX { get; set; }
    public string PositionY { get; set; }
}
public class Message
{
    public string Time { get; set; }
    public string Content { get; set; }
}