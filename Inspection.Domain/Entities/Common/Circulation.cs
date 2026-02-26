using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Inspection.Domain.Entities.Contacts;
using Inspection.Domain.Entities.InspectionNotification.Entities;
using Inspection.Domain.Entities.InspectionNotification.Enums;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using Inspection.Domain.Entities.ValueObjects;

namespace Inspection.Domain.Entities.Common
{
    public class Circulation : BaseDomainEntity<Guid>
    {
        
        public Circulation() : base()
        {


        }
        // public Request? Request { get; set; }
        //public ActionType ActionType { get; set; }

        public Revision Revision { get; set; } = null!;
        public Guid RevisionId { get; set; }

        public int? ActionType { get; set; }
        public string? PerformerPN { get; set; }
        public string? PerformedActionDate { get; set; }
        public int? SequenceNo { get; set; }
        public DateTime? PerformedActionDateGregorian { get; set; }
        public Contact? PerformerContactPerson { get; set; }        
        public int? PerformerContactType { get; set; }
        public int? QCActionType { get; set; }
        public int? PreviousActionType { get; set; }

        public string Comment { get; set; }


        public List<CirculationAttachments> Attachments { get; set; }
        public List<CirculationInspector> Inspectors { get; set; }

        public static Circulation CreateNew(Revision revision, Contact creatorContactPerson)
        {


            string currentDate = "";
            return new Circulation()
            {
                Id = Guid.NewGuid(),
                Revision = revision,
                RevisionId = revision.RevisionId,
                ActionType = (int)ActionTypes.CreateInspection,
                PerformerContactPerson = creatorContactPerson,
                SequenceNo = 0,
                Comment = ""
            };
        }

        public static Circulation CreateForNextContact(InspectionNotification.Entities.InspectionNotification inspectionNotification, Contact contactPerson)
        {


            string currentDate = "";
            return new Circulation()
            {
                Id = Guid.NewGuid(),
                Revision = inspectionNotification.Revisions[0].Revision!,
                RevisionId = inspectionNotification.Revisions[0]!.RevisionId,
                ActionType = null,
                PerformerContactPerson = contactPerson,
                SequenceNo = 1,
                PerformedActionDate = null,
                PerformedActionDateGregorian = null,
                PerformerContactType = contactPerson.ContactType,
                Comment = ""
            };
        }
    }
}
