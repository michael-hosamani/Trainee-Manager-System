using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TraineeManagementApi.Dto;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using TraineeManagementApi.Models;

namespace TraineeManagementApi.Services;

public class AuthService: IAuthService
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(AppDbContext db, IConfiguration configuration, ILogger<AuthService> logger)
    {   
        _db = db;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<LoginResponse?> UserLogin(LoginRequest loginRequest)
    {
        User? user = _db.Users.Where(u => u.Username == loginRequest.Username).FirstOrDefault();

        if (user == null)
        {
            _logger.LogError("User not found");
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

        var accessToken = this.GenerateToken(user, AuthTokenType.AccessToken);
        var refreshToken = this.GenerateToken(user, AuthTokenType.RefreshToken);

        user.RefreshToken = refreshToken.jwt;
        user.RefreshTokenExpiry = refreshToken.expiryDate;

        await _db.SaveChangesAsync(); 

        UserWithoutPassword userWithoutPassword = new UserWithoutPassword
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.Username,
            Role = user.Role,
            CreatedDate = user.CreatedDate,
            UpdatedDate = user.UpdatedDate,
            RefreshToken = user.RefreshToken,
            RefreshTokenExpiry = user.RefreshTokenExpiry
        };

        _logger.LogInformation("Logged In Successfully By user: {user} at {time}", user.Username, DateTime.UtcNow); 
        
        return new LoginResponse
        {
            User = userWithoutPassword,
            Token = accessToken.jwt,
            ExpiresIn = accessToken.expiryDate
        };
    }

    public async Task<LoginResponse?> Refresh(RefreshTokenDto refreshTokenDto)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]));

        var validationParameters = new TokenValidationParameters()
        {
            IssuerSigningKey = securityKey,
            ValidIssuer = _configuration["Jwt:Issuer"],
            ValidAudience = _configuration["Jwt:Audience"],
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ClockSkew = TimeSpan.Zero
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var result = tokenHandler.ValidateToken(refreshTokenDto.RefreshToken, validationParameters, out SecurityToken validatedToken);
        var jwtToken = (JwtSecurityToken)validatedToken;
  
        var userId = jwtToken.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;

        User? user =  _db.Users.Where(u => u.Id == int.Parse(userId)).FirstOrDefault();
        if(user == null)
        {
            _logger.LogError("Refresh token is invalid");
            return null;
        }

        if(refreshTokenDto.RefreshToken != user.RefreshToken)
        {
            _logger.LogError("Refresh token is invalid");
            return null;
        }

        if(user.RefreshTokenExpiry <= DateTime.UtcNow)
        {
            _logger.LogError("Refresh token has expired");
            return null;
        }

        var newAccessToken = GenerateToken(user, AuthTokenType.AccessToken);
        var newRefreshToken = GenerateToken(user, AuthTokenType.RefreshToken);
        user.RefreshToken = newRefreshToken.jwt;
        user.RefreshTokenExpiry = newRefreshToken.expiryDate;

        await _db.SaveChangesAsync(); 

        UserWithoutPassword userWithoutPassword = new UserWithoutPassword
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.Username,
            Role = user.Role,
            CreatedDate = user.CreatedDate,
            UpdatedDate = user.UpdatedDate,
            RefreshToken = user.RefreshToken,
            RefreshTokenExpiry = user.RefreshTokenExpiry
        };
        
        _logger.LogInformation("Refresh token and access token generated");
        return new LoginResponse
        {
            Token = newAccessToken.jwt,
            ExpiresIn = newAccessToken.expiryDate,
            User = userWithoutPassword
        };
    }

    public GenerateTokenResult GenerateToken(User user, AuthTokenType tokenType){
        Claim[] claims;
        if(tokenType == AuthTokenType.AccessToken){
            claims = 
            [
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),  
                new Claim(ClaimTypes.Name, user.Username), 
                new Claim(ClaimTypes.Role, user.Role.ToString())
            
            ];
        }
        else{
            claims = 
            [
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),  
            ];
        }
        

        var key = _configuration["Jwt:Key"];

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: tokenType == AuthTokenType.AccessToken ? DateTime.UtcNow.AddMinutes(10) : DateTime.UtcNow.AddDays(7),
            signingCredentials: credentials
        );

        var handler = new JwtSecurityTokenHandler();

        var jwt = handler.WriteToken(token);

        var expiryDate = handler.ReadJwtToken(jwt).ValidTo;

        return new GenerateTokenResult{
            jwt = jwt,
            expiryDate = expiryDate
        };
    }
}