using Schedule.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Schedule.Api.Services
{
    public class AuthenticationService
    {
        private readonly int _defaultIterations = 2;
        private const int _hashLength = 32;

        public AuthenticationService(int defaultIterations)
        {
            _defaultIterations = defaultIterations;
        }
        public void SetPassword(User user, string password)
        {
            var salt = GenerateSalt(_hashLength);
            var hash = GenerateHash(StringToBytes(password), salt, _defaultIterations, _hashLength);
            user.PasswordSalt = salt;
            user.PasswordHash = hash;
            user.PasswordIterations = _defaultIterations;
        }
        public bool CheckPassword(User user, string password)
        {
            if (user.PasswordHash != null && user.PasswordSalt != null && user.PasswordIterations.HasValue)
            {
                var calculatedHash = GenerateHash(StringToBytes(password), user.PasswordSalt, user.PasswordIterations.Value, _hashLength);
                return calculatedHash.SequenceEqual(user.PasswordHash);
            }
            else
            {
                return false;
            }
        }

        private byte[] StringToBytes(string password)
        {
            return System.Text.UTF8Encoding.UTF8.GetBytes(password);
        }

        private byte[] GenerateSalt(int length)
        {
            var bytes = new byte[length];

            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(bytes);
            }

            return bytes;
        }

        public byte[] GenerateHash(byte[] password, byte[] salt, int iterations, int length)
        {
            using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, iterations))
            {
                return deriveBytes.GetBytes(length);
            }
        }
    }
}
