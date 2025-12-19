using System;
using System.Collections.Generic;

namespace MediCareApi.Models;

public partial class Role
{
    public short Id { get; set; }

    public string RoleName { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public bool IsActive { get; set; }

    public bool IsDelete { get; set; }
}
