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
        public Guid SkillId{get;set;}
        public string Level { get; set; } = "Experienced | proffesional | Internship";
    }
}
