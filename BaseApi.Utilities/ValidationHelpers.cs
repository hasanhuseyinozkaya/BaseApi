using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace BaseApi.Utilities
{
   public class ValidationHelpers
    {
        public static bool ValidateEmail(string email)
        {
            string emailRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                                       @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                                          @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            Regex re = new Regex(emailRegex);

            return re.IsMatch(email);
        }

        public static bool ValidateUserName(string userName)
        {
            string userRegex = @"^[a-zA-Z0-9]*$";
            Regex re = new Regex(userRegex);

            return re.IsMatch(userName);
        }


    }
}
