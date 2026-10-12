using System;
using System.Collections.Generic;

namespace LittleSteps.Api.Data.Entities;

public partial class Room
{
    public int RoomId { get; set; }

    public string Name { get; set; } = null!;

    public virtual Class? Class { get; set; }
}
