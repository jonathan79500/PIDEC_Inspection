using System.ComponentModel.DataAnnotations;

namespace Inspection.Application.DTOs.Inspector;

// Body for POST .../approve and POST .../reject
public class InspectorReviewDto
{
    [StringLength(1000)]
    public string? Comment { get; set; }
}