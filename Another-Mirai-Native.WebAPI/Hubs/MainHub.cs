using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Another_Mirai_Native.WebAPI.Hubs
{
    [Authorize]
    public class MainHub : Hub
    {
    }
}
