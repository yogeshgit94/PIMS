
using Microsoft.EntityFrameworkCore;
using Entities;
using ServiceContracts;
using Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Filters;
using PIMS.Filters;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Exceptions;
using PIMS.Middleware;

var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).Enrich.WithExceptionDetails().CreateLogger();
Log.Logger.Information("Logging is working fine");

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
builder.Services.AddScoped<CustomExceptionFilter>();
builder.Services.AddControllers(options =>
{
    options.Filters.AddService<CustomExceptionFilter>();
})
.AddJsonOptions(options =>
{    
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    options.JsonSerializerOptions.MaxDepth = 64; 
});

// Add JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
    };
    options.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {            
            context.HandleResponse();

            var result = JsonSerializer.Serialize(new { status = 401, message = "Unauthorized access." });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 401;
            return context.Response.WriteAsync(result);
        },
        OnForbidden = context =>
        {
            var result = JsonSerializer.Serialize(new { status = 403, message = "Forbidden. You do not have permission to access this resource." });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 403;
            return context.Response.WriteAsync(result);
        }
    };
});

// Role-Based Authorization Setup
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Administrator"));  // Administrator role
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));  // User role
    options.AddPolicy("UserOrAdmin", policy => policy.RequireRole("User", "Administrator"));//User and Administrator role
});

builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0); // Set default version to 1.0
    options.ReportApiVersions = true; 
});

// Add Swagger configuration for JWT authentication
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "PIMS API", Version = "v1" });


    // Add XML comments from the API project
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    // Check if the XML file exists
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
    else
    {
        // Optionally log or handle the missing file scenario
        Log.Warning("XML comments file not found: {XmlPath}", xmlPath);
    }

    // Add JWT Bearer token definition in Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter 'Bearer' followed by a space and the JWT token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // Add the requirement that all requests use this security scheme
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
            Array.Empty<string>()
        }
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add services to the container
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IUserService, UserService>();
var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.MapControllers();
app.Run();
