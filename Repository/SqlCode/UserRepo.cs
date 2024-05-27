using Account_Management.Data;
using Account_Management.Middleware;
using Account_Management.Models.Domain;
using Account_Management.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Account_Management.Repository.SqlCode
{
    public class UserRepo : IUserRepository
    {
        private readonly ApplicationDbContext dbContext;

        public UserRepo(ApplicationDbContext context)
        {
            dbContext = context;
        }

        public async Task<bool> UserExistsAsync(string userName)
        {
            return await dbContext.Users.AnyAsync(u => u.username == userName);
        }

        public async Task CreateUserAsync(User user)
        {
            using (IDbContextTransaction transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    await dbContext.Users.AddAsync(user);
                    await dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch(CustomerExceptionHandler ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine(ex.Message);
                    throw;
                }
            }
                
        }

        public async Task<User> GetUserByUsernameAsync(string userName)
        {
            return await dbContext.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.username == userName);
        }

        public async Task<Role> GetRoleByNameAsync(string roleName)
        {
            return await dbContext.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
        }

        //public async Task AssignRoleToUserAsync(User user, Role role)
        //{
        //    using (IDbContextTransaction transaction = dbContext.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            user.Role = role;
        //            user.roleType = role.Name;
        //            user.roleId = role.Id;
        //            await dbContext.SaveChangesAsync();
        //            await transaction.CommitAsync();
        //        }
        //        catch (CustomerExceptionHandler ex)
        //        {
        //            await transaction.RollbackAsync();
        //            Console.WriteLine(ex.Message);
        //            throw;
        //        }
        //    }
            

        //}



    }
}
