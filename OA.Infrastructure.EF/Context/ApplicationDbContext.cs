﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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
        public virtual DbSet<Department> Department { get; set; } = null!;
        public virtual DbSet<Timekeeping> Timekeeping { get; set; } = null!;
        public virtual DbSet<Holiday> Holiday { get; set; } = null!;
        public virtual DbSet<Insurance> Insurance { get; set; } = null!;
        public virtual DbSet<InsuranceType> InsuranceTypes { get; set; }
        public virtual DbSet<TimeOff> TimeOff { get; set; } = null!;
        public virtual DbSet<EmploymentContract> EmploymentContract { get; set; }
        public virtual DbSet<Notifications> Notifications { get; set; } = null!;
        public virtual DbSet<UserNotifications> UserNotifications { get; set; } = null!;
        public virtual DbSet<NotificationFiles> NotificationFiles { get; set; } = null!;
        public virtual DbSet<NotificationDepartments> NotificationDepartments { get; set; } = null!;
        public virtual DbSet<NotificationRoles> NotificationRoles { get; set; } = null!;
        public virtual DbSet<Reward> Reward { get; set; }
        public virtual DbSet<Discipline> Discipline { get; set; }
        public virtual DbSet<WorkingRules> WorkingRules { get; set; }
        public virtual DbSet<WorkShifts> WorkShifts { get; set; }
        #endregion --DBSET--

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Insurance>()
            .HasOne(i => i.InsuranceType)
            .WithMany(it => it.Insurances)
            .HasForeignKey(i => i.InsuranceTypeId);

            modelBuilder.Entity<Benefit>()
                .HasOne(i => i.BenefitType)
                .WithMany(it => it.Benefits)
                .HasForeignKey(i => i.BenefitTypeId);
        }
    }
}
