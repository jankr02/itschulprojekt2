namespace LocalDatabase.Data
{
    public interface IAuthRepository
    {
        Task<ServiceResponse<string>> Login(string username, string password);
        Task<ServiceResponse<int>> Register(User user, string password);

        Task<bool> UserExists(string username);
    }
}
