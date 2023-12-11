using ChallengeCFOTech.Models.Dtos;
using ChallengeCFOTech.Models;
using ChallengeCFOTech.Repositories.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiPeliculas.Data;
using ApiPeliculas.Utils;

namespace ChallengeCFOTech.Repositories
{
    public class UserRepository : IUser
    {

        private readonly ApplicationDbContext _db;
        private string _secretKey;

        public UserRepository(ApplicationDbContext db, IConfiguration config)
        {
            _db = db;
            _secretKey = config.GetValue<string>("ApiSettings:SecretKey");
        }

        public User GetUser(int userId)
        {
            return _db.Users.FirstOrDefault(user => user.Id == userId);
        }

        public ICollection<User> GetUsers()
        {
            return _db.Users.OrderBy(user => user.UserName).ToList();
        }

        public bool IsUniqueUser(string username)
        {
            var userDb = _db.Users.FirstOrDefault(user => user.UserName == username);

            return userDb == null ? true : false;
        }

        public async Task<User> Register(UserRegisterDto userRegisterDto)
        {
            var passwordHash = Helpers.hashPassword(userRegisterDto.Password);

            User user = new User()
            {
                UserName = userRegisterDto.UserName,
                Password = passwordHash,
                Role = userRegisterDto.Role,
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return user;
        }



        public async Task<UserAuthDto> Login(UserLoginDto userLoginDto)
        {
            var passwordHash = Helpers.hashPassword(userLoginDto.Password);

            var user = _db.Users.FirstOrDefault(
                    user => user.UserName == userLoginDto.UserName.Trim() &&
                    user.Password == passwordHash
                  );

            if (user == null)
            {
                return new UserAuthDto()
                {
                    UserName = "",
                    Token = ""
                };
            }

            var handleToken = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserName.ToString()),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = handleToken.CreateToken(tokenDescriptor);

            UserAuthDto userAuthDto = new UserAuthDto()
            {
                UserName = user.UserName,
                Token = handleToken.WriteToken(token),
            };

            return userAuthDto;
        }
    }
}
