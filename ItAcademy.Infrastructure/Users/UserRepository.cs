using ItAcademy.Application.Accounts.Constants;
using ItAcademy.Application.Accounts.Helper;
using ItAcademy.Application.Accounts.Repositories;
using ItAcademy.Application.Accounts.Requests;
using ItAcademy.Application.Accounts.Responses;
using ItAcademy.Application.Infrastructure.Errors.CustomExceptions;
using ItAcademy.Application.Infrastructure.Localization.Errors;
using ItAcademy.Domain.UserAggregate;
using ItAcademy.Infrastructure.BaseRepo;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ItAcademy.Infrastructure.Users
{
    public class UserRepository : IUserRepository
    {
        #region Ctor
        private readonly IBaseRepository<AppUser> _repository;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public UserRepository(IBaseRepository<AppUser> repository, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _repository = repository;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        #endregion
        public async Task<AppUser> GetById(CancellationToken token, string id)
        {
            return await _repository.GetQuery().SingleOrDefaultAsync(i => i.Id == id).ConfigureAwait(false);
        }
        public async Task<AppUser> GetByUserName(CancellationToken token, string userName)
        {
            return await _repository.GetQuery().SingleOrDefaultAsync(i => i.UserName == userName).ConfigureAwait(false);
        }
       
        public async Task<bool> AddUser(CancellationToken token, AppUser user)
        {
            await _repository.Create(token, user).ConfigureAwait(false);
            return await _repository.SaveChangesAsync(token).ConfigureAwait(false);
        }
        public async Task<bool> MakeModerator(CancellationToken token, string id)
        {
            var user = await _repository.GetQuery().SingleOrDefaultAsync(i => i.Id == id).ConfigureAwait(false);
            if (user == null)
                throw new DoesntExistsException(ErrorMessages.NotFound);
            await _userManager.RemoveFromRoleAsync(user, RoleType.User.ToString()).ConfigureAwait(false);
            await _userManager.AddToRoleAsync(user, RoleType.Moderator.ToString()).ConfigureAwait(false);
            _repository.Update(user);
            return await _repository.SaveChangesAsync(token).ConfigureAwait(false);
        }
        public async Task<bool> DeleteUser(CancellationToken token, string userId)
        {
            var user = await _repository.GetQuery().SingleOrDefaultAsync(i => i.Id == userId).ConfigureAwait(false);
            if (user == null)
                throw new DoesntExistsException(ErrorMessages.NotFound);
            user.Status = false;
            _repository.Update(user);
            return await _repository.SaveChangesAsync(token).ConfigureAwait(false);
        }
        #region Sing In
        //IdentityServices
        public async Task<bool> LogIn(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email).ConfigureAwait(false);
            if(user == null)
                throw new DoesntExistsException(ErrorMessages.NotFound);
            var passwordHash = PasswordHashGenerator.HashPassword(request.Password);
            if (user.PasswordHash != passwordHash)
                throw new IncorrectInfoException(ErrorMessages.IncorrectInfo);
            var result = await _signInManager.PasswordSignInAsync(user, passwordHash, false, false).ConfigureAwait(false);
            if (result.Succeeded)
                return true;
            return false;
        }
        public async Task<UserRepsonse> SignIn(CancellationToken token, LogInUserRequest request)
        {
            var userExists = await _repository.GetQuery().SingleOrDefaultAsync(i => i.Email == request.Email).ConfigureAwait(false);
            if (userExists == null)
                throw new DoesntExistsException(ErrorMessages.NotFound);
            var passwordHash = PasswordHashGenerator.HashPassword(request.PasswordHash);
            if (userExists.Email == request.Email && userExists.PasswordHash == passwordHash)
                return userExists.Adapt<UserRepsonse>();
            else
                throw new IncorrectInfoException(ErrorMessages.IncorrectInfo);
        }
        #endregion
        #region Sign Up
        public async Task<bool> Signup(RegisterRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email).ConfigureAwait(false);
            if (user != null)
                throw new AlreadyExistsException(ErrorMessages.AlreadyExists);
            var newUser = new AppUser()
            {
                Email = request.Email,
                UserName = request.UserName,
                Gender = request.Gender,
                PasswordHash = request.Password
            };
            var result = await _userManager.CreateAsync(newUser,request.Password).ConfigureAwait(false);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, RoleType.User.ToString()).ConfigureAwait(false);
                var createdUser = await _userManager.FindByEmailAsync(request.Email).ConfigureAwait(false);
                var genareteNewHash = PasswordHashGenerator.HashPassword(createdUser.PasswordHash);
                createdUser.PasswordHash = genareteNewHash;
                _repository.Update(createdUser);
                await _repository.SaveChangesAsync(new CancellationToken()).ConfigureAwait(false);
                return true;
            }
            return false;
        }
        public async Task<string> RegisterUser(CancellationToken token, RegisterUserRequest request)
        {
            var user = request.Adapt<AppUser>();
            var guid = Guid.NewGuid().ToString();
            var checkUser = await _repository.GetQuery().SingleOrDefaultAsync(i => i.Email == request.Email || i.UserName == request.UserName || i.Id == guid).ConfigureAwait(false);
            if (checkUser != null)
                throw new AlreadyExistsException(ErrorMessages.AlreadyExists);
            user.PasswordHash = PasswordHashGenerator.HashPassword(user.PasswordHash);
            user.Email = request.Email;
            user.CreatedAt = DateTimeOffset.Now;
            user.Id = guid;
            await _repository.Create(token, user).ConfigureAwait(false);
            await _repository.SaveChangesAsync(token).ConfigureAwait(false);
            await _userManager.AddToRoleAsync(user, RoleType.User.ToString()).ConfigureAwait(false);
            return user.Id;
        }

        #endregion
        public async Task Signout()
        {
            await _signInManager.SignOutAsync().ConfigureAwait(false);
        }

        public async Task<List<UserRoleResponse>> GetUsersWithRoles(CancellationToken token)
        {
            var allUsers = await _repository.GetQuery(i => i.Status == true).ToListAsync(token).ConfigureAwait(false);
            var result = new List<UserRoleResponse>();
            foreach (var user in allUsers)
            {
                result.Add(new UserRoleResponse
                {
                    Email = user.Email,
                    Id = user.Id,
                    UserName = user.UserName,
                    Roles = (List<string>)await _userManager.GetRolesAsync(user).ConfigureAwait(false)
                });
            }
            return result;
        }

        public async Task<UserRoleResponse> GetUserWithRoles(CancellationToken token, string userId)
        {
            var user = await _repository.GetQuery(i => i.Id == userId).FirstOrDefaultAsync().ConfigureAwait(false);
            if (user == null)
                throw new DoesntExistsException(ErrorMessages.NotFound);
            var result = new UserRoleResponse()
            {
                Email = user.Email,
                Id = user.Id,
                UserName = user.UserName,
                Roles = (List<string>)await _userManager.GetRolesAsync(user).ConfigureAwait(false)
            };
            return result;
        }
    }
}
