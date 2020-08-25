using RestEase;
using Rexobot.Gumroad.Responses;
using System.Threading.Tasks;

namespace Rexobot.Gumroad
{
    [Header("Accept", "application/json")]
    [Header("Content-Type", "application/json")]
    public interface IGumroadApi
    {
        [Get("user")]
        Task<GetUserResponse> GetCurrentUser(
            [Query("access_token")] string token);

        [Get("products")]
        Task<GetProductsResponse> GetProductsAsync(
            [Query("access_token")] string token);

        [Get("products/{id}")]
        Task<GetProductResponse> GetProductAsync(
            [Query("access_token")] string token,
            [Path] string id);

        [Get("sales")]
        Task<GetSalesResponse> GetSalesAsync(
            [Query("access_token")] string token, 
            [QueryMap]GetSalesParams args);
    }
}
