using System.Threading.Tasks;
using AuthorisationApi.Models;

namespace AuthorisationApi.TokenProviders
{
    public interface ITokenProvider
    {
        Task<string> GenerateToken(User user);
    }
}