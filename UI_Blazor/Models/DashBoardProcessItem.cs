namespace Another_Mirai_Native.BlazorUI.Models
{
    public class DashBoardProcessItem
    {
        public int PID { get; set; }

        public string PluginName { get; set; } = "";

        public bool Running { get; set; }

        public double CPUUsage { get; set; }
       
        public double MemoryUsage { get; set; }
    }
}
