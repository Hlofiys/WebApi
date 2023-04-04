using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; } = new byte[0];
        public byte[] PasswordSalt { get; set; } = new byte[0];
        public string RefreshToken {get; set;} = string.Empty;
        public DateTime RefreshTokenExpires {get; set;}
        public int? CartId {get; set;} = null;
        public string ActivationId { get; set;} = string.Empty;
        public bool IsActivated { get; set;} = false;
        public bool IsAdmin {get; set;} = false;

    }
}