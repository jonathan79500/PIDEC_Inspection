using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.DTOs.Common
{
    public class InspectionNotificationContactDto 
    {      
        public InspectionNotificationContactDto() { }

        public Guid? Id { get; set; }= Guid.Empty;
        public string Name { get;  set; }= string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? UserId { get; set; } = string.Empty;
        public int DomainId { get;  set; } = int.MaxValue;
        public AddressDto Address { get;  set; } = new AddressDto();
        public string Phone { get; set; } = string.Empty;
        public string Fax { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string HomePage { get; set; } = string.Empty;
        public string Attention { get; set; } = string.Empty;
        public string Cc { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public int ContactType { get; set; }
        public bool IsVisible { get; set; }
        public int SequenceNo { get; set; }
        public Guid InspectionNotificationId { get; set; }

    }
}
