using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Data
{
    public interface IAuthRepository
    {
        Task<ServiceResponse<string>> Register(User user, string password, HttpResponse response);
        Task<ServiceResponse<string>> Login(string username, string password, HttpResponse response, HttpRequest httpRequest);
        Task<ServiceResponse<bool>> Delete(string username, string password);

        Task<bool> UserExists(string username);

        ServiceResponse<string> CheckToken(HttpRequest request);
        Task<ServiceResponse<string>> Refresh(HttpRequest request, HttpResponse response);
        Task<ServiceResponse<string>> Activate(string id, HttpResponse response, HttpRequest httpRequest);
        Task<ServiceResponse<string>> DeleteAll();
        Task<ServiceResponse<bool>> IsAdmin(string Token);
    }
}