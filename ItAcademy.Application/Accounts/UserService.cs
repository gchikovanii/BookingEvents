using ItAcademy.Application.Accounts.Repositories;
using ItAcademy.Application.Accounts.Requests;
using ItAcademy.Application.Accounts.Responses;
using ItAcademy.Application.Infrastructure.Errors.CustomExceptions;
using ItAcademy.Application.Infrastructure.Localization.Errors;

namespace ItAcademy.Application.Accounts
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<UserRoleResponse> GetUserWithRoles(CancellationToken token, string userId)
        {
            return await _userRepository.GetUserWithRoles(token, userId).ConfigureAwait(false);
        }

        public async Task<List<UserRoleResponse>> GetUsersWithRoles(CancellationToken token)
        {
            return await _userRepository.GetUsersWithRoles(token).ConfigureAwait(false);
        }

        public async Task<bool> MakeModerator(CancellationToken token, string id)
        {
            var userExists = await _userRepository.GetById(token, id).ConfigureAwait(false);
            if (userExists == null)
                throw new DoesntExistsException(ErrorMessages.NotFound);
            return await _userRepository.MakeModerator(token, id).ConfigureAwait(false);
        }
        public async Task<bool> DeactivateAccount(CancellationToken token, string userId)
        {
            var userExists = await _userRepository.GetById(token, userId).ConfigureAwait(false);
            if (userExists == null)
                throw new DoesntExistsException(ErrorMessages.NotFound);
            return await _userRepository.DeleteUser(token, userId).ConfigureAwait(false);
        }

        #region Sign in
        public async Task<bool> LogIn(LoginRequest request)
        {
            return await _userRepository.LogIn(request).ConfigureAwait(false);
        }
        public async Task<UserRepsonse> SignIn(CancellationToken token, LogInUserRequest request)
        {
            return await _userRepository.SignIn(token, request).ConfigureAwait(false);
        }
        #endregion
        #region Sign Up
        public async Task<bool> Register(RegisterRequest request)
        {
            return await _userRepository.Signup(request).ConfigureAwait(false);
        }
        public async Task<string> RegisterUser(CancellationToken token, RegisterUserRequest request)
        {
            return await _userRepository.RegisterUser(token, request).ConfigureAwait(false);
        }
        #endregion

        public async Task Logout()
        {
            await _userRepository.Signout().ConfigureAwait(false);
        }

    }
}
