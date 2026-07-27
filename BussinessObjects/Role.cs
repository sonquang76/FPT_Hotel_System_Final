using System;
using System.Collections.Generic;

namespace BussinessObjects;

public partial class Role
{
    public string RoleId { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
}
