using eVote470Plus.Core.Application.Dtos.ApplicationUser;
using eVote470Plus.Core.Application.Dtos.ApplicationUser.Responses;
using eVote470Plus.Core.Application.Dtos.People;

namespace eVote470Plus.Core.Application.Interfaces.ApplicationUser
{
    public interface IAccountServicesForWebApp
    {

        // Auth
        //Task<LoginResponseDto> AuthenticateAsync(LoginDto loginDto);
        Task SignOut();

        // CRUD
        Task<UserResponseDto> CreateUserAsync(CreateApplicationUserDto dto);
        Task<UserResponseDto> EditUserAsync(EditApplicationUserDto dto);
        Task<ApplicationUserResponseDto> ToggleActiveAsync(string id);
        Task<ApplicationUserResponseDto> DeleteAsync(string id);


        // Requests
        Task<ApplicationUserDto?> GetUserByEmail(string email);
        Task<ApplicationUserDto?> GetUserByUsername(string username);
        Task<ApplicationUserDto?> GetUserById(string id);
        Task<List<ApplicationUserDto>> GetAllAsync(bool? isActive = true);
        Task<List<string>> GetRolesByUserIdAsync(string id);

    }
}
