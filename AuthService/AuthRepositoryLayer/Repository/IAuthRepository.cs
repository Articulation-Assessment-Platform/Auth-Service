using AuthRepositoryLayer.Entities;

namespace AuthRepositoryLayer.Repository
{
    public interface IAuthRepository
    {
        UserCredentialsEntity GetUserByEmail(string email);
        UserCredentialsEntity GetUserByUserId(int id);
        Task<UserCredentialsEntity> AddAsync(UserCredentialsEntity userCredentials);
        void DeleteAsync(UserCredentialsEntity entity);
    }
}
