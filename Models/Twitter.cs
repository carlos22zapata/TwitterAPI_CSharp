using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api_Tweet1.Models
{
    public class Twitter
    {
    }

    public class userTw
    {
        public string user { get; set; }
        public int followers { get; set; }
    }

    public class followers
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class messages
    {
        public string id { get; set; }
        public string text { get; set; }
    }
}
