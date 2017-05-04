﻿using KidSports.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KidSports.Repositories
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Sport> Sports { get; set; }
        public DbSet<School> Schools { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<PreviousGradesCoached> PreviousGradesCoached { get; set; }
        public DbSet<PreviousYearsCoached> PreviousYearsCoached{ get; set; }
        public DbSet<ApplicationStatus> ApplicationStatus{ get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}