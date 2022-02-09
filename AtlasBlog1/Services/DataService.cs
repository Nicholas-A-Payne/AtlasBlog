using AtlasBlog1.Data;
using Microsoft.EntityFrameworkCore;

namespace AtlasBlog1.Services
{
    public class DataService
    {
        //Calling a method or an instruction that executes the migrations
        readonly ApplicationDbContext _dbContext;

        public DataService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SetupDbAsync() 
        {
        
            //Run Migrations async
            await _dbContext.Database.MigrateAsync();
        
        }
    }
}
