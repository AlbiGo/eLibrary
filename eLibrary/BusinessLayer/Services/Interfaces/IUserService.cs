using eLibrary.ViewModels;

namespace eLibrary.BusinessLayer.Services.Interfaces
{
    public interface IUserService
    {
        public Task RegisterUser(UserRegisterVM userRegisterVM);
        public Task<string> LogIn(LogInVM logInVM);
        public Task Logoff();
        public Task<UserProfileVM> GetUserProfile();

    }
}
