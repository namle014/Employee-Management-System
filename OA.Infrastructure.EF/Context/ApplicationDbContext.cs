using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OA.Infrastructure.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA.Infrastructure.EF.Context
{
    public class ApplicationDbContext : IdentityDbContext<AspNetUser, AspNetRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
        {
        }

        #region --DBSET--
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<SysFile> SysFile { get; set; } = null!;
        public virtual DbSet<SysFunction> SysFunctions { get; set; } = null!;
        public virtual DbSet<SysApi> SysApis { get; set; } = null!;
        public virtual DbSet<SysConfiguration> SysConfigurations { get; set; } = null!;
        public virtual DbSet<Salary> Salary { get; set; } = null!;
        public virtual DbSet<Benefit> Benefit { get; set; } = null!;
        #endregion --DBSET--

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
