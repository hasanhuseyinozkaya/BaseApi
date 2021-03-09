using System;
using System.Collections.Generic;
using System.Text;

namespace BaseApi.Models
{
    public class Users : EntityBase<Guid>
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string MasterPassToken { get; set; }
        public string ApplicationToken { get; set; }
        public string Application { get; set; }
        public bool IsMacroMerchant { get; set; }

    }
}
