using System;
using System.Collections.Generic;

namespace LittleSteps.Api.Data.Entities;

public partial class ClassTransfer
{
    public int TransferId { get; set; }

    public int StudentId { get; set; }

    public int? FromClassId { get; set; }

    public int ToClassId { get; set; }

    public DateOnly EffectiveDate { get; set; }

    public string Reason { get; set; } = null!;

    public int ByStaffId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Staff ByStaff { get; set; } = null!;

    public virtual Class? FromClass { get; set; }

    public virtual Student Student { get; set; } = null!;

    public virtual Class ToClass { get; set; } = null!;
}
