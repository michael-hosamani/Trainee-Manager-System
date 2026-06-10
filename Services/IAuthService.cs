using TraineeManagementApi.Dto;

namespace TraineeManagementApi.Services;

public interface IAuthService
{
    LoginResponse? UserLogin(LoginRequest loginRequest);
}