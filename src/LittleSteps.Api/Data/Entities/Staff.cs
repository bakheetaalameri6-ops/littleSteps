using System;
using System.Collections.Generic;

namespace LittleSteps.Api.Data.Entities;

public partial class Staff
{
    public int StaffId { get; set; }

    public string FullName { get; set; } = null!;

    public string Role { get; set; } = null!;

    public string Username { get; set; } = null!;

    public byte[] PasswordHash { get; set; } = null!;

    public byte[] PasswordSalt { get; set; } = null!;

    public bool MustChangePassword { get; set; }

    public string? Phone { get; set; }

    public byte[]? Photo { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();

    public virtual ICollection<AuditEntry> AuditEntries { get; set; } = new List<AuditEntry>();

    public virtual Class? Class { get; set; }

    public virtual ICollection<ClassTransfer> ClassTransfers { get; set; } = new List<ClassTransfer>();

    public virtual ICollection<EarlyExit> EarlyExits { get; set; } = new List<EarlyExit>();

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();

    public virtual ICollection<Visit> Visits { get; set; } = new List<Visit>();
}
