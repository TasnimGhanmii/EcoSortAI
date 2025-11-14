using System;
using System.Collections.Generic;

namespace sevrer.Api.Models;

public partial class Classification
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Category { get; set; } = null!;

    public string ImageUrl { get; set; } = null!;

    public float? PredictedConfidence { get; set; }

    public DateTime? ClassifiedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
