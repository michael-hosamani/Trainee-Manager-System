using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TraineeManagementApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
   .AddJsonOptions(options =>
   {
      options.JsonSerializerOptions.Converters.Add(
         new JsonStringEnumConverter() 
      );
   });

builder.Services.AddOpenApiDocument(config =>
{
   config.DocumentName = "v1";
   config.Title = "Training Management api"; 
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
 
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

var jwtSecret = builder.Configuration["Jwt:Key"];

builder.Services.AddScoped<ITraineeService, TraineeService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// builder.Services
//     .AddAuthentication(options =>
//     {
//         options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//         options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//     })
//     .AddJwtBearer(options =>
//     {
//         // Keep the claim names exactly as they appear in the token (no surprise remapping).
//         options.MapInboundClaims = false;
//         options.TokenValidationParameters = new TokenValidationParameters
//         {
//             ValidateIssuer = true,
//             ValidateAudience = true,
//             ValidateLifetime = true,
//             ValidateIssuerSigningKey = true,
//             ValidIssuer = jwtSettings.Issuer,
//             ValidAudience = jwtSettings.Audience,
//             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
//             ClockSkew = TimeSpan.Zero,
//             NameClaimType = JwtRegisteredClaimNames.Name,
//             RoleClaimType = "role"
//         };
//     });

// builder.Services.AddAuthorization();
// builder.Services.AddScoped<JwtService>();

var app = builder.Build();

// This seeds the database by creating one user in the User table
using(var scope = app.Services.CreateAsyncScope()) 
{
   var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

   if (!db.Users.Any())
   {
      var admin = new User
      {
         Username = "michael",
         Email = "michael@gmail.com",
         PasswordHash = "",
         Role = Role.Admin
      };
      var hasher = new PasswordHasher<User>();
      string hashedPassword = hasher.HashPassword(admin, "pass");
      admin.PasswordHash = hashedPassword;
      Console.WriteLine("Seeding user: " + admin);
      db.Users.Add(admin);
      db.SaveChanges();
   }
}

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();

app.MapGet("/", () => "Hello World!");


app.UseOpenApi();
app.UseSwaggerUi();


app.Run();