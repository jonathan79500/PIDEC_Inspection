using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.DTOs.Common
{
    public class AddressDto
    {
        public AddressDto() { }
        public Guid Id { get; set; }= Guid.NewGuid();
        public string Country { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string StreetLine1 { get; set; } = string.Empty;
        public string StreetLine2 { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;

    }
}
