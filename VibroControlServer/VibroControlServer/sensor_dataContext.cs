using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using VibroControlServer.Models;

namespace VibroControlServer
{
    public partial class sensor_dataContext : DbContext
    {
        public sensor_dataContext()
        {
            foreach (var item in Connections)
            {
                item.UpdList();
            }
        }

        public sensor_dataContext(DbContextOptions<sensor_dataContext> options)
            : base(options)
        {
            foreach (var item in Connections)
            {
                item.UpdList();
            }
        }

        public virtual DbSet<Connection> Connections { get; set; } = null!;
        public virtual DbSet<Sensor> Sensors { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=sensor_data;Username=postgres;Password=1120");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Connection>(entity =>
            {
                entity.HasKey(e => e.Uuid)
                    .HasName("connection_pkey");

                entity.ToTable("connection");

                entity.Property(e => e.Uuid)
                    .ValueGeneratedNever()
                    .HasColumnName("uuid");

                entity.Property(e => e.CountPoints).HasColumnName("count_points");

                entity.Property(e => e.EndTime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("end_time");

                entity.Property(e => e.SensorUuid).HasColumnName("sensor_uuid");

                entity.Property(e => e.StartTime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("start_time");

                entity.Property(e => e.VibrationData)
                    .HasColumnType("jsonb")
                    .HasColumnName("vibration_data");
            });

            modelBuilder.Entity<Sensor>(entity =>
            {
                entity.HasKey(e => e.Uuid)
                    .HasName("sensors_pkey");

                entity.ToTable("sensors");

                entity.Property(e => e.Uuid)
                    .ValueGeneratedNever()
                    .HasColumnName("uuid");

                entity.Property(e => e.NumberOfConnections).HasColumnName("number_of_connections");

                entity.Property(e => e.UserName).HasColumnName("user_name");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
