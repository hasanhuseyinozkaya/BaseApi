using System;
using System.Collections.Generic;
using System.Text;

namespace BaseApi.Models
{
   public class UserToken
    {
        public Guid Id { get; set; }

        public string AccessTokenHash { get; set; }

        public DateTimeOffset AccessTokenExpiresDateTime { get; set; }

        public string RefreshTokenIdHash { get; set; }

        public string RefreshTokenIdHashSource { get; set; }

        public DateTimeOffset RefreshTokenExpiresDateTime { get; set; }


        public Guid UserId { get; set; }
        public virtual Users User { get; set; }
    }
}
