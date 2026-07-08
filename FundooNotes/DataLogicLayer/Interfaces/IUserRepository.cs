using FundooNotes.Models.Entities;

namespace FundooNotes.DataLogicLayer.Interfaces
{
    public interface IUserRepository
    {
        Task<User> RegisterUser(User user);

        Task<User?> GetUserByEmail(string email);

        Task<User?> GetUserById(int userId);

        Task<User> UpdateUser(User user);

        Task<User?> GetUserByResetToken(string token);
    }
}
