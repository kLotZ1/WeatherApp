using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using WeatherApp.Exceptions;

namespace WeatherApp.Controllers;

public class AuthenticationController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AuthenticationController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [Authorize]
    public async Task Logout()
    {
        var idpClient = _httpClientFactory.CreateClient("IDPClient");

        var discoveryDocument = await idpClient.GetDiscoveryDocumentAsync();

        if (discoveryDocument == null) throw new ObjectIsNullException("Discovery document is null!");

        /*var accessTokenOperationResponse = await idpClient.RevokeTokenAsync(new TokenRevocationRequest()
        {
            Address = discoveryDocument.RevocationEndpoint,
            ClientId = "weatherclient",
            ClientSecret = "secret",
            Token = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken)
        });

        if (accessTokenOperationResponse.IsError)
        {
            
        }*/
        
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        
    }

    public IActionResult ActionDenied()
    {
        return View();
    }
}