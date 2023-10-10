using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Another_Mirai_Native
{
    public class PluginManager
    {
        public static PluginManager Instance { get; private set; }

        public PluginManager()
        {
            Instance = this;
        }

        public bool LoadAndEnable()
        {
            return true;
        }

        public bool LoadAndEnable(string pluginPath)
        {
            return true;
        }
    }
}