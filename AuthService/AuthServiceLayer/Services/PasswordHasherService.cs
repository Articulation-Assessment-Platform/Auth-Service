using System.Security.Cryptography;
using System.Text;

namespace AuthServiceLayer.Services
{
    public class PasswordHasherService
    {
        public (string HashedPassword, byte[] Salt) HashPassword(string password)
        {
            // Generate a random salt
            byte[] saltBytes = new byte[16];
            new RNGCryptoServiceProvider().GetBytes(saltBytes);

            // Combine salt with password
            string combinedValue = password + Convert.ToBase64String(saltBytes);

            // Hash the combined value using SHA256
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combinedValue));
                string hashedPassword = Convert.ToBase64String(hashedBytes);
                return (hashedPassword, saltBytes);
            }
        }

        public bool VerifyPassword(string password, string hashedPassword, byte[] salt)
        {
            // Combine salt with provided password
            string combinedValue = password + Convert.ToBase64String(salt);

            // Hash the combined value using SHA256
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combinedValue));
                string hashedPasswordToCompare = Convert.ToBase64String(hashedBytes);
                return hashedPasswordToCompare == hashedPassword;
            }
        }
    }
}
