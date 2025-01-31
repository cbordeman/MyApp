using BitzArt.Blazor.Cookies;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

namespace TagAuctions.Client;

internal class Program
{
    //internal static bool IsDarkMode;

    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);

        builder.Services.AddBlazoredLocalStorage();
        builder.AddBlazorCookies();

        builder.Services.AddMudServices();

        builder.Services.AddAuthorizationCore();
        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddAuthenticationStateDeserialization();

        var app = builder.Build();

        //var cs = app.Services.GetService<ICookieService>();
        //Cookie? isDarkModeCookie = await cs.GetAsync(nameof(IsDarkMode));
        //bool? isDarkModeSetting = isDarkModeCookie == null ? null :
        //    bool.Parse(isDarkModeCookie.Value);
        //if (isDarkModeSetting != null)
        //    IsDarkMode = isDarkModeSetting.GetValueOrDefault();
        //Console.WriteLine($"At startup, Program.IsDarkMode: {Program.IsDarkMode}");

        await app.RunAsync();
    }
}