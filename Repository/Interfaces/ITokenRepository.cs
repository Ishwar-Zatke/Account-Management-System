using Account_Management.Models.Domain;

namespace Account_Management.Repository.Interfaces
{
    public interface ITokenRepository
    {
        string CreateJwtToken(User user, List<string> roles);
    }
}
