using System.IO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace ManageCategoriesApp
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryID { get; set; }

        [Required]
        [StringLength(40)]
        public string CategoryName { get; set; } = string.Empty;
    }

    public class MyStock : DbContext
    {
        public DbSet<Category> Categories { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string projectDir = System.IO.Directory.GetCurrentDirectory();
                string baseDir = System.AppDomain.CurrentDomain.BaseDirectory;
                
                var builder = new ConfigurationBuilder();
                
                // Determine the best base path
                if (System.IO.File.Exists(System.IO.Path.Combine(projectDir, "appsettings.json")))
                {
                    builder.SetBasePath(projectDir);
                }
                else if (System.IO.File.Exists(System.IO.Path.Combine(baseDir, "appsettings.json")))
                {
                    builder.SetBasePath(baseDir);
                }
                else
                {
                    // Fallback to project root directory
                    builder.SetBasePath(projectDir);
                }

                var configuration = builder
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();

                var connectionString = configuration.GetConnectionString("MyStockDB");
                if (string.IsNullOrEmpty(connectionString))
                {
                    connectionString = configuration.GetConnectionString("MyStore2");
                }

                if (string.IsNullOrEmpty(connectionString))
                {
                    // Look in parent directory (often useful during EF Core CLI executions from sub-folders)
                    var parentDir = System.IO.Directory.GetParent(projectDir)?.FullName;
                    if (parentDir != null && System.IO.File.Exists(System.IO.Path.Combine(parentDir, "appsettings.json")))
                    {
                        configuration = new ConfigurationBuilder()
                            .SetBasePath(parentDir)
                            .AddJsonFile("appsettings.json", optional: true)
                            .Build();
                        connectionString = configuration.GetConnectionString("MyStockDB");
                        if (string.IsNullOrEmpty(connectionString))
                        {
                            connectionString = configuration.GetConnectionString("MyStore2");
                        }
                    }
                }

                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new System.InvalidOperationException(
                        $"ConnectionString 'MyStockDB' or 'MyStore2' not found in appsettings.json. Search paths: CurrentDir={projectDir}, BaseDir={baseDir}");
                }

                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.CategoryID);
                entity.Property(e => e.CategoryName)
                    .IsRequired()
                    .HasMaxLength(40);

                // Seed data
                entity.HasData(
                    new Category { CategoryID = 1, CategoryName = "Beverages" },
                    new Category { CategoryID = 2, CategoryName = "Condiments" },
                    new Category { CategoryID = 3, CategoryName = "Confections" }
                );
            });
        }
    }
}
