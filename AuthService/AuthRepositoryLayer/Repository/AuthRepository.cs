using AuthRepositoryLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace AuthRepositoryLayer.Repository
{
    public class AuthRepository : IAuthRepository
    {
        protected readonly AuthContext _context;

        public AuthRepository(AuthContext context)
        {
            _context = context;
        }

        public UserCredentialsEntity GetUserByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }

        public UserCredentialsEntity GetUserByUserId(int id)
        {
           return _context.Users.FirstOrDefault(u => u.UserID == id);
        }
        public async Task<UserCredentialsEntity> AddAsync(UserCredentialsEntity entity)
        {
            await _context.Set<UserCredentialsEntity>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public void DeleteAsync(UserCredentialsEntity entity)
        {
            _context.Set<UserCredentialsEntity>().Remove(entity);
            _context.SaveChanges();
        }
    }
}
