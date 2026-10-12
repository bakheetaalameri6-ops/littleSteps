using System;
using System.Collections.Generic;

namespace LittleSteps.Api.Data.Entities;

public partial class Guardian
{
    public int GuardianId { get; set; }

    public string EmiratesId { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public byte[]? Photo { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<EarlyExit> EarlyExits { get; set; } = new List<EarlyExit>();

    public virtual ICollection<StudentGuardian> StudentGuardians { get; set; } = new List<StudentGuardian>();
}
