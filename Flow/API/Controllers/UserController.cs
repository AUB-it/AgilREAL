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
    [Route("api/register")]
    public IActionResult RegisterUser(User user)
    {
        if (user == null || string.IsNullOrWhiteSpace(user.Name) || string.IsNullOrWhiteSpace(user.Password))
            return BadRequest();
        
        try 
        {
            // 2. Forsøg at registrere brugeren via repository
            var repoResult = _userRepository.Register(user);
            
            // Hvis navnet allerede findes (repo returnerer false)
            if (!repoResult)
                return BadRequest();
                
            return Ok(user);
        }
        catch (Exception ex)
        {
            // TEST 7: Dette sikrer at vi returnerer 500 hvis databasen fejler i stedet for at crashe
            Console.WriteLine($"Fejl ved registrering: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    [Route("users/login")]
    public IActionResult Login(UserLogin loginRequest)
    {
        if (loginRequest == null || string.IsNullOrEmpty(loginRequest.username) || string.IsNullOrEmpty(loginRequest.password))
            return BadRequest();
        
        User? userExist = _userRepository.Login(loginRequest.username, loginRequest.password);
        if (userExist == null)
            return Unauthorized();
        return Ok(userExist);
    }
}

public record UserLogin(string username, string password);