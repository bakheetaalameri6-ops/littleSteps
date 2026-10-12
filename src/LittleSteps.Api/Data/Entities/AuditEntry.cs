using System;
using System.Collections.Generic;

namespace LittleSteps.Api.Data.Entities;

public partial class AuditEntry
{
    public long AuditId { get; set; }

    public DateTime OccurredAt { get; set; }

    public int? StaffId { get; set; }

    public string? Username { get; set; }

    public string Area { get; set; } = null!;

    public string Action { get; set; } = null!;

    public string? Details { get; set; }

    public string Result { get; set; } = null!;

    public string? EntityType { get; set; }

    public int? EntityId { get; set; }

    public virtual Staff? Staff { get; set; }
}
