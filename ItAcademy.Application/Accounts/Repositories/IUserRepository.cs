using ItAcademy.Application.Accounts.Requests;
using ItAcademy.Application.Accounts.Responses;
using ItAcademy.Domain.UserAggregate;

namespace ItAcademy.Application.Accounts.Repositories
{
    public interface IUserRepository
    {
        Task<bool> AddUser(CancellationToken token, AppUser request);
        Task<AppUser> GetByUserName(CancellationToken token, string userName);
        Task<AppUser> GetById(CancellationToken token, string id);
        Task<bool> MakeModerator(CancellationToken token, string id);
        Task<bool> DeleteUser(CancellationToken token, string userId);

        Task<bool> LogIn(LoginRequest request);
        Task<UserRepsonse> SignIn(CancellationToken token, LogInUserRequest request);
        Task<string> RegisterUser(CancellationToken token, RegisterUserRequest request);
        Task<bool> Signup(RegisterRequest request);
        Task Signout();
        Task<List<UserRoleResponse>> GetUsersWithRoles(CancellationToken token);
        Task<UserRoleResponse> GetUserWithRoles(CancellationToken token, string userId);
    }
}
