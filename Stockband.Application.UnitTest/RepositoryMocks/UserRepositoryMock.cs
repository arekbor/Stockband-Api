using Stockband.Application.Interfaces.Repositories;
using Moq;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;

namespace Stockband.Application.UnitTest.RepositoryMocks;

public class UserRepositoryMock
{
    public static Mock<IUserRepository> GetUserRepositoryMock()
    {
        List<User> userMocks = GetUserMocks();

        Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();
        
        userRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(userMocks);

        userRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))!.ReturnsAsync((int id) =>
        {
            User? user = userMocks
                .FirstOrDefault(x => x.Id == id);
            return user;
        });

        userRepositoryMock.Setup(r => r.AddAsync(It.IsAny<User>())).Callback<User>((user) =>
        {
            userMocks.Add(user);
        });

        userRepositoryMock.Setup(r => r.DeleteAsync(It.IsAny<User>())).Callback<User>((user) =>
        {
            userMocks.Remove(user);
        });
        
        userRepositoryMock.Setup(r => r.GetUserByEmailAsync(It.IsAny<string>())).ReturnsAsync((string email) =>
        {
            User? user = userMocks
                .FirstOrDefault(x => x.Email == email);
            return user;
        });
        
        userRepositoryMock.Setup(r => r.GetUserByUsernameAsync(It.IsAny<string>())).ReturnsAsync((string username) =>
        {
            User? user = userMocks
                .FirstOrDefault(x => x.Username == username);
            return user;
        });

        return userRepositoryMock;
    }
    public static List<User> GetUserMocks()
    {
        return new List<User>
        {
            new()
            {
                Id = 1,
                Username = "Mock1",
                //usermocktestpassword1
                Password = "$2a$11$pa85B4C/XH7u/BLL5h8XTefdG29vkcsZKBoHSOYHjsBP.Svmf6pti",
                Email = "mock1@mock.com",
                Role = UserRole.User,
            },
            new()
            {
                Id = 2,
                Username = "Mock2",
                //usermocktestpassword2
                Password = "$2a$11$o.e5SfIYGqby.NVyq/6vrONg76hL6yo2R2DZkuH1esekWcd6ihQnO",
                Email = "mock2@mock.com",
                Role = UserRole.User,
            },
            new()
            {
                Id = 3,
                Username = "Mock3",
                //usermocktestpassword3
                Password = "$2a$11$xLF4MuqyTWQp7wZhHX0KMe2ghha.lgEjfJpFd7ujdsaUZnnBPlSnG",
                Email = "mock3@mock.com",
                Role = UserRole.User,
            },
            new()
            {
                Id = 4,
                Username = "Mock4",
                //usermocktestpassword3
                Password = "$2a$11$xLF4MuqyTWQp7wZhHX0KMe2ghha.lgEjfJpFd7ujdsaUZnnBPlSnG",
                Email = "mock4@mock.com",
                Role = UserRole.User,
            },
            new()
            {
                Id = 5,
                Username = "Mock5",
                //usermocktestpassword3
                Password = "$2a$11$xLF4MuqyTWQp7wZhHX0KMe2ghha.lgEjfJpFd7ujdsaUZnnBPlSnG",
                Email = "mock4@mock.com",
                Role = UserRole.Admin,
            }
        };
    }
    
}