using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.Inspector
{

    public class InspectionSkill
    {
        public Guid SkillId { get; set; }

        public string Code { get; set; } = "";     // VT, RT, UT, MT
        public string Name { get; set; } = "";     // Visual Testing

        public bool IsActive { get; set; }

    }
}
