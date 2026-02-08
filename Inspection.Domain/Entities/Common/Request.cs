using Inspection.Domain.Entities.Contacts;
using Inspection.Domain.Entities.Enums;
using Inspection.Domain.Entities.InspectionNotification.Enums;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using Inspection.Domain.Entities.User.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.Common
{
    public class Request : BaseDomainEntity<Guid>
    {
        public Request() { }

        public Contact? CreatorContactPerson { get; set; }
        public Guid CurrentRevisionId { get; set; }

        public List<Revision> Revisions { get; set; } = new List<Revision>();
        public string RequestType { get; set; }
        public string CurrentStatus { get; set; }


        public static Request CreateNew(Contact Creator)
        {
            Request request = new Request();
            request.Id = Guid.NewGuid();
            request.CreatorContactPerson = Creator;

            return request;
        }
        public static Request CreateNewRequestWithRevision(RequestTypes requestType, Contact Creator)
        {
            Request request = new Request();
            request.Id = Guid.NewGuid();
            request.CreatorContactPerson = Creator;
            request.RequestType = requestType.ToString();
            request.CurrentStatus = ((int)ActionTypes.CreateInspection).ToString();

            Revision revision = new Revision();
            revision.RevisionId = Guid.NewGuid();
            revision.Request = request;
            revision.RequestId = request.Id;

            request.CurrentRevisionId = revision.RevisionId;

            request.Revisions.Add(revision);
            return request;
        }
    }
}
