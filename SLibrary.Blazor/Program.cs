using Blazored.LocalStorage;
using Blazored.Modal;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SLibrary.Blazor;
using SLibrary.Blazor.Services.Account_Services;
using SLibrary.Blazor.Services.Authentication_Services;
using SLibrary.Blazor.Services.Book_Services;
using SLibrary.Blazor.Services.Reservation_Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();

//HttpClient that automatically adds your JWT
builder.Services.AddScoped<JwtAuthorizationMessageHandler>();

builder.Services.AddScoped<HttpClient>(sp =>
{
    var handler = sp.GetRequiredService<JwtAuthorizationMessageHandler>();
    return new HttpClient(handler)
    {
        BaseAddress = new Uri("https://localhost:7037/")
    };
});

builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<CustomAuthStateProvider>();

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IReservationService, ReservationService>();

await builder.Build().RunAsync();
