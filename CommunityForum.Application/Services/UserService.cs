using Azure.Core;
using CommunityForum.Application.DTOs.RequestDTOs;
using CommunityForum.Application.DTOs.ResponseDTOs;
using CommunityForum.Application.Mappers;
using CommunityForum.Application.Authorization;
using CommunityForum.Domain.Entities;
using CommunityForum.Domain.Exceptions;
using CommunityForum.Domain.Interfaces;
using CommunityForum.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Logging;
using CommunityForum.Infrastructure.SignalR;
using Microsoft.AspNetCore.SignalR;
using Azure;

namespace CommunityForum.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly ForumAuthorizationService? _authorizationService;
        private readonly ILogger<UserService> _logger;
        public UserService(IUserRepository userRepository, IConfiguration configuration, ILogger<UserService> logger,
            ForumAuthorizationService? authorizationService = null)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _logger = logger;
            _authorizationService = authorizationService;
        }
        public async Task DeleteUserAsync(DeleteUserRequest request)
        {
            if (request == null)
            {
                _logger.LogError("Attempt to delete user with null request instance.");
                throw new ArgumentNullException(nameof(request), "Delete user request can not be null.");
            }
            var user = await _userRepository.GetByIdAsync(request.Id);
            if (user == null)
            {
                _logger.LogError("Attempt to delete non existing user. User id: {userId}", request.Id);
                throw new KeyNotFoundException($"User with id {request.Id} not found.");
            }
            _authorizationService?.EnsureCanManageUserAccount(user.Id);

            await _userRepository.DeleteAsync(request.Id);
            _logger.LogInformation("User deleted successfully.");
        }

        public async Task<UserResponseDTO> UpdateUserAsync(UpdateUserRequest request)
        {
            if (request == null)
            {
                _logger.LogError("Attempt to update user with null request instance.");
                throw new ArgumentNullException(nameof(request), "Update user request can not be null.");
            }
            if (string.IsNullOrWhiteSpace(request.Username))
            {
                _logger.LogError("Attempt to update username with empty. User id: {userId}", request.Id);
                throw new ArgumentException("Username is required.", nameof(request.Username));
            }
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                _logger.LogError("Attempt to update email with empty. User id: {userId}", request.Id);
                throw new ArgumentException("Email is required", nameof(request.Email));
            }

            var user = await _userRepository.GetByIdAsync(request.Id);
            if (user == null)
            {
                _logger.LogError("Attempt to update non existing user. User id: {userId}", request.Id);
                throw new KeyNotFoundException($"User with id {request.Id} not found.");
            }
            _authorizationService?.EnsureCanManageUserAccount(user.Id);

            user.Email = request.Email;
            user.Username = request.Username;

            await _userRepository.UpdateAsync(user);
            _logger.LogInformation("User updated successfully.");
            return user.ToResponse();
        }
        public async Task<ICollection<UserResponseDTO>?> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            if(users == null)
            {
                _logger.LogInformation("No users to fetch from database.");
                return null;
            }
            _logger.LogInformation("Successfully fetched {count} users from database.", users.Count);
            return users.Select(user => user.ToResponse()).ToList();
        }
        public async Task<UserResponseDTO> GetUserById(Guid id)
        {
            if(id == Guid.Empty)
            {
                _logger.LogError("Atempt to retrieve user with empty id.");
                throw new ArgumentException("User id can not be empty.", nameof(id));
            }
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogError("Atempt to retrieve non existing user. User id: {userId}", id);
                throw new KeyNotFoundException($"User with id {id} not found.");
            }
            _logger.LogInformation("User with id {id} retrieved successfully.", id);
            return user.ToResponse();
        }

        public async Task<UserResponseDTO> RegisterUserAsync(RegisterUserRequest request)
        {
            if(request == null)
            {
                _logger.LogError("Attempt to register user with null request instance.");
                throw new ArgumentNullException(nameof(request), "Register user request can not be null.");
            }
            var existingUser = await _userRepository.GetByUsernameAsync(request.Username);
            if (existingUser != null)
            {
                _logger.LogError("Attempt to register under a taken username. Username: {username}", request.Username);
                throw new UsernameAlreadyExistsException(request.Username);
            }
            var user = new User(request.Username, BCrypt.Net.BCrypt.HashPassword(request.Password), request.Email, Role.User);

            await _userRepository.AddAsync(user);
            _logger.LogInformation("User registered successfully.");
            return user.ToResponse();
        }

        public async Task<string?> LoginUserAsync(LoginUserRequest request)
        {
            if(request == null)
            {
                _logger.LogError("Attempt to register user with null request instance.");
                throw new ArgumentNullException(nameof(request), "Register user request can not be null.");
            }
            var user = await _userRepository.GetByUsernameAsync(request.Username);
            if (user == null)
            {
                _logger.LogError("Atempt to login under non existing username. Username: {userName}", request.Username);
                return null;
            }
            if(!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                _logger.LogError("Atempt to login with wrong password.");
                return null;
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]
                ?? throw new KeyNotFoundException("Jwt key not found.")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "AppIssuer",
                audience: "AppAudience",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);
            _logger.LogInformation("Security token successfully created.");
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
