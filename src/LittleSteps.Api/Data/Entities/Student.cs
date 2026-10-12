using System;
using System.Collections.Generic;

namespace LittleSteps.Api.Data.Entities;

public partial class Student
{
    public int StudentId { get; set; }

    public short EnrolYear { get; set; }

    public string? StudentCode { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateOnly DateOfBirth { get; set; }

    public char Gender { get; set; }

    public int ClassId { get; set; }

    public byte[]? Photo { get; set; }

    public bool? PhotoMissing { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();

    public virtual Class Class { get; set; } = null!;

    public virtual ICollection<ClassTransfer> ClassTransfers { get; set; } = new List<ClassTransfer>();

    public virtual ICollection<EarlyExit> EarlyExits { get; set; } = new List<EarlyExit>();

    public virtual ICollection<StudentGuardian> StudentGuardians { get; set; } = new List<StudentGuardian>();
}
