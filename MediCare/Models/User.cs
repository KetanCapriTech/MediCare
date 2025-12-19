using System;
using System.Collections.Generic;

namespace MediCareApi.Models;

public partial class User
{
    public long Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public short RoleId { get; set; }

    public DateTime? CreatedOn { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public long? UpdatedBy { get; set; }

    public string? Address { get; set; }

    public bool IsActive { get; set; }
}
