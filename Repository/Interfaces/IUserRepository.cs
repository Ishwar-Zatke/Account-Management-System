using Account_Management.Models.Domain;

namespace Account_Management.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> UserExistsAsync(string username);
        Task CreateUserAsync(User user);
        Task<User> GetUserByUsernameAsync(string username);
        Task<Role> GetRoleByNameAsync(string roleName);
    }
}
