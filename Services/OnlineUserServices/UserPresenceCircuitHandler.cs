using System.Security.Claims;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.Components.Authorization; // Add this if needed

namespace Accounting_V4.Services.OnlineUserServices;

public class UserPresenceCircuitHandler : CircuitHandler
{
    private readonly OnlineUserService _onlineUsers;
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public UserPresenceCircuitHandler(OnlineUserService onlineUsers, AuthenticationStateProvider authenticationStateProvider)
    {
        _onlineUsers = onlineUsers;
        _authenticationStateProvider = authenticationStateProvider;
    }

    public override async Task OnConnectionUpAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var userId = authState.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!string.IsNullOrEmpty(userId))
        {
            _onlineUsers.UserConnected(userId);
        }
    }

    public override async Task OnConnectionDownAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var userId = authState.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!string.IsNullOrEmpty(userId))
        {
            _onlineUsers.UserDisconnected(userId);
        }
    }
}
