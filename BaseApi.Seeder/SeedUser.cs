using BaseApi.Data;
using BaseApi.Models;
using BaseApi.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace BaseApi.Seeder
{
    public static partial class Seed
    {
        public static void SeedUser(BaseApiDbContext dbContext)
        {

            var normalUser = new Users()
            {
                Name = "Hasan",
                Surname = "Ozkaya",
                Email = "xx@.xcom",
                Password = CryptoHelpers.GetMd5Hash("mypass1"),
                Application = "Mydomain",
                IsMacroMerchant = true,
                PhoneNumber = "1111111111",

            };

            var iosUser = new Users()
            {
                Name = "ios",
                Surname = "X",
                Email = "xxx@x.com",
                Password = CryptoHelpers.GetMd5Hash("mypass2"),
                Application = "Mydomain",
                IsMacroMerchant = true,
                PhoneNumber = "1111111112",

            };


            var droid = new Users()
            {
                Name = "droid",
                Surname = "X",
                Email = "xxx@x.com",
                Password = CryptoHelpers.GetMd5Hash("mypass3"),
                Application = "Mydomain",
                IsMacroMerchant = true,
                PhoneNumber = "1111111113",

            };


            List<Users> userList = new List<Users>();
            userList.Add(normalUser);
            userList.Add(iosUser);
            userList.Add(droid);


            foreach (var item in userList)
            {
                var isUserExist = dbContext.Users.Any(t => t.Email == item.Email);
                if (!isUserExist)
                {

                    dbContext.Users.Add(item);
                    dbContext.SaveChanges();
                }
            }




        }
    }
}
