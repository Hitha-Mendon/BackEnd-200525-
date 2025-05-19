using WisVestAPI.Repositories.Matrix;
using WisVestAPI.Services;
using WisVestAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;
using WisVestAPI.Configuration;

var builder = WebApplication.CreateBuilder(args);
// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console() // Logs to the console
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day) // Logs to a file
    .CreateLogger();

builder.Host.UseSerilog();
// Add services to the container.
builder.Services.AddControllers();


builder.Services.Configure<AppSettings>(
    builder.Configuration.GetSection("AppSettings"));
    builder.Services.AddScoped<ProductRepositoryService>();


    
builder.Services.AddSingleton<UserService>();
builder.Services.AddScoped<IUserInputService, UserInputService>();
builder.Services.AddScoped<IAllocationService, AllocationService>();
builder.Services.AddScoped<ProductAllocationService>();
builder.Services.AddScoped<InvestmentAmountService>();
builder.Services.AddScoped<MatrixRepository>(provider =>
    new MatrixRepository(provider.GetRequiredService<IConfiguration>()["MatrixFilePath"]));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// builder.Services.Configure<RequestLocalizationOptions>(options =>
// {
//     options.DefaultRequestCulture = new RequestCulture("en");
// });

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"])),
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"]
        };
    });
// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200") 
              .AllowAnyHeader() 
              .AllowAnyMethod(); 
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowAngular");

app.UseAuthorization();

app.MapControllers();

app.Run();