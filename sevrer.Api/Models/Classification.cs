using System;
using System.Collections.Generic;
using sevrer.Api.Enums;
namespace sevrer.Api.Models;

public partial class Classification
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public WasteCategory Category { get; set; }

    public string ImageUrl { get; set; } = null!;

    public float? PredictedConfidence { get; set; }

    public DateTime? ClassifiedAt { get; set; }

    public virtual User User { get; set; } = null!;

    public string FormattedDate => ClassifiedAt?.ToString("dd/MM/yyyy") ?? "";
    public string FormattedTime => ClassifiedAt?.ToString("HH:mm:ss") ?? "";
}
