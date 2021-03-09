using System;
using System.Collections.Generic;
using System.Text;

namespace BaseApi.Models
{
    public class TokenResponseDto
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
    }
}
