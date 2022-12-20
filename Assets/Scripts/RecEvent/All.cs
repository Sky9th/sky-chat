using System.Collections.Generic;

namespace RecEvent
{
    public class All
    {
        public string type;
        public string msg;
        public Dictionary<string, Player> playerList { get; set; }
        public List<Message> msgList { get; set; }
    }
}