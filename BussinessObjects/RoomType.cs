using System;
using System.Collections.Generic;

namespace BussinessObjects;

public partial class Roomtype
{
    public int RoomTypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public decimal BasePrice { get; set; }

    public int Capacity { get; set; }

    public string? Description { get; set; }

    public string? Url { get; set; }

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}
