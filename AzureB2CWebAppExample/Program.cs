using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddControllersWithViews();

builder.Services.AddRazorPages();

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services
    .AddAuthentication(opts =>
    {
        opts.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        opts.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        opts.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme,opts =>
    {
        var config = builder.Configuration.GetSection("AzureADB2C").Get<OpenIdConnectOptions>();
        var policyBasedAuthority = builder.Configuration.GetSection("AzureADB2CPolicyAuthority").Get<string>();
        
        opts.Authority = policyBasedAuthority;
        opts.ClientId = config.ClientId;
        opts.ClientSecret = config.ClientSecret;
        opts.ResponseType = OpenIdConnectResponseType.Code;
        opts.ResponseMode = OpenIdConnectResponseMode.FormPost;
        
        opts.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = "name",
            RoleClaimType = "role"
        };

        opts.Events.OnTokenResponseReceived += context =>
        {
            Console.WriteLine($"{Environment.NewLine}{context.TokenEndpointResponse.AccessToken}");
            return Task.CompletedTask;
        };
    });

builder.Services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    options.Scope.Add("offline_access");
    builder.Configuration
        .GetSection("AzureADB2CScopes")
        .Get<List<string>>()
        ?.ForEach(x =>  options.Scope.Add(x));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    IdentityModelEventSource.ShowPII = true;
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();