using Inspection.Domain.Entities.Inspector.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.Inspector
{
    public class InspectorSkill
    {
        public Guid InspectorSkillId { get; set; }

        public Guid InspectorId { get; set; }
        public Inspector Inspector { get; set; } = null!;

        public Guid SkillId { get; set; }
        public InspectionSkill Skill { get; set; } = null!;

        public InspectorSkillLevel Level { get; set; } = InspectorSkillLevel.Experienced;
    }
}
