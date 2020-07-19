using RestEase;
using System.Threading.Tasks;

namespace Rexobot.Gumroad
{
    [Header("Accept", "application/json")]
    [Header("Content-Type", "application/json")]
    public interface IGumroadApi
    {
        [Get("sales")]
        Task<GetSalesResponse> GetSales(
            [Header("Authorization")]string access_token, 
            [QueryMap]GetSalesParams args);
    }
}
