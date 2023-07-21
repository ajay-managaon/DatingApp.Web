using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Web.DatingApp.API.Web.DatingApp.Entities;

namespace Web.DatingApp.API.Web.DatingApp.Database
{
    public class SeedData
    {
        public static async Task SeedUsers(DatingAppDbContext datingAppDbContext)
        {
            if ( await datingAppDbContext.tbl_User.AnyAsync())
            {
                return;
            }
            else
            {
                var userData = await File.ReadAllTextAsync("Web.DatingApp.Database/UserSeedData.json");
                var users = JsonSerializer.Deserialize<List<AppUser>>(userData);
                if (users?.Count > 0)
                {
                    foreach (var user in users)
                    {
                        using (var hmac = new HMACSHA512())
                        {
                            user.UserName = user.UserName?.ToLower();
                            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Managaon"));
                            user.PasswordSalt = hmac.Key;
                            datingAppDbContext.tbl_User.Add(user);
                        }
                    }
                }
                datingAppDbContext.SaveChanges();
            }
        }
    }
}
