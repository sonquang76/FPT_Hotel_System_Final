using System;
using System.Collections.Generic;

namespace BussinessObjects;

public partial class Serviceorder
{
    public int ServiceOrderId { get; set; }

    public int BookingId { get; set; }

    public int ServiceId { get; set; }

    public int Quantity { get; set; }

    public DateTime? OrderTime { get; set; }

    public string OrderStatus { get; set; } = null!;

    public decimal Price { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual Service Service { get; set; } = null!;
}
