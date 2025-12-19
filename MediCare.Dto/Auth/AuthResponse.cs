using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCareDto.Auth
{
    public class AuthResponse
    {
        public string Token { get; set; }
        public string Status { get; set; }
        public long UserId { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
    }
}
