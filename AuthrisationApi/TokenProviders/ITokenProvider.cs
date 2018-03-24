using System.Threading.Tasks;
using AuthrisationApi.Models;

namespace AuthrisationApi.TokenProviders
{
    public interface ITokenProvider
    {
        Task<string> GenerateToken(User user);
    }
}