using System;
using System.Collections.Generic;

namespace LittleSteps.Api.Data.Entities;

public partial class Visit
{
    public int VisitId { get; set; }

    public string VisitorName { get; set; } = null!;

    public string VisitorEmiratesId { get; set; } = null!;

    public string Reason { get; set; } = null!;

    public string PersonVisited { get; set; } = null!;

    public DateTime TimeIn { get; set; }

    public DateTime? TimeOut { get; set; }

    public int RegisteredByStaffId { get; set; }

    public virtual Staff RegisteredByStaff { get; set; } = null!;
}
