using ChallengeCFOTech.Models.Dtos;
using ChallengeCFOTech.Models;

namespace ChallengeCFOTech.Repositories.Interfaces
{
    public interface IUser
    {
        ICollection<User> GetUsers();
        User GetUser(int userId);
        bool IsUniqueUser(string username);
        Task<UserAuthDto> Login(UserLoginDto userLoginDto);
        Task<User> Register(UserRegisterDto userRegisterDto);
    }
}
