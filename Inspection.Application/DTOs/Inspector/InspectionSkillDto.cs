using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.DTOs.Inspector;


public class InspectionSkillDto
{
    public Guid SkillId { get; set; }

    public string Code { get; set; } = "";     // VT, RT, UT, MT
    public string Name { get; set; } = "";     // Visual Testing

    public bool IsActive { get; set; }
    public int InspectorCount { get; set; }   // how many inspectors currently hold this skill

}
public class SkillSaveDto
{
    [Required, StringLength(10)]
    public string Code { get; set; } = "";

    [Required, StringLength(100)]
    public string Name { get; set; } = "";

    public bool IsActive { get; set; } = true;
}
