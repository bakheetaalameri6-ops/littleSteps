using System;
using System.Collections.Generic;

namespace LittleSteps.Api.Data.Entities;

public partial class Session
{
    public int SessionId { get; set; }

    public int StaffId { get; set; }

    public string Token { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public virtual Staff Staff { get; set; } = null!;
}
