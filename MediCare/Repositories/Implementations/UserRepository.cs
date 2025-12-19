using MediCare.Dto.Auth;
using MediCareApi.Data;
using MediCareApi.Models;
using MediCareApi.Repositories.Interfaces;
using MediCareDto.Auth;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace MediCareApi.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<long> CreateUserAsync(User user)
        {
            try
            {
                await _context.Users.AddAsync(user);
                var result = await _context.SaveChangesAsync();

                return result > 0 ? user.Id : 0;
            }
            catch (Exception ex) {
                return 0;
            }
        }

        public async Task<User> GetUserByEmail(string email)
        {
            try
            {
                var user = await _context.Users.Where(s => s.Email == email ).FirstOrDefaultAsync();
                if (user == null) {
                    return null;
                }
                return user;
            }
            catch (Exception ex) {
                return null;
            }
        }
    }
}
