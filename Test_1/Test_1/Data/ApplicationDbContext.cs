using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Test_1.Models;

namespace Test_1.Data
{
    public class ApplicationDbContext : DbContext
    {
        

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            
        }

        public DbSet<Contract> Contracts { get; set; }
        public DbSet<Stages> Stages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //  отношение "один ко многим" между Contract и Stages
            modelBuilder.Entity<Stages>()
                .HasOne(s => s.Contract)
                .WithMany(c => c.Stages)
                .HasForeignKey(s => s.ContractId);
        }
    }
}
