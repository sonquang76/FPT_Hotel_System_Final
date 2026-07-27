using System;
using System.Collections.Generic;

namespace BussinessObjects;

public partial class Account
{
    public string AccountId { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Gender { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public DateTime Dob { get; set; }

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string AccountStatus { get; set; } = null!;

    public string IdentityCard { get; set; } = null!;

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ChangePasswordModel? ChangePasswordModel { get; set; }

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
