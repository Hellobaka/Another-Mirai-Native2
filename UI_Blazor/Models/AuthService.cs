using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;
using Another_Mirai_Native.BlazorUI.Models;
using Microsoft.AspNetCore.Components.Server.Circuits;
using System.Windows.Forms.Design;
using System.Collections.Concurrent;

namespace Another_Mirai_Native.BlazorUI
{
    public class AuthService : AuthenticationStateProvider, IDisposable
    {
        private static readonly AuthenticationState UnauthorizedAuthenticationState = new(new ClaimsPrincipal());
        private ClaimsPrincipal? _principal;
        private readonly ProtectedSessionStorage _protectedSessionStorage;

        public static event Action<string> OnAuthChanged;

        public string SessionId { get; set; } = Guid.NewGuid().ToString();

        public AuthService(ProtectedSessionStorage protectedSessionStorage, Shared shared)
        {
            _protectedSessionStorage = protectedSessionStorage;
            OnAuthChanged += AuthService_OnAuthChanged;
        }

        public void Dispose()
        {
            OnAuthChanged -= AuthService_OnAuthChanged;
        }

        private async void AuthService_OnAuthChanged(string sessionId)
        {
            if (_principal == null || SessionId == sessionId)
            {
                return;
            }
            await UpdateSignInStatusAsync(null);
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if (_principal is null) return Task.FromResult(UnauthorizedAuthenticationState);
            return Task.FromResult(new AuthenticationState(_principal));
        }

        public async Task LoadAuthenticationStateAsync()
        {
            var result = await _protectedSessionStorage.GetAsync<string>("authkey");
            if (result.Success)
            {
                var name = result.Value!;
                await UpdateSignInStatusAsync(new ClaimsPrincipal(
                    new ClaimsIdentity(
                        [
                            new(ClaimTypes.Name, name)
                        ],
                        "Blazor"
                    )
                ));
            }
        }

        public async Task UpdateSignInStatusAsync(ClaimsPrincipal? principal)
        {
            _principal = principal;
            try
            {
                if (_principal?.Identity?.IsAuthenticated ?? false)
                {
                    await _protectedSessionStorage.SetAsync("authkey", _principal.Identity.Name!);
                }
                else
                {
                    await _protectedSessionStorage.DeleteAsync("authkey");
                }
            }
            catch
            {
            }
            if (principal != null)
            {
                OnAuthChanged?.Invoke(SessionId);
            }
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public async Task<bool> AuthenticateUser(string password)
        {
            if (password != Blazor_Config.Instance.Password)
            {
                await UpdateSignInStatusAsync(null);
                return false;
            }

            var identity = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.Name, "AuthedUser"),
            ], "Blazor");

            var user = new ClaimsPrincipal(identity);
            await UpdateSignInStatusAsync(user);
            return true;
        }
    }

    public class AuthCircuitHandler : CircuitHandler
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ConcurrentDictionary<Circuit, IServiceScope> _scopes = new ConcurrentDictionary<Circuit, IServiceScope>();

        public AuthCircuitHandler(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public override Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            var scope = _serviceScopeFactory.CreateScope();
            _scopes.TryAdd(circuit, scope);
            return Task.CompletedTask;
        }

        public override async Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            if (_scopes.TryRemove(circuit, out var scope))
            {
                if (scope != null)
                {
                    var myService = scope.ServiceProvider.GetService<AuthService>();
                    if (myService is IDisposable disposableService)
                    {
                        disposableService.Dispose();
                    }
                    scope.Dispose();
                }
            }

            await base.OnCircuitClosedAsync(circuit, cancellationToken);
        }
    }

}
