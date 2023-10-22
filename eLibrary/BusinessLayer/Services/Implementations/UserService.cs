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
using Microsoft.EntityFrameworkCore;

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
                    //IsAdmin = true,
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
                throw new Exception("Error in registering user");
            }
        }

        // To generate token
        private string GenerateToken(Client user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("eLibrary123131414"));
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

        public async Task Logoff()
        {
            try
            {
                await _httpContextAccessor.HttpContext.SignOutAsync();
            }
            catch(Exception ex)
            {
                throw new Exception("Error in logging off");
            }
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

                //Add Admin claims
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Email, client.Email),
                    new Claim(ClaimTypes.NameIdentifier, client.ID.ToString()),
                    new Claim("Token", token)
                };

                //If user is admin add admin claim
                if (client.IsAdmin)
                {
                    claims.Add(new Claim("IsAdmin", client.IsAdmin.ToString()));
                }

                //Security informarition
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                AuthenticationProperties properties = new AuthenticationProperties()
                {
                    AllowRefresh = true,
                    IsPersistent = true,
                };

                //Sign in on the session
                await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                 new ClaimsPrincipal(claimsIdentity), properties);

                return token;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in log in");
            }
        }

        public async Task BorrowBook(int bookID)
        {
            try
            {
                var book = _eLibraryDbContext.Books
                    .Where(p => p.ID == bookID)
                    .FirstOrDefault();

                //Get logged user from session
                #region 
                var userClaim = _httpContextAccessor.HttpContext.User.Claims
                    .Where(p => p.Type == ClaimTypes.NameIdentifier)
                    .FirstOrDefault();

                if (userClaim == null)
                {
                    throw new Exception("User is not authenticated");
                }

                var userID = int.Parse(userClaim.Value);

                var user = await _eLibraryDbContext.Clients
                    .Where(p => p.ID == userID)
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    throw new Exception("User account does not exist");
                }
                #endregion
                if (book == null)
                    throw new Exception("Book does not exist");

                var clientBook = new BookClient()
                {
                    BookID = book.ID,
                    ClientID = user.ID,
                    BorrowedDate = DateTime.Now
                };

                _eLibraryDbContext.BookClients.Add(clientBook);
                _eLibraryDbContext.SaveChanges();

                book.Stock--;
                _eLibraryDbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in borrowing book");
            }
        }

        public async Task<UserProfileVM> GetUserProfile()
        {
            try
            {
                //Get logged user from session
                #region 
                var userClaim = _httpContextAccessor.HttpContext.User.Claims
                    .Where(p => p.Type == ClaimTypes.NameIdentifier)
                    .FirstOrDefault();

                if (userClaim == null)
                {
                    throw new Exception("User is not authenticated");
                }

                var userID = int.Parse(userClaim.Value);

                var user = await _eLibraryDbContext.Clients
                    .Where(p => p.ID == userID)
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    throw new Exception("User account does not exist");
                }
                #endregion

                var userProfileVM = new UserProfileVM()
                {
                    FirstName = user.FirstName,
                    LastnName = user.LastnName,
                    Email = user.Email,
                    FullName = user.FullName,
                    ProfilePicUrl = "/Images/1600w-2PE9qJLmPac.WEBP"
                };

                return userProfileVM;
            }

            catch (Exception ex)
            {
                throw new Exception("Error in getting user profile");
            }
        }
    }
}
