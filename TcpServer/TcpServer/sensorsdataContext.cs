using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace TcpServer
{
    public partial class sensorsdataContext : DbContext
    {
        public sensorsdataContext()
        {
        }

        public sensorsdataContext(DbContextOptions<sensorsdataContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Sensor> Sensors { get; set; } = null!;
        public virtual DbSet<SensorsDatum> SensorsData { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
            ///#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=sensorsdata;Username=postgres;Password=1120");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("uuid-ossp");

            modelBuilder.Entity<Sensor>(entity =>
            {
                entity.HasKey(e => e.SensorUuid)
                    .HasName("sensors_pkey");

                entity.ToTable("sensors");

                entity.Property(e => e.SensorUuid)
                    .ValueGeneratedNever()
                    .HasColumnName("sensor_uuid");

                entity.Property(e => e.NumberOfConnections).HasColumnName("number_of_connections");

                entity.Property(e => e.Username)
                    .HasMaxLength(28)
                    .HasColumnName("username");
                
            });

            modelBuilder.Entity<SensorsDatum>(entity =>
            {
                entity.HasKey(e => e.DataUuid)
                    .HasName("sensors_data_pkey");

                entity.ToTable("sensors_data");

                entity.Property(e => e.DataUuid)
                    .ValueGeneratedNever()
                    .HasColumnName("data_uuid");

                entity.Property(e => e.CountPoints).HasColumnName("count_points");

                entity.Property(e => e.EndTime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("end_time");

                entity.Property(e => e.ListPoints).HasColumnName("list_points");

                entity.Property(e => e.SensorUuid).HasColumnName("sensor_uuid");

                entity.Property(e => e.StartTime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("start_time");

            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
