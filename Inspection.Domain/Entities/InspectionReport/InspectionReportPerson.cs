using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.InspectionReport
{
    public class InspectionReportPerson
    {
        public InspectionReportPerson() { }

        public Guid Id { get; set; }
        public InspectionReportId InspectionReportId { get; set; }
        public Guid PersonId { get; set; }

        public InspectionReport? InspectionReport { get;set; }
        public Person? Person { get; set; }
    }
}
