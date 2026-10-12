using System;
using System.Collections.Generic;

namespace LittleSteps.Api.Data.Entities;

public partial class EarlyExit
{
    public int EarlyExitId { get; set; }

    public int StudentId { get; set; }

    public string PresentedEmiratesId { get; set; } = null!;

    public int? GuardianId { get; set; }

    public bool Authorized { get; set; }

    public string? Reason { get; set; }

    public DateTime OccurredAt { get; set; }

    public int GuardStaffId { get; set; }

    public virtual Staff GuardStaff { get; set; } = null!;

    public virtual Guardian? Guardian { get; set; }

    public virtual Student Student { get; set; } = null!;
}
