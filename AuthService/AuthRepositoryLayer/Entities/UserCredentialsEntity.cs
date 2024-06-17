using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json.Serialization;

namespace AuthRepositoryLayer.Entities
{
    public class UserCredentialsEntity
    {
        public int Id {  get; set; }
        public int UserID { get; set; }     
        public string Email { get; set; }
        public string HashedPassword { get; set; }
        public Byte[] Salt { get; set; }
        public string Role {  get; set; }
    }
}
