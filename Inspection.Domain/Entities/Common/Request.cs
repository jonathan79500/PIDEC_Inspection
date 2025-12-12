using Inspection.Domain.Entities.Contacts;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using Inspection.Domain.Entities.User.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.Common
{
    public class Request:BaseDomainEntity<Guid>
    {
        public Request() { }
        //public List<Circulation>? Circulations{ get; set; }
        public Contact? CreatorContactPerson { get; set; }
        public List<InspectionNotification.Entities.InspectionNotification> InspectionNotifications { get; set; }
       // public int RequestType { get; set; }
        public static Request CreateNew(Contact Creator)
        {
            Request request = new Request();
            request.Id = Guid.NewGuid();
            request.CreatorContactPerson = Creator;
            
            return request;
        }
    }
}
