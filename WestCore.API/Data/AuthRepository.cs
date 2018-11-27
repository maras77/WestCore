using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WestCore.API.Models;

namespace WestCore.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _contex;

        public AuthRepository(DataContext contex)
        {
            _contex = contex;
        }

        public async Task<User> Login(string username, string password)
        {
            var user = await _contex.Users.FirstOrDefaultAsync(x => x.Username.ToLower() == username.ToLower());
            if(user == null)
                return null;
            
            if(!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for(int i = 0; i < computedHash.Length; i++)
                {
                    if(computedHash[i] != passwordHash[i]) return false;
                }
            }
            return true;
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.Username = user.Username.ToLower();
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _contex.Users.AddAsync(user);
            await _contex.SaveChangesAsync();

            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<bool> UserExists(string username)
        {
            if(await _contex.Users.AnyAsync(x => x.Username.ToLower() == username.ToLower()))
                return true;
            
            return false;
        }
    }
}