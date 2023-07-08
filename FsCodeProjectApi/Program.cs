using Application.Interface.Repository;
using Application.Interface.Services;
using Application.Validation;
using AspNetCoreRateLimit;
using Domain.Dtos;
using Domain.Entities;
using FluentValidation;
using FsCodeProjectApi.Middlewares;
using infrastructure.DAL;
using infrastructure.Interface.Repository;
using Infrastructure.DAL;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
using System.Text;
using Task.Rest.Api.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IReminderRepo, ReminderRepo>();
builder.Services.Configure<EmailConfiguration>(builder.Configuration.GetSection("EmailConfiguration"));
builder.Services.AddSingleton<ITelegramService>(new TelegramService(builder.Configuration["TelegramBotToken"]));
builder.Logging.ClearProviders();
builder.Services.AddMemoryCache();
builder.Services.Configure<Domain.Entities.RateLimitOptions>(builder.Configuration.GetSection("RateLimit"));

// Other service configurations...
builder.Logging.AddConsole();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Change to true in production
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "your_issuer",
        ValidAudience = "your_audience",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_secret_key"))
    };
});
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
builder.Services.AddSingleton<ExcelPackage>();
builder.Services.AddScoped<IValidator<CreateReminderDto>, ReminderValidator>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});
var app = builder.Build();
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
SeedData.Initialize(services);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseMiddleware<RateLimitMiddleware>();
app.UseAuthorization();
app.UseAuthorization();
app.UseMiddleware<ExceptionMiddleware>();
app.UseStaticFiles(); // Make sure this line is present
app.MapControllers();

app.Run();
