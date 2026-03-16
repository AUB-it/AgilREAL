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
        mock.Setup(r => r.Register(It.IsAny<User>()))
            .Returns(false);
        var controller = new UsersController(mock.Object);
        
        // Act
        var result = controller.RegisterUser(new User()) as  BadRequestResult;
        
        // Assert
        Assert.IsInstanceOfType<BadRequestResult>(result);
        mock.Verify(r => r.Register(It.IsAny<User>()), Times.Once);
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
        mock.Setup(r => r.Register(It.Is<User>(u => u != null && string.IsNullOrEmpty(u.Name))))
            .Returns(false);

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
        mock.Verify(r => r.Register(It.Is<User>(u => u == newUser)), Times.Once);
    }
}