using FlueFlame.AspNetCore;
using FlueFlame.Http.Host;
using FlueFlame.Serialization.Newtonsoft;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Stockband.Domain.Exceptions;
using Stockband.Infrastructure;

namespace Stockband.Api.E2E;

public abstract class BaseTest
{
    protected TestServer TestServer { get; set; }
    protected IFlueFlameHttpHost HttpHost { get; set; }
    protected IServiceProvider ServiceProvider { get; set; }

    protected StockbandDbContext Context => ServiceProvider.CreateScope()
        .ServiceProvider.GetRequiredService<StockbandDbContext>();

    [SetUp]
    protected async Task Setup()
    {
        WebApplicationFactory<Program> factory =
            new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.UseEnvironment("E2E");

                    builder.ConfigureServices(services =>
                    {
                        ServiceDescriptor? dbContextDescriptor = services.SingleOrDefault(
                            d => d.ServiceType ==
                                 typeof(DbContextOptions<StockbandDbContext>));
                        if (dbContextDescriptor == null)
                        {
                            throw new ObjectNotFound(typeof(ServiceDescriptor));
                        }

                        services.Remove(dbContextDescriptor);

                        string dbName = $"E2E_{Guid.NewGuid()}";

                        services.AddDbContext<StockbandDbContext>(x => x.UseInMemoryDatabase(dbName));
                    });
                });

        TestServer = factory.Server;
        ServiceProvider = factory.Services;
        HttpHost = FlueFlameAspNetBuilder.CreateDefaultBuilder(factory)
            .BuildHttpHost(builder =>
            {
                builder.ConfigureHttpClient(client =>
                {
                    
                });
                builder.UseNewtonsoftJsonSerializer();
                builder.Build();
            });
        
        await Context.Database.EnsureCreatedAsync();
    }
    
    [TearDown]
    protected async Task CleanUp()
    {
        await Context.Database.EnsureDeletedAsync();
        await Context.DisposeAsync();
        
        TestServer.Dispose();
    }

    //TODO: prepare this
    /*protected string GetJwtToken(int userId, string username, string email, UserRole userRole = UserRole.Admin)
    {
        JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        byte[] keyBytes = Encoding.UTF8.GetBytes("JzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6Ikpv");
        
        SecurityTokenDescriptor securityTokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new (ClaimTypes.NameIdentifier, userId.ToString()),
                new (ClaimTypes.Name, username),
                new (ClaimTypes.Email, email),
                new (ClaimTypes.Role, userRole.ToString())
            }),
            Expires = DateTime.Now.AddMinutes(2),
            Audience = "http://localhost:5000",
            Issuer = "localhost",
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
        };
        
        SecurityToken token = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
        string tokenString = jwtSecurityTokenHandler.WriteToken(token);
        
        return $"Bearer {tokenString}";
    }*/
}