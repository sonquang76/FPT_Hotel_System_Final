using System;
using System.Collections.Generic;

namespace BussinessObjects;

public partial class Invoice
{
    public int InvoiceId { get; set; }

    public int BookingId { get; set; }

    public decimal? RoomCharge { get; set; }

    public decimal? ServiceCharge { get; set; }

    public decimal? Discount { get; set; }

    public decimal? TotalAmount { get; set; }

    public DateTime? PaymentDate { get; set; }

    public string? PaymentMethod { get; set; }

    public virtual Booking Booking { get; set; } = null!;
}
