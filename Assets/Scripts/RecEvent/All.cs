using System.Collections.Generic;

namespace RecEvent
{
    public class All
    {
        public string type;
        public string msg;
        public Dictionary<string, Player> playerList { get; set; }
        public Dictionary<string, List<Message>> msgList { get; set; }
    }
}