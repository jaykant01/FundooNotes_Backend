using FundooNotes.BusinessLogicLayer.Interfaces;
using FundooNotes.BusinessLogicLayer.Services;
using FundooNotes.DataLogicLayer.Context;
using FundooNotes.DataLogicLayer.Interfaces;
using FundooNotes.DataLogicLayer.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowAngular", policy =>
  {
    policy.WithOrigins("http://localhost:4200")  // Angular dev server
          .AllowAnyHeader()
          .AllowAnyMethod();
  });
});

Log.Logger = new LoggerConfiguration()
  .MinimumLevel.Information()
  .WriteTo.Console()
  .WriteTo.File(
  "Logs/fundoonotes_log.txt",
  rollingInterval: RollingInterval.Day)
  .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.

// Database Context
builder.Services.AddDbContext<FundooContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("FundooDb")));

//Add Repository
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<INotesRepository, NotesRepository>();

//Add Service
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<INotesService, NotesService>();

builder.Services.AddControllers()
  .AddJsonOptions(options =>
  {
    // camelCase so Angular interface names match
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
  });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
  // API Info
  options.SwaggerDoc("v1", new OpenApiInfo
  {
    Title = "FundooNotes API",
    Version = "v1",
    Description = "FundooNotes REST API with JWT Authentication"
  });

  // JWT Security Definition
  options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
  {
    Name = "Authorization",
    Type = SecuritySchemeType.ApiKey,
    Scheme = "Bearer",
    BearerFormat = "JWT",
    In = ParameterLocation.Header,
    Description = "Enter your JWT token like this: Bearer {your token}"
  });

  // JWT Security Requirement
  options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// JWT Authentication
builder.Services.AddAuthentication(options =>
{
  options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
  options.TokenValidationParameters = new TokenValidationParameters
  {
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = builder.Configuration["Jwt:Issuer"],
    ValidAudience = builder.Configuration["Jwt:Audience"],
    IssuerSigningKey = new SymmetricSecurityKey(
          Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
  };
});

// Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
  options.Configuration = builder.Configuration["Redis:ConnectionString"];
  options.InstanceName = "FundooNotes_";
});

var app = builder.Build();
app.UseCors("AllowAngular");
app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
