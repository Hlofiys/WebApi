using System.Reflection;

namespace WebApi.Dtos.User
{
    public class UserSetInfoDto : UserInfoDto
    {
        public static implicit operator Dictionary<string, string?>(UserSetInfoDto v)
        {
            return  v.GetType()
     .GetProperties(BindingFlags.Instance | BindingFlags.Public)
          .ToDictionary(prop => prop.Name, prop => (string)prop.GetValue(v, null));
        }
    }
}