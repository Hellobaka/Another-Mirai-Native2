using Another_Mirai_Native.Config;

namespace Another_Mirai_Native.gRPC
{
    public class Server
    {
        public static Grpc.Core.Server ServerInstance { get; set; }

        public static Server Instance { get; set; }

        public Server()
        {
            Instance = this;
        }

        public bool Start()
        {
            try
            {
                ServerInstance = new()
                {
                    Ports = { new Grpc.Core.ServerPort(AppConfig.gRPCListenIP, AppConfig.gRPCListenPort, Grpc.Core.ServerCredentials.Insecure) }
                };
                ServerInstance.Services.Add(CQPFunctions.BindService(new CQPFunctionWrapper()));
                ServerInstance.Services.Add(CoreFunctions.BindService(new CoreFunctionWrapper()));
                ServerInstance.Start();
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public bool Stop()
        {
            try
            {
                ServerInstance?.ShutdownAsync().Wait();
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
