using DataAccess.Models;
using BusinessLayer.DTOs; // For RegisterRequestDto

namespace BusinessLayer.Interfaces
{
    public interface IUserFactory
    {
        User CreateUser(RegisterRequestDto dto, string hashedPassword);
    }
}
