using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TraineeManagementApi.Dto;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace TraineeManagementApi.Services;

public class AuthService: IAuthService
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _configuration;

    public AuthService(AppDbContext db, IConfiguration configuration)
    {   
        _db = db;
        _configuration = configuration;
    }

    public LoginResponse? UserLogin(LoginRequest loginRequest)
    {
        var user = _db.Users.Where(u => u.Username == loginRequest.Username).FirstOrDefault();

        if (user == null)
        {
            return null;
        }
  
        var hasher = new PasswordHasher<User>();
        var isCorrectPassword = hasher.VerifyHashedPassword(user, user.PasswordHash, loginRequest.Password);
        if(isCorrectPassword == PasswordVerificationResult.Failed)
        {
            return null;
        }
        
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),  
            new Claim(ClaimTypes.Name, user.Username), 
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var key = _configuration["Jwt:Key"];
        if(key == null)
        {
            return null;
        }

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "TraineeManagementApi",
            audience: "TraineeManagementClient",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        // var token = _jwtService.GenerateToken(checkUser.Username);
        // return Ok(new {Token = token });
        var handler = new JwtSecurityTokenHandler();

        // Read the token without validating signature
        var expiryDate = handler.ReadJwtToken(jwt).ValidTo;
        Console.WriteLine("type of token: " + token.GetType());
        Console.WriteLine("token: " + expiryDate);
        return new LoginResponse
        {
            User = user,
            Token = jwt,
            ExpiresIn = expiryDate
        };
    }
}