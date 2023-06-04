using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Stockband.Api.Interfaces;
using Stockband.Api.Services;
using Stockband.Application;
using Stockband.Domain.Exceptions;
using Stockband.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactClientPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
            .AllowCredentials()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtKey"]!)),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JwtIssuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JwtAudience"],
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = ctx =>
        {
            string? cookieName = builder.Configuration["CookieName"];
            if (cookieName == null)
            {
                throw new ObjectNotFound(nameof(cookieName));
            }

            ctx.Token = ctx.Request.Cookies[cookieName];
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = ctx =>
        {
            Console.WriteLine(ctx.Exception.ToString());
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddScoped<IAuthorizationUser, AuthorizationUser>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructureDbContexts(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);

var app = builder.Build();

app.UseStaticFiles();

app.UseCors("ReactClientPolicy");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Stockband API");
        c.InjectStylesheet("/swagger-ui/style.css");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();