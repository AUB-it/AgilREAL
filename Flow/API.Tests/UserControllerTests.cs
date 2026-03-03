using API.Controllers;
using API.Models;
using API.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;

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
}