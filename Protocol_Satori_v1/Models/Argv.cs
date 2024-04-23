using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Another_Mirai_Native.Protocol.Satori.Models
{
    public class Argv
    {
        public string name { get; set; }
        public object[] arguments { get; set; }
        public object options { get; set; }
    }
}
