using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TraineeManagementApi.Services;
using System.Text;
using System.ComponentModel;
using NSwag;
using NSwag.Generation.Processors.Security;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TraineeManagementApi.Models;
using Microsoft.AspNetCore.Diagnostics;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
   options.AddPolicy("AllowSpecificOrigin", 
      policy =>
      {
         policy.WithOrigins("http://localhost:5173")
               .AllowAnyHeader()
               .AllowAnyMethod();
      }
   );


});

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

   config.AddSecurity("JWT", new OpenApiSecurityScheme
   {
      Type = OpenApiSecuritySchemeType.ApiKey,
      Scheme = "bearer",
      In = OpenApiSecurityApiKeyLocation.Header,
      Name = "Authorization",
      Description = "Bearer {your JWT token}" 
   });

   config.OperationProcessors.Add(
      new AspNetCoreOperationSecurityScopeProcessor("JWT")
   );
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
 
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddScoped<ITraineeService, TraineeService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMentorService, MentorService>();
builder.Services.AddScoped<ILearningTaskService, LearningTaskService>();
builder.Services.AddScoped<ITaskAssignmentService, TaskAssignmentService>();
builder.Services.AddScoped<ISubmissionService, SubmissionService>();
builder.Services.AddScoped<IReviewService, ReviewService>();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            ClockSkew = TimeSpan.Zero,
            RoleClaimType = ClaimTypes.Role
        };
    });

builder.Services.AddAuthorization();

builder.Logging.AddConsole();     
builder.Logging.AddDebug();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

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
         RefreshToken = "",
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
app.UseCors("AllowSpecificOrigin");

app.MapGet("/", () => "Hello World!");

app.UseExceptionHandler();

app.UseOpenApi();
app.UseSwaggerUi();

// app.UseExceptionHandler(options =>
// {
//    options.Run(async context =>
//    {
//       context.Response.StatusCode = StatusCodes.Status500InternalServerError;
//       context.Response.ContentType = "application/json";

//       var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
//       if (exceptionFeature is not null)
//       {
//          var error = new { message = "An unexpected error occurred" };
//          await context.Response.WriteAsJsonAsync(error);
//       }
//    });
// });

app.Run();