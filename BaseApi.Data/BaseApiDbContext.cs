using Microsoft.EntityFrameworkCore;
using BaseApi.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseApi.Data
{
    public class BaseApiDbContext : DbContext
    {
        
        public BaseApiDbContext(DbContextOptions<BaseApiDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Users>(entity => { entity.ToTable(name: "Users"); });
            builder.Entity<UserToken>(entity => { entity.ToTable(name: "UserToken"); });
            base.OnModelCreating(builder);
        }
        public DbSet<Users> Users { get; set; }
        public DbSet<UserToken> UserToken { get; set; }
     
    }
}
