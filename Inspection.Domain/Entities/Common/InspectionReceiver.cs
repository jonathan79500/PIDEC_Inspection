using System;


namespace Inspection.Domain.Entities.Common
{
    public class InspectionReceiver : BaseDomainEntity<Guid>
    {
        public InspectionReceiver(Guid id) : base(id)
        {

        }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; } = "${UserName}@pidec.com";
        public string Description { get; set; }
    }
}
