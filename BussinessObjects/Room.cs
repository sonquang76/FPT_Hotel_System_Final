using System;
using System.Collections.Generic;

namespace BussinessObjects;

public partial class Room
{
    public int RoomId { get; set; }

    public string RoomNumber { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string? Description { get; set; }

    public int RoomTypeId { get; set; }

    public int Floor { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Roomtype RoomType { get; set; } = null!;
}
