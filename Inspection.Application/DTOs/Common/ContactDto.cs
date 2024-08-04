using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.DTOs.Common
{
    public class ContactDto
    {
        public Guid Id { get; set; }
        public string Name { get;  set; }
        public AddressDto AddressDto { get;  set; }
        public string Phone { get;  set; }
        public string Fax { get;  set; } 
        public string Email { get;  set; }
        public string HomePage { get;  set; }
       
    }
}
