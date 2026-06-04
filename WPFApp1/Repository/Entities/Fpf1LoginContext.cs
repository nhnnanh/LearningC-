using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Repository.Entities;

public partial class Fpf1LoginContext : DbContext
{
    public Fpf1LoginContext()
    {
    }

    public Fpf1LoginContext(DbContextOptions<Fpf1LoginContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetConnectionString("DBDefault");
        optionsBuilder.UseSqlServer(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Account__3214EC072FF7B7BC");

            entity.ToTable("Account");

            entity.HasIndex(e => e.Username, "UQ__Account__536C85E426FEB891").IsUnique();

            entity.Property(e => e.Password).HasMaxLength(100);
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
