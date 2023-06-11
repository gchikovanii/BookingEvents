using ItAcademy.Application.Accounts.Requests;
using ItAcademy.Application.Accounts.Responses;

namespace ItAcademy.Application.Accounts
{
    public interface IUserService
    {
        Task<string> RegisterUser(CancellationToken token, RegisterUserRequest request);
        Task<UserRepsonse> SignIn(CancellationToken token, LogInUserRequest request);
        Task<bool> DeactivateAccount(CancellationToken token, string userId);
        Task<bool> MakeModerator(CancellationToken token, string id);
        Task<bool> LogIn(LoginRequest request);
        Task<bool> Register(RegisterRequest request);
        Task Logout();
        Task<List<UserRoleResponse>> GetUsersWithRoles(CancellationToken token);
        Task<UserRoleResponse> GetUserWithRoles(CancellationToken token, string userId);
    }
}
