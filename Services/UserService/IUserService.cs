namespace WebApi.Services
{
    public interface IUserService
    {
        Task<ServiceResponse<string>> SetInfo(UserSetInfoDto setInfo, string token);
        Task<ServiceResponse<UserGetInfoDto>> GetInfo(string token);
    }
}