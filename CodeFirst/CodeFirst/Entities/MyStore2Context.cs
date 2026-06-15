using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ManageCategoriesApp.Entities;

public partial class MyStore2Context : DbContext
{
    public MyStore2Context()
    {
    }

    public MyStore2Context(DbContextOptions<MyStore2Context> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=localhost;Database=MyStore2;User Id=sa;Password=ngocanh1203;TrustServerCertificate=true;Trusted_Connection=SSPI;Encrypt=false;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
