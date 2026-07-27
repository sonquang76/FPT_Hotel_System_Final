using System;
using System.Collections.Generic;

namespace BussinessObjects;

public partial class Service
{
    public int ServiceId { get; set; }

    public string ServiceName { get; set; } = null!;

    public decimal Price { get; set; }

    public string Unit { get; set; } = null!;

    public virtual ICollection<Serviceorder> Serviceorders { get; set; } = new List<Serviceorder>();
}
