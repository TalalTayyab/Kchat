using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kchat.UI.Framework
{
    public class KafkaOptions
    {
        public string Topic { get; set; }
        public string BootstrapServers { get; set; }
        public string Username { get; set;}
        public string Password { get; set; }
    }
}
