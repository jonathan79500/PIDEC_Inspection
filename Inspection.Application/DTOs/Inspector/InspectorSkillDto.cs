
using Inspection.Domain.Entities.Inspector.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.DTOs.Inspector;

public class InspectorSkillDto
{
    public Guid SkillId { get; set; }
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public InspectorSkillLevel Level { get; set; }
}
