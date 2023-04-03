﻿using GenshinTheoryCrafting.Controllers.Auth;
using GenshinTheoryCrafting.Models;
using GenshinTheoryCrafting.Models.Dto.User;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace GenshinTheoryCrafting.Services.User
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public static Users user = new Users();
        public UserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetMyName()
        {
            var result = string.Empty;

            if (_httpContextAccessor.HttpContext != null)
                result = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);

            return result;
        }

        public string Login(UserDto request, IConfiguration configuration)
        {
            if (request.Username == null)
                throw new ArgumentNullException(nameof(request.Username));

            if (user.Username != request.Username)
                return "Not Found";

            if (user.Email != request.Email)
                return "Not Found";

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return "Not Found";

            CrtToken crtToken = new CrtToken(configuration);

            string token = crtToken.CreateToken(user);

            return token;
        }

        public Users Register(UserDto request)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            user.Username = request.Username;
            user.Email = request.Email;
            user.PasswordHash = passwordHash;
            user.Admin = false;

            return user; 
        }

        public string RegAdmin(UserDto request, IConfiguration configuration)
        {
            if (request.Username == null)
                throw new ArgumentNullException(nameof(request.Username));

            if (user.Username != request.Username)
                return "Not Found";

            if (user.Email != request.Email)
                return "Not Found";

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return "Bad Request";

            CrtToken crtToken = new CrtToken(configuration);

            string token = crtToken.CreateToken(user);

            return token;
        }
    }

}
