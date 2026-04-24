using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DemoMvc.Models;

namespace DemoMvc.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<WeeklyActivity> WeeklyActivities { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<ReportDetail> ReportDetails { get; set; }
        public DbSet<ReportActivity> ReportActivities { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<InstructorJcfAssignment> InstructorJcfAssignments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // importante para Identity

            // 🔑 Relación Report → ReportDetail con cascada
            modelBuilder.Entity<ReportDetail>()
                .HasOne(rd => rd.Report)
                .WithMany(r => r.Details)
                .HasForeignKey(rd => rd.ReportId)
                .OnDelete(DeleteBehavior.Cascade);

            // 🔑 Clave compuesta para ReportActivity
            modelBuilder.Entity<ReportActivity>()
                .HasKey(ra => new { ra.ReportDetailId, ra.WeeklyActivityId });

            // 🔑 Relación ReportDetail → ReportActivity con cascada
            modelBuilder.Entity<ReportActivity>()
                .HasOne(ra => ra.ReportDetail)
                .WithMany(rd => rd.ReportActivities)
                .HasForeignKey(ra => ra.ReportDetailId)
                .OnDelete(DeleteBehavior.Cascade);

            // 🔑 Relación ReportActivity → WeeklyActivity (Restrict para no borrar catálogo)
            modelBuilder.Entity<ReportActivity>()
                .HasOne(ra => ra.WeeklyActivity)
                .WithMany(w => w.ReportActivities)
                .HasForeignKey(ra => ra.WeeklyActivityId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación opcional con usuario creador en WeeklyActivity
            modelBuilder.Entity<WeeklyActivity>()
                .HasOne(w => w.CreatedByUser)
                .WithMany()
                .HasForeignKey(w => w.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            // Configuración de InstructorJcfAssignment
            modelBuilder.Entity<InstructorJcfAssignment>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.Instructor)
                      .WithMany()
                      .HasForeignKey(e => e.InstructorUserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Jcf)
                      .WithMany()
                      .HasForeignKey(e => e.JcfUserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => new { e.InstructorUserId, e.JcfUserId });

                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.EffectiveStartDate).IsRequired();
                entity.Property(e => e.Status).HasMaxLength(20).IsRequired();
                entity.Property(e => e.Observaciones).HasMaxLength(500);
            });
        }
    }
}
