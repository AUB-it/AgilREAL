using API.Models;

namespace API.Repositories;

public interface IUserRepository
{
    public User? Login(string username, string password);
    public IResult Register(User user);
}