using System;
using System.Collections.Generic;

namespace LittleSteps.Api.Data.Entities;

public partial class StudentGuardian
{
    public int StudentId { get; set; }

    public int GuardianId { get; set; }

    public string Relationship { get; set; } = null!;

    public virtual Guardian Guardian { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
