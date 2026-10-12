using System;
using System.Collections.Generic;

namespace LittleSteps.Api.Data.Entities;

public partial class Class
{
    public int ClassId { get; set; }

    public string Name { get; set; } = null!;

    public int RoomId { get; set; }

    public int? TeacherId { get; set; }

    public int Capacity { get; set; }

    public virtual ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();

    public virtual ICollection<ClassTransfer> ClassTransferFromClasses { get; set; } = new List<ClassTransfer>();

    public virtual ICollection<ClassTransfer> ClassTransferToClasses { get; set; } = new List<ClassTransfer>();

    public virtual Room Room { get; set; } = null!;

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    public virtual Staff? Teacher { get; set; }
}
