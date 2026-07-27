using Inspection.Domain.Entities.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.DTOs.Common
{
    public class InspectionTypeDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid CreatedBy_ContactId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? VisibleByDefault { get; set; }
        public int ShowOrderNo { get; set; }
    }
}
