using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

namespace AzureB2CWebAppExample.Controllers;

public class AccountController : Controller
{
    public IActionResult SignIn()
    {
        return Challenge(
            new AuthenticationProperties()
            {
                RedirectUri = "/"
            },
            CookieAuthenticationDefaults.AuthenticationScheme, 
            OpenIdConnectDefaults.AuthenticationScheme);
    }
    
    public IActionResult SignOut()
    {
        return SignOut(
            CookieAuthenticationDefaults.AuthenticationScheme,
            OpenIdConnectDefaults.AuthenticationScheme);
    }

    // public IActionResult PostLogout()
    // {
    //     
    // }
}