using System.Net.Http.Headers;
using System.Security.Claims;
using Accounting_V4.Services.TokenServices;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Accounting_V4.Services.AuthenServices;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient httpClient;
    private readonly ILocalStorageService localStorage;
    private readonly AuthenticationState _anonymous;


    public CustomAuthStateProvider(HttpClient httpClient, ILocalStorageService localStorage)
    {
        this.httpClient = httpClient;
        this.localStorage = localStorage;
        _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }

    public ILocalStorageService LocalStorageService
    {
        get;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await localStorage.GetItemAsStringAsync("tokenStorage");

        if (string.IsNullOrWhiteSpace(token))
        {
            return _anonymous;
        }

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);

        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(JwtParser.ParseClaimFromJwt(token), "jwtAuthType")));

    }
    public void NotifyUserAuthentication(string token)
    {
        var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(JwtParser.ParseClaimFromJwt(token), "jwtAuthType"));
        var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
        NotifyAuthenticationStateChanged(authState);
    }


    public void NotifyUserLogOut()
    {
        var authState = Task.FromResult(_anonymous);
        NotifyAuthenticationStateChanged(authState);
    }
}



