using FundooNotes.DataLogicLayer.Context;
using FundooNotes.DataLogicLayer.Interfaces;
using FundooNotes.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FundooNotes.DataLogicLayer.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly FundooContext _context;

        public UserRepository(FundooContext context)
        {
            _context = context;
        }
        public async Task<User?> GetUserByEmail(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            return user;
        }

        public async Task<User?> GetUserById(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            return user;
        }

        public async Task<User?> GetUserByResetToken(string token)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.ResetToken == token);
        }

        public async Task<User> RegisterUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateUser(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}
