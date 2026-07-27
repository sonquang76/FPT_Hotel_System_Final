using System;
using System.Collections.Generic;

namespace BussinessObjects;

public partial class Booking
{
    public int BookingId { get; set; }

    public int CustomerId { get; set; }

    public int RoomId { get; set; }

    public DateTime? CheckInDate { get; set; }

    public DateTime? CheckOutDate { get; set; }

    public DateTime ExpectedCheckIn { get; set; }

    public DateTime ExpectedCheckOut { get; set; }

    public string? BookingStatus { get; set; }

    public string CreatedBy { get; set; } = null!;

    public virtual Account CreatedByNavigation { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;

    public virtual Invoice? Invoice { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Room Room { get; set; } = null!;

    public virtual ICollection<Serviceorder> Serviceorders { get; set; } = new List<Serviceorder>();
}
