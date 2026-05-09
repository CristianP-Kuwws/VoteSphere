using eVote470Plus.Core.Application.Dtos.ApplicationUser;
using eVote470Plus.Core.Application.Dtos.ApplicationUser.Responses;
using eVote470Plus.Core.Application.Dtos.People;
using eVote470Plus.Core.Application.Interfaces.ApplicationUser;
using eVote470Plus.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace eVote470Plus.Infrastructure.Identity.Services
{
    public class AccountServicesForWebApp : IAccountServicesForWebApp
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public AccountServicesForWebApp(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager) 
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // Auth
        public async Task SignOut()
        {
            await _signInManager.SignOutAsync();
        }

        // CRUD
        public async Task<UserResponseDto> CreateUserAsync(CreateApplicationUserDto dto)
        {
            UserResponseDto response = new()
            {
                HasError = false,
                Errors = new List<string>()
            };

            var userWithSameUsername = await _userManager.FindByNameAsync(dto.UserName);
            if (userWithSameUsername != null)
            {
                response.HasError = true;
                response.Errors.Add($"The UserName '{dto.UserName}' is already in use.");
            }

            var userWithSameEmail = await _userManager.FindByEmailAsync(dto.Email);
            if (userWithSameEmail != null)
            {
                response.HasError = true;
                response.Errors.Add($"The Email '{dto.Email}' is already in use.");
            }

            if (response.HasError)
            {
                return response;
            }

            ApplicationUser newUser = new ApplicationUser()
            {
                Name = dto.Name,
                LastName= dto.LastName,
                Email = dto.Email,
                UserName = dto.UserName,
                IsActive = dto.IsActive
            };

            var result = await _userManager.CreateAsync(newUser, dto.Password);

            if (!result.Succeeded)
            {
                response.HasError = true;
                response.Errors.AddRange(result.Errors.Select(e => e.Description));
                return response;
            }

            await _userManager.AddToRoleAsync(newUser, dto.Role);

            response.Id = newUser.Id;
            response.Name = newUser.Name;
            response.LastName = newUser.LastName;
            response.Email = dto.Email ?? "";
            response.UserName = dto.UserName ?? "";
            response.Role = dto.Role;
            response.IsActive = newUser.IsActive;

            return response;
        }

        public async Task<UserResponseDto> EditUserAsync(EditApplicationUserDto dto)
        {
            UserResponseDto response = new()
            {
                HasError = false,
                Errors = new List<string>()
            };

            var user = await _userManager.FindByIdAsync(dto.Id);
            if (user == null)
            {
                response.HasError = true;
                response.Errors.Add($"There's no account registered for this user.");
                return response;
            }

            var userWithSameUsername = await _userManager.FindByNameAsync(dto.UserName);
            if (userWithSameUsername != null && userWithSameUsername.Id != dto.Id)
            {
                response.HasError = true;
                response.Errors.Add($"The UserName '{dto.UserName}' is already in use.");
            }

            var userWithSameEmail = await _userManager.FindByEmailAsync(dto.Email);
            if (userWithSameEmail != null && userWithSameEmail.Id != dto.Id)
            {
                response.HasError = true;
                response.Errors.Add($"The Email '{dto.Email}' is already in use.");
            }

            if (response.HasError)
            {
                return response;
            }

            user.Name = dto.Name;
            user.LastName = dto.LastName;
            user.Email = dto.Email ?? "";
            user.UserName = dto.UserName;
            user.IsActive = dto.IsActive;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                response.HasError = true;
                response.Errors.AddRange(updateResult.Errors.Select(e => e.Description));
                return response;
            }

            if (!string.IsNullOrWhiteSpace(dto.Password)) // mejorable
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordResult = await _userManager.ResetPasswordAsync(user, token, dto.Password);

                if (!passwordResult.Succeeded)
                {
                    response.HasError = true;
                    response.Errors.AddRange(passwordResult.Errors.Select(e => e.Description));
                    return response;
                }
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
            }

            await _userManager.AddToRoleAsync(user, dto.Role);

            response.Id = user.Id;
            response.Name = user.Name;
            response.LastName = user.LastName;
            response.Email = user.Email ?? "";
            response.UserName = user.UserName ?? "";
            response.Role = dto.Role;
            response.IsActive = user.IsActive;

            return response;


        }

        public async Task<ApplicationUserResponseDto> ToggleActiveAsync(string id)
        {
            ApplicationUserResponseDto response = new()
            {
                HasError = false,
                Errors = new List<string>()
            };

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                response.HasError = true;
                response.Errors.Add("There's no account registered for this user.");
                return response;
            }

            user.IsActive = !user.IsActive;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                response.HasError = true;
                response.Errors.AddRange(result.Errors.Select(e => e.Description));
            }

            return response;
        }

        public async Task<ApplicationUserResponseDto> DeleteAsync(string id)
        {
            ApplicationUserResponseDto response = new()
            {
                HasError = false,
                Errors = new List<string>()
            };

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                response.HasError = true;
                response.Errors.Add("There's no account registered for this user.");
                return response;
            }

            await _userManager.DeleteAsync(user);
            return response;
        }

       // Requests

        public async Task<List<ApplicationUserDto>> GetAllAsync(bool? isActive = true)
        {
            var listUsersDto = new List<ApplicationUserDto>();
            var users = _userManager.Users;

            if (isActive != null && isActive == true)
            {
                users = users.Where(u => u.IsActive); 
                // en el original solia ser users = users.Where(w => w.EmailConfirmed), pero aqui no usamos EmailConfirmed
            }

            var userList = await users.ToListAsync();

            foreach (var item in userList)
            {
                var roleList = await _userManager.GetRolesAsync(item);

                listUsersDto.Add(new ApplicationUserDto()
                {
                    Id = item.Id,
                    Name = item.Name,
                    LastName = item.LastName,
                    Email = item.Email ?? "",
                    UserName = item.UserName ?? "",
                    IsActive = item.IsActive,

                });
            }

            return listUsersDto;
        }

        public async Task<ApplicationUserDto?> GetUserByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email); 

            if (user == null)
            {
                return null;
            }

            //var rolesList = await _userManager.GetRolesAsync(user);

            var userDto = new ApplicationUserDto()
            {
                Id = user.Id,
                Name = user.Name,
                LastName = user.LastName,
                Email = user.Email ?? "",
                UserName = user.UserName ?? "",
                IsActive = user.IsActive,
            };

            return userDto;

        }

        public async Task<ApplicationUserDto?> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return null;
            }

            //var rolesList = await _userManager.GetRolesAsync(user);

            var userDto = new ApplicationUserDto()
            {
                Id = user.Id,
                Name = user.Name,
                LastName = user.LastName,
                Email = user.Email ?? "",
                UserName = user.UserName ?? "",
                IsActive = user.IsActive,
            };

            return userDto;

        }

        public async Task<ApplicationUserDto?> GetUserByUsername(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            //var rolesList = await _userManager.GetRolesAsync(user);

            var userDto = new ApplicationUserDto()
            {
                Id = user.Id,
                Name = user.Name,
                LastName = user.LastName,
                Email = user.Email ?? "",
                UserName = user.UserName ?? "",
                IsActive = user.IsActive,
            };

            return userDto;
        }

        public async Task<List<string>> GetRolesByUserIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return new List<string>();

            var roles = await _userManager.GetRolesAsync(user);
            return roles.ToList();
        }
    }
}
