using Microsoft.EntityFrameworkCore;
namespace AuthRepositoryLayer.Entities
{
    public class AuthContext : DbContext
    {
        public AuthContext(DbContextOptions<AuthContext> options) : base(options)
        { }

        public DbSet<UserCredentialsEntity> Users { get; set; }
    }
}
