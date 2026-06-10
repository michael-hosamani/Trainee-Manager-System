using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace TraineeManagementApi.Dto;

public class LoginResponse
{
    public required string Token { get; set; }
    public required DateTime ExpiresIn { get; set; }
    public required User User { get; set; }
}