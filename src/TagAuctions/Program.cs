using BitzArt.Blazor.Cookies;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using Serilog;
using TagAuctions;
using TagAuctions.Components;
using TagAuctions.Components.Account;
using TagAuctions.Data;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Starting web application");

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddSerilog();

    builder.Services.AddBlazoredLocalStorage();
    builder.AddBlazorCookies();

    // Add MudBlazor services
    builder.Services.AddMudServices();

    // Add services to the container.
    builder.Services.AddRazorComponents()
        .AddInteractiveWebAssemblyComponents()
        .AddAuthenticationStateSerialization();

    builder.Services.AddScoped<AntiforgeryStateProvider, WorkaroundEndpointAntiforgeryStateProvider>();
    
    builder.Services.AddCascadingAuthenticationState();
    builder.Services.AddScoped<IdentityUserAccessor>();
    builder.Services.AddScoped<IHttpContextCookieService, HttpContextCookieService>();
    builder.Services.AddScoped<IdentityRedirectManager>();
    builder.Services.AddHttpContextAccessor();
    
    builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = IdentityConstants.ApplicationScheme;
            options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
        })
        .AddIdentityCookies();
    builder.Services.AddAuthorization();

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString));
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();
    builder.Services.AddCascadingValue(sp =>
    {
        var cs = sp.GetService<IHttpContextCookieService>();
        var str = cs!.GetCookieValue("IsDarkMode");
        bool.TryParse(str, out var isDarkMode);
        Console.WriteLine($"IsDarkMode at startup: {isDarkMode}");
        var source = new CascadingValueSource<bool>("IsDarkMode", isDarkMode, isFixed: true);
        return source;
        //return isDarkMode;
    });

    builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddSignInManager()
        .AddDefaultTokenProviders();

    builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityAzureEmailSender>();
    
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
    }

    app.UseHttpsRedirection();

    app.UseAntiforgery();

    app.MapStaticAssets();
    app.MapRazorComponents<App>()
        .AddInteractiveWebAssemblyRenderMode()
        .AddAdditionalAssemblies(typeof(TagAuctions.Client._Imports).Assembly);

    // Add additional endpoints required by the Identity /Account Razor components.
    app.MapAdditionalIdentityEndpoints();

    app.Run();

    Log.Information("App terminated normally.");
}
catch (Exception e)
{
    Log.Fatal(e, "Application terminated unexpectedly");
}