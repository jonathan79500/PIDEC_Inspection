using Inspection.Application.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.DTOs
{
    public class OrderInspectionSubjectDto
    {
        public string Id { get; set; }        
        public Guid? OrderId { get; set; }
        public Guid? InspectionSubjectId { get; set; }
        public OrderDto Order { get; set; }    
        public InspectionSubjectDto InspectionSubject { get; set; }
    }
}
