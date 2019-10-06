using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZermeloFunction.Models
{
    public class Announce
    {
        public List<Message> Messages { get; set; }
    }

    public class Message
    {
        public string TheMessage { get; set; }

        public string Teacher { get; set; }

        public string Classroom { get; set; }

        public bool cancelled { get; set; }
    }
}
