namespace Another_Mirai_Native.WebAPI.Models
{
    public class DashboardPluginItem
    {
        public int Id { get; set; }

        public int PID { get; set; }

        public string PluginName { get; set; } = "";

        public bool Running { get; set; }

        public double CPUUsage { get; set; }

        public double MemoryUsage { get; set; }
    }
}
