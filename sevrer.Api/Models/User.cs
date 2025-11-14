using System;
using System.Collections.Generic;

namespace sevrer.Api.Models;

public partial class User
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? FullName { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Classification> Classifications { get; set; } = new List<Classification>();
}
