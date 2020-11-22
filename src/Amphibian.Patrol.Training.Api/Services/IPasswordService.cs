using Amphibian.Patrol.Training.Api.Models;

namespace Amphibian.Patrol.Training.Api.Services
{
    public interface IPasswordService
    {
        bool CheckPassword(User user, string password);
        byte[] GenerateHash(byte[] password, byte[] salt, int iterations, int length);
        void SetPassword(User user, string password);
    }
}