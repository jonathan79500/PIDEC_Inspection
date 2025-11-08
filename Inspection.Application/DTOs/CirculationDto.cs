using Inspection.Application.DTOs.Common;
using Inspection.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.DTOs
{
    public class CirculationDto
    {
        public CirculationDto()
        {


        }
        public Guid Id { get; set; }
        public RequestDto? Request { get; set; }
        public int? ActionType { get; set; }
        public int QcActionType { get; set; }//because QC Lead befor any action related to vendor Request,
                                             //should gather all commennts and attachments belongs to technical departments and inspectors and then send that to vendor.
                                             //it should do that by approving or rejecting that circulation and its attachments
        public string? PerformerPN { get; set; }
        public string? PerformedActionDate { get; set; }
        public int? SequenceNo { get; set; }
        public DateTime? PerformedActionDateGregorian { get; set; }
        public ContactDto? PerformerContactPerson { get; set; }
        public int? PerformerContactType { get; set; }
        public DateTime? CreatedDate { set; get; }
        public DateTime? ModifiedDate { set; get; }
        public string? Comment { set; get; }
    }
}
