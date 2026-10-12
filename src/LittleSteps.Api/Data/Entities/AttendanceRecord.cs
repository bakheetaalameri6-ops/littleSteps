using System;
using System.Collections.Generic;

namespace LittleSteps.Api.Data.Entities;

public partial class AttendanceRecord
{
    public int AttendanceId { get; set; }

    public int StudentId { get; set; }

    public int ClassId { get; set; }

    public DateOnly AttendanceDate { get; set; }

    public char Status { get; set; }

    public int MarkedByStaffId { get; set; }

    public DateTime MarkedAt { get; set; }

    public DateTime? CorrectedAt { get; set; }

    public virtual Class Class { get; set; } = null!;

    public virtual Staff MarkedByStaff { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
