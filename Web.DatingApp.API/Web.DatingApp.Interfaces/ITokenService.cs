using Web.DatingApp.API.Web.DatingApp.Entities;

namespace Web.DatingApp.API.Web.DatingApp.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser appUser);
    }
}
