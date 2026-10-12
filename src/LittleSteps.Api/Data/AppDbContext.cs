using System;
using System.Collections.Generic;
using LittleSteps.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LittleSteps.Api.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AttendanceRecord> AttendanceRecords { get; set; }

    public virtual DbSet<AuditEntry> AuditEntries { get; set; }

    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<ClassTransfer> ClassTransfers { get; set; }

    public virtual DbSet<EarlyExit> EarlyExits { get; set; }

    public virtual DbSet<Guardian> Guardians { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<Session> Sessions { get; set; }

    public virtual DbSet<Staff> Staff { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<StudentGuardian> StudentGuardians { get; set; }

    public virtual DbSet<Visit> Visits { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AttendanceRecord>(entity =>
        {
            entity.HasKey(e => e.AttendanceId).HasName("pk_attendance");

            entity.HasIndex(e => new { e.ClassId, e.AttendanceDate }, "ix_att_classday");

            entity.HasIndex(e => new { e.StudentId, e.AttendanceDate }, "uq_att_studentday").IsUnique();

            entity.Property(e => e.MarkedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasMaxLength(1);

            entity.HasOne(d => d.Class).WithMany(p => p.AttendanceRecords)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_att_classes");

            entity.HasOne(d => d.MarkedByStaff).WithMany(p => p.AttendanceRecords)
                .HasForeignKey(d => d.MarkedByStaffId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_att_staff");

            entity.HasOne(d => d.Student).WithMany(p => p.AttendanceRecords)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_att_students");
        });

        modelBuilder.Entity<AuditEntry>(entity =>
        {
            entity.HasKey(e => e.AuditId).HasName("pk_auditentries");

            entity.HasIndex(e => e.OccurredAt, "ix_audit_occurredat").IsDescending();

            entity.Property(e => e.Action).HasMaxLength(40);
            entity.Property(e => e.Area).HasMaxLength(20);
            entity.Property(e => e.Details).HasMaxLength(400);
            entity.Property(e => e.EntityType).HasMaxLength(30);
            entity.Property(e => e.OccurredAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Result).HasMaxLength(12);
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.Staff).WithMany(p => p.AuditEntries)
                .HasForeignKey(d => d.StaffId)
                .HasConstraintName("fk_audit_staff");
        });

        modelBuilder.Entity<Class>(entity =>
        {
            entity.HasKey(e => e.ClassId).HasName("pk_classes");

            entity.HasIndex(e => e.Name, "uq_classes_name").IsUnique();

            entity.HasIndex(e => e.RoomId, "uq_classes_room").IsUnique();

            entity.HasIndex(e => e.TeacherId, "ux_classes_teacher")
                .IsUnique()
                .HasFilter("(\"TeacherId\" IS NOT NULL)");

            entity.Property(e => e.Name).HasMaxLength(20);

            entity.HasOne(d => d.Room).WithOne(p => p.Class)
                .HasForeignKey<Class>(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_classes_rooms");

            entity.HasOne(d => d.Teacher).WithOne(p => p.Class)
                .HasForeignKey<Class>(d => d.TeacherId)
                .HasConstraintName("fk_classes_teacher");
        });

        modelBuilder.Entity<ClassTransfer>(entity =>
        {
            entity.HasKey(e => e.TransferId).HasName("pk_classtransfers");

            entity.HasIndex(e => new { e.StudentId, e.EffectiveDate }, "ix_ct_student");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Reason).HasMaxLength(200);

            entity.HasOne(d => d.ByStaff).WithMany(p => p.ClassTransfers)
                .HasForeignKey(d => d.ByStaffId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ct_staff");

            entity.HasOne(d => d.FromClass).WithMany(p => p.ClassTransferFromClasses)
                .HasForeignKey(d => d.FromClassId)
                .HasConstraintName("fk_ct_from");

            entity.HasOne(d => d.Student).WithMany(p => p.ClassTransfers)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ct_students");

            entity.HasOne(d => d.ToClass).WithMany(p => p.ClassTransferToClasses)
                .HasForeignKey(d => d.ToClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ct_to");
        });

        modelBuilder.Entity<EarlyExit>(entity =>
        {
            entity.HasKey(e => e.EarlyExitId).HasName("pk_earlyexits");

            entity.HasIndex(e => new { e.StudentId, e.OccurredAt }, "ix_ee_student");

            entity.Property(e => e.OccurredAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.PresentedEmiratesId)
                .HasMaxLength(18)
                .IsFixedLength();
            entity.Property(e => e.Reason).HasMaxLength(30);

            entity.HasOne(d => d.GuardStaff).WithMany(p => p.EarlyExits)
                .HasForeignKey(d => d.GuardStaffId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ee_staff");

            entity.HasOne(d => d.Guardian).WithMany(p => p.EarlyExits)
                .HasForeignKey(d => d.GuardianId)
                .HasConstraintName("fk_ee_guardians");

            entity.HasOne(d => d.Student).WithMany(p => p.EarlyExits)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ee_students");
        });

        modelBuilder.Entity<Guardian>(entity =>
        {
            entity.HasKey(e => e.GuardianId).HasName("pk_guardians");

            entity.HasIndex(e => e.EmiratesId, "uq_guardians_emiratesid").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.EmiratesId)
                .HasMaxLength(18)
                .IsFixedLength();
            entity.Property(e => e.FullName).HasMaxLength(120);
            entity.Property(e => e.Phone).HasMaxLength(20);
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.RoomId).HasName("pk_rooms");

            entity.HasIndex(e => e.Name, "uq_rooms_name").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(40);
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasKey(e => e.SessionId).HasName("pk_sessions");

            entity.HasIndex(e => e.Token, "uq_sessions_token").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Token)
                .HasMaxLength(64)
                .IsFixedLength();

            entity.HasOne(d => d.Staff).WithMany(p => p.Sessions)
                .HasForeignKey(d => d.StaffId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_sessions_staff");
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.HasKey(e => e.StaffId).HasName("pk_staff");

            entity.HasIndex(e => e.Username, "uq_staff_username").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.FullName).HasMaxLength(120);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.MustChangePassword).HasDefaultValue(true);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Role).HasMaxLength(20);
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("pk_students");

            entity.HasIndex(e => e.ClassId, "ix_students_class");

            entity.HasIndex(e => new { e.LastName, e.FirstName }, "ix_students_name");

            entity.HasIndex(e => e.StudentCode, "ux_students_code").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.EnrolYear).HasDefaultValueSql("EXTRACT(year FROM CURRENT_TIMESTAMP)");
            entity.Property(e => e.FirstName).HasMaxLength(60);
            entity.Property(e => e.Gender).HasMaxLength(1);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastName).HasMaxLength(60);
            entity.Property(e => e.PhotoMissing).HasComputedColumnSql("(\"Photo\" IS NULL)", true);
            entity.Property(e => e.StudentCode).HasComputedColumnSql("((('LS-'::text || (\"EnrolYear\")::text) || '-'::text) || lpad((\"StudentId\")::text, GREATEST(4, length((\"StudentId\")::text)), '0'::text))", true);

            entity.HasOne(d => d.Class).WithMany(p => p.Students)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_students_classes");
        });

        modelBuilder.Entity<StudentGuardian>(entity =>
        {
            entity.HasKey(e => new { e.StudentId, e.GuardianId }).HasName("pk_studentguardians");

            entity.Property(e => e.Relationship).HasMaxLength(20);

            entity.HasOne(d => d.Guardian).WithMany(p => p.StudentGuardians)
                .HasForeignKey(d => d.GuardianId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_sg_guardians");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentGuardians)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_sg_students");
        });

        modelBuilder.Entity<Visit>(entity =>
        {
            entity.HasKey(e => e.VisitId).HasName("pk_visits");

            entity.HasIndex(e => e.TimeIn, "ix_visits_timein");

            entity.Property(e => e.PersonVisited).HasMaxLength(120);
            entity.Property(e => e.Reason).HasMaxLength(30);
            entity.Property(e => e.TimeIn).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.VisitorEmiratesId)
                .HasMaxLength(18)
                .IsFixedLength();
            entity.Property(e => e.VisitorName).HasMaxLength(120);

            entity.HasOne(d => d.RegisteredByStaff).WithMany(p => p.Visits)
                .HasForeignKey(d => d.RegisteredByStaffId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_visits_staff");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
