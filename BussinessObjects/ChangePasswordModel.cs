using System;
using System.Collections.Generic;

namespace BussinessObjects;

public partial class ChangePasswordModel
{
    public string AccountId { get; set; } = null!;

    public string OldPassword { get; set; } = null!;

    public string NewPassword { get; set; } = null!;

    public string ConfirmPassword { get; set; } = null!;

    public virtual Account Account { get; set; } = null!;
}
