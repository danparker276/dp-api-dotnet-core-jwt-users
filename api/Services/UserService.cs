using dp.api.Helpers;
using dp.business.Enums;
using dp.business.Models;
using dp.data;
using dp.data.Interfaces;
using fox.datasimple.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace dp.api.Services
{
    public interface IUserService
    {
        Task<AccessToken> Authenticate(string email, string password, UserType userType);

    }

    public class UserService : IUserService
    {

        private readonly AppSettings _appSettings;
        private string _dpDbConnectionString;
        private IDaoFactory AdoDao => DaoFactories.GetFactory(DataProvider.AdoNet, _dpDbConnectionString);


        public UserService(string dpDbConnectionString, IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
            _dpDbConnectionString = dpDbConnectionString;
        }

        public async Task<AccessToken> Authenticate(string email, string password, UserType userType)
        {

            User user = await AdoDao.UserDao.ValidateUser(email, password, userType);

            // return null if user not found
            if (user == null)
                return null;
            if (user.IsActive == false)
                return null;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(24),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            AccessToken ur = new AccessToken()
            {
                 access_token = tokenHandler.WriteToken(token)
            };
            return ur;
        }


    }
}