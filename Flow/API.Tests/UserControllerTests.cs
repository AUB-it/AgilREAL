using API.Controllers;
using API.Models;
using API.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Range = Moq.Range;

namespace API.Tests;

[TestClass]
public sealed class UserControllerTests
{
    [TestMethod]
    public void Valid_Registration_Returns_200_Code_And_Adds_User()
    {
        // Arrange
        var mock = new Mock<IUserRepository>();
        User? savedUser = null;
        var user = new User()
        {
            Id = 24,
            Email = "Umut@hjemmeværnet.dk",
            Name = "Umut",
            Password = "Umut123"
        };
        mock.Setup(r => r.Register(user))
            .Returns(true)
            .Callback<User>(u => savedUser = u);
        
        var controller = new UsersController(mock.Object);
        
        // Act
        var result = controller.RegisterUser(user) as OkObjectResult;
        
        // Assert
        Assert.IsInstanceOfType<OkObjectResult>(result);
        Assert.AreEqual(user, savedUser);
        mock.Verify(r => r.Register(user), Times.Once);
        
    }
    
    [TestMethod]
    public void Username_Already_Exists_Returns_400_Bad_Request()
    {
        // Arrange
        var mock = new Mock<IUserRepository>();
        var controller = new UsersController(mock.Object);
        var user = new User
        {
            Name = "Umut",
            Password = "Umut123",
            Email = "umut@hjemmeværnet.dk"
        };

        mock.Setup(r => r.Register(user))
            .Returns(false);
        // Act
        var result = controller.RegisterUser(user) as  BadRequestResult;
        
        // Assert
        Assert.IsInstanceOfType<BadRequestResult>(result);
        mock.Verify(r => r.Register(user), Times.Once);
    }
    
    [TestMethod]
    public void NULL_DTO_Returns_400_Bad_Request()
    {
        // Arrange
        var mock = new Mock<IUserRepository>();
        mock.Setup(r => r.Register(null))
            .Returns(false);
        mock.Setup(r => r.Login(null, null))
            .Returns((User?)null);
        
        // Act
        var controller = new UsersController(mock.Object);
        
        // Assert
        var result = controller.RegisterUser(null) as BadRequestResult;
        Assert.IsInstanceOfType<BadRequestResult>(result);
    }
    
    [TestMethod]
    public void NoUsername_Returns_400_Bad_Request()
    {
        // Arrange
        var mock = new Mock<IUserRepository>();
        // Repository returnerer false hvis username er tom eller null

        var controller = new UsersController(mock.Object);

        var newUser = new User
        {
            Name = "", // tomt username
            Password = "somepassword"
        };

        // Act
        var result = controller.RegisterUser(newUser) as BadRequestResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
        mock.Verify(r => r.Register(It.IsAny<User>()), Times.Never);
    }
    [TestMethod]
    public void NoPassword_Returns_400_Bad_Request()
    {
        // Arrange
        var mock = new Mock<IUserRepository>();
        var user = new User()
        {
            Id = 24,
            Email = "Umut@hjemmeværnet.dk",
            Name = "Testcase5navn",
            Password = null
        };

        var controller = new UsersController(mock.Object);

        // Act
        var result = controller.RegisterUser(user) as BadRequestResult;

        // Assert
        Assert.IsInstanceOfType<BadRequestResult>(result);
        mock.Verify(r => r.Register(user), Times.Never);
    }

    [TestMethod]
    public void Login_WrongPasswordOrUserNotFound_Returns_401_Unauthorized()
    {
        // Arrange 
        var mock = new Mock<IUserRepository>();
        var controller = new UsersController(mock.Object);
        
        mock.Setup(r => r.Login(It.IsAny<string>(), It.IsAny<string>()))
            .Returns((User?)null);
        
        var loginRequest = new UserLogin("existingUser", "wrongPassword");
        
        // Act
        var result = controller.Login(loginRequest) as UnauthorizedResult;
        
        // Assert 
        Assert.IsNotNull(result);
        Assert.AreEqual(401, result.StatusCode);
        
        mock.Verify(r => r.Login("existingUser", "wrongPassword"));
    }
    
    [TestMethod]
    public void Null_DTO_Login_Returns_400_Bad_Request()
    {
        // Arrange
        var mock = new Mock<IUserRepository>();
        var controller = new UsersController(mock.Object);
        
        // Act 
        var result = controller.Login(null) as BadRequestResult;
        
        // Assert 
        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
        
        mock.Verify(r => r.Login(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
    
    [TestMethod]
    public void WhitespaceUserName_Returns_400_Bad_Request()
    {
        // Arrange
        var mock = new Mock<IUserRepository>();
        var controller = new UsersController(mock.Object);

        var newUser = new User
        {
            Name = "   ",
            Password = "somepassword",
            Email = "test@test.dk"
        };

        // Act
        var result = controller.RegisterUser(newUser) as BadRequestResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
        mock.Verify(r => r.Register(It.IsAny<User>()), Times.Never);
    }
    
    // TEST 7: Repository kaster en fejl under oprettelse
    [TestMethod]
    public void RegisterUser_Returns_500_When_Repository_Throws_Exception()
    {
        // Arrange
        var mock = new Mock<IUserRepository>();
        var user = new User { Name = "John Doe", Password = "password123", Email = "john@doe.com" };
        
        mock.Setup(repo => repo.Register(It.IsAny<User>()))
            .Throws(new System.Exception("Critical Database Failure"));

        var controller = new UsersController(mock.Object);

        // Act
        var result = controller.RegisterUser(user) as ObjectResult;

        // Assert
        Assert.IsNotNull(result, "Controlleren fangede ikke exceptionen. Husk try-catch!");
        Assert.AreEqual(500, result.StatusCode);
    }

}
