using Microsoft.AspNetCore.Mvc;
using API.Models;
using API.Repositories;
using API.Repositories;
namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    
    private readonly IUserRepository _userRepository;
    
    public UsersController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    [HttpPost]
    public IActionResult RegisterUser(User user)
    {
        _userRepository.Register(user);
        Console.WriteLine("User Registered");
        return Ok(user);
    }

    [HttpPost]
    [Route("users/login")]
    public IActionResult Login(UserLogin loginRequest)
    {
        User? userExist = _userRepository.Login(loginRequest.username, loginRequest.password);
        if (userExist == null)
            return Unauthorized();
        return Ok(userExist);
    }
}

public record UserLogin(string username, string password);