using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using AlMualim.Data;
using AlMualim.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AlMualim.Models
{
    public class SeedUserData
    {
        public async static Task Initialize(IServiceProvider serviceProdiver)
        {
            try
            {
                var _context = serviceProdiver.GetService<AlMualimDbContext>();
                var config = serviceProdiver.GetService<IConfiguration>();
                var userName = config.GetValue<string>("BaseUserName");
                var basePassword = config.GetValue<string>("BasePassword");

                var user = new IdentityUser
                {
                    UserName = userName,
                    NormalizedUserName = userName,
                    Email = userName,
                    NormalizedEmail = userName,
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                var roleStore = new RoleStore<IdentityRole>(_context);

                if (!_context.Roles.Any(r => r.Name == "admin"))
                {
                    await roleStore.CreateAsync(new IdentityRole { Name = "admin", NormalizedName = "admin" });
                }

                if (!_context.Users.Any(u => u.UserName == user.UserName))
                {
                    var password = new PasswordHasher<IdentityUser>();
                    var hashed = password.HashPassword(user, basePassword);
                    user.PasswordHash = hashed;
                    var userStore = new UserStore<IdentityUser>(_context);
                    await userStore.CreateAsync(user);
                    await userStore.AddToRoleAsync(user, "admin");
                }

                await _context.SaveChangesAsync();
            }
            catch
            {
                return;
            }
        }

        public static async Task<IdentityResult> AssignRoles(IServiceProvider services, string email, string roles)
        {
            UserManager<IdentityUser> _userManager = services.GetService<UserManager<IdentityUser>>();
            IdentityUser user = await _userManager.FindByEmailAsync(email);
            var result = await _userManager.AddToRoleAsync(user, roles);

            return result;
        }
    }
}
