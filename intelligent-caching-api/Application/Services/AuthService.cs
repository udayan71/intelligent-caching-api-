using Application.Constants;
using Application.DTOs.Auth;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IPasswordHasher passwordHasher,
            IJwtTokenService jwtTokenService)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<AuthResponseDto> RegisterAsync(
            RegisterRequestDto request)
        {
            var existingUser =
                await _userRepository.GetByEmailAsync(
                    request.Email);

            if (existingUser != null)
            {
                throw new ApplicationException(
                    "Email already exists.");
            }

            var viewerRole =
                await _roleRepository.GetByNameAsync(
                    Roles.Viewer);

            if (viewerRole == null)
            {
                throw new ApplicationException(
                    "Viewer role not found.");
            }

            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash =
                    _passwordHasher.HashPassword(
                        request.Password),

                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            user.UserRoles.Add(
                new UserRole
                {
                    RoleId = viewerRole.Id
                });

            await _userRepository.AddAsync(user);

            await _userRepository.SaveChangesAsync();

            var token =
                _jwtTokenService.GenerateToken(user);

            return new AuthResponseDto
            {
                Token = token,
                Email = user.Email,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60)
            };
        }

        public async Task<AuthResponseDto> LoginAsync(
            LoginRequestDto request)
        {
            var user =
                await _userRepository.GetByEmailAsync(
                    request.Email);

            if (user == null)
            {
                throw new ApplicationException(
                    "Invalid email or password.");
            }

            if (!user.IsActive)
            {
                throw new ApplicationException(
                    "Account is inactive.");
            }

            var isPasswordValid =
                _passwordHasher.VerifyPassword(
                    request.Password,
                    user.PasswordHash);

            if (!isPasswordValid)
            {
                throw new ApplicationException(
                    "Invalid email or password.");
            }

            var token =
                _jwtTokenService.GenerateToken(user);

            return new AuthResponseDto
            {
                Token = token,
                Email = user.Email,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60)
            };
        }
    }
}
