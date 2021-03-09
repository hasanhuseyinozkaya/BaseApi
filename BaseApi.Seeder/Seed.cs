using BaseApi.Data;
using BaseApi.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace BaseApi.Seeder
{
    public static partial class Seed
    {
        public static void SeedData(BaseApiDbContext dbContext)
        {

            SeedUser(dbContext);

        }
    }
}
