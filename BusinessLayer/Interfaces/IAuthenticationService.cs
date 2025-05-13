using BusinessLayer.DTOs; // For RegisterRequestDto if it were still here
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IAuthenticationService
    {
        Task<string?> LoginAsync(string email, string password);
        // Register method is removed
        // CreateUser factory method is removed
    }
}
