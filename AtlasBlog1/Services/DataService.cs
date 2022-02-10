using AtlasBlog1.Data;
using AtlasBlog1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AtlasBlog1.Services
{
    public class DataService
    {
        //Calling a method or an instruction that executes the migrations
        readonly ApplicationDbContext _dbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public DataService(ApplicationDbContext dbContext,
                            RoleManager<IdentityRole> roleManager,
                            UserManager<AppUser> userManager)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task SetupDbAsync() 
        {
        
            //Run Migrations async
            await _dbContext.Database.MigrateAsync();

            //Add a few roles into the system
            await SeedRolesAsync();


            //Assign those users to a role
            await SeedUsersAsync();

        }

        private async Task SeedRolesAsync()
        {
            if (_roleManager.Roles.Count() == 0)
            {
                await _roleManager.CreateAsync(new IdentityRole("Administrator"));
                await _roleManager.CreateAsync(new IdentityRole("Moderator"));
            }
        }
        private async Task SeedUsersAsync()
        {


            try
            {
                AppUser user = new()
                {
                    UserName = "nicholaspayne2703@gmail.com",
                    Email = "nicholaspayne2703@gmail.com",
                    FirstName = "Nicholas",
                    LastName = "Payne",
                    DisplayName = "Prof",
                    PhoneNumber = "4404541791",
                    EmailConfirmed = true
                };


                var newUser = await _userManager.FindByEmailAsync(user.Email);
                if(newUser is null)
                {
                    await _userManager.CreateAsync(user, "Abc&123!");
                    await _userManager.AddToRoleAsync(user, "Administrator");
                }
                
                user = new()
                {
                    UserName = "DrewRussell@Mailinator.com",
                    Email = "DrewRussell@Mailinator.com",
                    FirstName = "Drew",
                    LastName = "Russel",
                    DisplayName = "Prof 2",
                    PhoneNumber = "666777888",
                    EmailConfirmed = true
                };

                newUser = await _userManager.FindByEmailAsync(user.Email);
                if (newUser is null)
                {
                    await _userManager.CreateAsync(user, "Abc&123!");
                    await _userManager.AddToRoleAsync(user, "Moderator");
                }

            }
            catch (Exception ex)
            {

            }

        }
    }
}
