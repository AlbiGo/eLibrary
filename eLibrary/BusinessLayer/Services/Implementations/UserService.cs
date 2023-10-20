using eLibrary.BusinessLayer.Services.Interfaces;
using eLibrary.DataLayers.Context;
using eLibrary.DataLayers.Entities;
using eLibrary.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using Microsoft.AspNetCore.Identity;


namespace eLibrary.BusinessLayer.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly eLibraryDbContext _eLibraryDbContext;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(eLibraryDbContext eLibraryDbContext,
            IHttpContextAccessor httpContextAccessor)
        {
            _eLibraryDbContext = eLibraryDbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// User register
        /// </summary>
        /// <param name="userRegisterVM"></param>
        /// <returns></returns>
        public async Task RegisterUser(UserRegisterVM userRegisterVM)
        {
            try
            {
                var existsUser = _eLibraryDbContext.Clients
                    .Where(p => p.Email == userRegisterVM.Email)
                    .FirstOrDefault();

                if (existsUser != null)
                {
                    throw new Exception("There is already an account with this email");
                }

                var newClient = new Client()
                {
                    Email = userRegisterVM.Email,
                    IsAdmin = true,
                    Created = DateTime.Now,
                    DataRegjistrimit = DateTime.Now,
                    FirstName = userRegisterVM.FirstName,
                    LastnName = userRegisterVM.LastName,
                    FullName = userRegisterVM.FirstName + " " + userRegisterVM.LastName,
                    Password = userRegisterVM.Password
                };

               await _eLibraryDbContext.Clients.AddAsync(newClient);
               await _eLibraryDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // To generate token
        private string GenerateToken(Client user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ACDt1vR3lXToPQ1g3MyN"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            
            var claims = new[]
            {
               new Claim(ClaimTypes.NameIdentifier,user.FullName),
                new Claim(ClaimTypes.Email,user.Email)
            };

            var token = new JwtSecurityToken("https://localhost:7129/",
                "https://localhost:7129/",
                claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: credentials);


            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Log in
        /// </summary>
        /// <param name="logInVM"></param>
        /// <returns></returns>
        public async Task<string> LogIn(LogInVM logInVM)
        {
            try
            {
                var client = _eLibraryDbContext.Clients
                    .Where(p => p.Email == logInVM.Email &&
                                p.Password == logInVM.Password)
                    .FirstOrDefault();

                if (client == null)
                    throw new Exception("User does not exist");

                //If user exits generate JWT token 
                var token = GenerateToken(client);

                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Email, client.Email),
                    new Claim("Token", token),
                    new Claim("IsAdmin", client.Email)
                };

                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                AuthenticationProperties properties = new AuthenticationProperties()
                {
                    AllowRefresh = true,
                    IsPersistent = true,
                };
                
                await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, 
                    new ClaimsPrincipal(claimsIdentity), properties);

                return token;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
