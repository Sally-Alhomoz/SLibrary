//using Microsoft.JSInterop;

//namespace Blazor2.Services
//{
//    public interface ITokenService
//    {
//        Task SetToken(string token);
//        Task<string?> GetToken();
//        Task RemoveToken();
//    }

//    public class TokenService : ITokenService
//    {
//        private readonly IJSRuntime _js;

//        public TokenService(IJSRuntime js)
//        {
//            _js = js;
//        }

//        public async Task SetToken(string token)
//        {
//            await _js.InvokeVoidAsync("localStorage.setItem", "jwtToken", token);
//        }

//        public async Task<string?> GetToken()
//        {
//            return await _js.InvokeAsync<string?>("localStorage.getItem", "jwtToken");
//        }

//        public async Task RemoveToken()
//        {
//            await _js.InvokeVoidAsync("localStorage.removeItem", "jwtToken");
//        }
//    }
//}




using Microsoft.JSInterop;

namespace Blazor2.Authentication;

public interface ITokenService
{
    Task SetToken(string token);
    Task<string?> GetToken();
    Task RemoveToken();
}

public class TokenService : ITokenService
{
    private const string Key = "jwtToken";
    private readonly IJSRuntime _js;

    public TokenService(IJSRuntime js) => _js = js;

    public Task SetToken(string token) =>
        _js.InvokeVoidAsync("localStorage.setItem", Key, token).AsTask();

    public async Task<string?> GetToken()
    {
        try
        {
            return await _js.InvokeAsync<string>("localStorage.getItem", Key);
        }
        catch
        {
            return null;
        }
    }

    public Task RemoveToken() =>
        _js.InvokeVoidAsync("localStorage.removeItem", Key).AsTask();
}