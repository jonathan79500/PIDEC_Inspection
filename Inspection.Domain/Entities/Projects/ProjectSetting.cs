using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.Contacts;
using Inspection.Domain.Entities.User.Entities;
using Inspection.Domain.Entities.ValueObjects;

namespace Inspection.Domain.Entities.Projects
{
    public class ProjectSetting
    {
        public ProjectId ProjectId{ get; set; }


        public List<Contact> OwnerContacts { get; set; }
        public List<Contact> MCContacts { get; set; }
        public List<Contact> PidecInspectors { get; set; } //Second Part Inspectors
        public List<Contact> TPInspectors { get; set; }//Third Part Inspectors

        private readonly List<Contact> PIDEC_TO = new();
        private readonly List<Contact> PIDEC_CC = new();
        private readonly List<InspectionSubject> inspectionSubjects = new();
        //prjNo(4digit)-INS|NTF-Order No(3 digits)-Inspection Number(3 digits)-Rev(2digits(revision Numver))
        private string InspectionNumberPattern = @"(\d{4})-INS-(\d{3})-(\d{3})-(Rev\d{2})|(\d{4})-NTF-(\d{3})-(\d{3})-(Rev\d{2})";

    }
}
