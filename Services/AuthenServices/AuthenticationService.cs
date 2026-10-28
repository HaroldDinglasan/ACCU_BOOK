using System.Net.Http.Headers;
using System.Text.Json;
using Accounting_V4.Controllers.Token;
using Accounting_V4.Model.UserModels;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace Accounting_V4.Services.AuthenServices;

public class AuthenticationService : IAuthenticationService
{
    private readonly HttpClient _client;
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly ILocalStorageService _localStorage;
    private JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };
    private Token _token;
    public AuthenticationService(Token token, HttpClient client,
        AuthenticationStateProvider authenticationStateProvider,
        ILocalStorageService localStorage)
    {
        _token = token;
        _client = client;
        _authenticationStateProvider = authenticationStateProvider;
        _localStorage = localStorage;
    }

    public async Task<AuthenticatedUserModel> Login(UserModel userForAuthentication)
    {
        var authResult = await _token.PutToken(userForAuthentication);

        if (authResult != null && authResult.Success)
        {
            var result = new AuthenticatedUserModel
            {
                Access_Token = authResult.Access_Token,
                UserName = authResult.UserName
            };

            await _localStorage.SetItemAsync("tokenStorage", authResult.Access_Token);

            ((CustomAuthStateProvider)_authenticationStateProvider)
                .NotifyUserAuthentication(authResult.Access_Token);

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", authResult.Access_Token);

            return result;
        }

        // failed login
        return new AuthenticatedUserModel();
    }

    public async Task LogOut()
    {
        await _localStorage.RemoveItemAsync("tokenStorage");
        ((CustomAuthStateProvider)_authenticationStateProvider).NotifyUserLogOut();
        _client.DefaultRequestHeaders.Authorization = null;
    }
}
