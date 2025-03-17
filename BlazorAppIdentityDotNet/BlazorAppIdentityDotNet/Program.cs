using Age.Integrations.MPass.Saml;
using BlazorAppIdentityDotNet.Client.Pages;
using BlazorAppIdentityDotNet.Components;
using BlazorAppIdentityDotNet.Components.Account;
using BlazorAppIdentityDotNet.Data;
using BlazorAppIdentityDotNet.Data.Models;
using BlazorAppIdentityDotNet.HttpHandlers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSystemCertificate(builder.Configuration.GetSection("Certificate"));
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization();
builder.Services.AddHttpClient();
builder.Services.AddScoped(sp => new HttpClient(new CustomHeaderHandler()) { BaseAddress = new Uri("https://localhost:5001/") });

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
/////////////////////////////////
builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.User.RequireUniqueEmail = true;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();
;
builder.Services.Configure<IdentityOptions>(options =>
{
    options.ClaimsIdentity.RoleClaimType = "role"; // Ensure roles are mapped correctly
});

//////////////////////////////////
builder.Services.AddQuickGridEntityFrameworkAdapter();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthentication(sharedOptions =>
{
    sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    sharedOptions.DefaultChallengeScheme = MPassSamlDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.Cookie.Name = "auth";
    options.LoginPath = "/account/login";
    options.LogoutPath = "/account/logout";
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
    options.Events.OnValidatePrincipal = context =>
    {

        //if (!context.Principal.IsInRole("Administrator"))
        //{
        //    Console.WriteLine("principal rejected");
        //    //context.RejectPrincipal();
        //}
        return Task.CompletedTask;
    };
})
.AddMPassSaml(builder.Configuration.GetSection("MPassSaml"));



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseMigrationsEndPoint();
}

app.UseHttpsRedirection();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BlazorAppIdentityDotNet.Client._Imports).Assembly);


app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.MapMPassSaml();
app.UseAntiforgery();
app.MapStaticAssets();
//app.MapControllers();
app.MapPost("/account/logout", async context =>
{
    var returnUrl = context.Request.Headers["Referer"].ToString() ?? "/backoffice";

    if (context.User.Identity is not { IsAuthenticated: true })
    {
        context.Response.Redirect(returnUrl);
        return;
    }

    context.Response.Cookies.Delete("auth");

    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

    var authProperties = new AuthenticationProperties
    {
        RedirectUri = returnUrl
    };

    await context.SignOutAsync(MPassSamlDefaults.AuthenticationScheme, authProperties);
});


// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();
app.Run();
