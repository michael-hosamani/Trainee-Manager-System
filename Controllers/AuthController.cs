using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TraineeManagementApi.Dto;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using TraineeManagementApi.Services;

namespace TraineeManagementApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController: ControllerBase
{
    private readonly IAuthService authService;
    public AuthController(IAuthService authService)
    {
        this.authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginRequest loginRequest)
    {
        LoginResponse? res = await authService.UserLogin(loginRequest);
        if(res == null)
        {
            return Unauthorized();
        }
        return Ok(res);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult> Refresh(RefreshTokenDto refreshTokenDto)
    {
        LoginResponse? res = await authService.Refresh(refreshTokenDto);
        if(res == null)
        {
            return Unauthorized();
        }
        return Ok(res);
    }
}