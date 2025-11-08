

using System;
using System.Collections.Generic;
using System.Data;
using Inspection.Domain.Entities.Contacts;
using Inspection.Domain.Entities.InspectionNotification.Entities;
using Inspection.Domain.Entities.InspectionReport;
using Inspection.Domain.Entities.Orders;
using Inspection.Domain.Entities.Projects;

namespace Inspection.Domain.Entities.Common
{
    public class BasicInspectorInfo//: BasicUserInfo
    {
        public BasicInspectorInfo() { }
        //All Contacts with them ,vendor works (Supplier Contacts, Sub Supplier Contacts, Pidec Contacts,Owner Contacts)
        //public List<Contact> contacts { get; set; }
        public string? Name { get; set; }=string.Empty;
        public string? UserName { get; set; } = string.Empty;
        public int? DomainId { get; set; }
        public List<Project>? Projects { get; set; } = [];
        public List<Order>? Orders { get; set; }

        public List<InspectionSubject>? InspectionSubjects { get; set; } = [];
        public List<InspectionType>? InspectionTypes { get; set; } = [];
        public List<OrderInspectionSubject>? OrderInspectionSubjects { get; set; } = [];
        public List<Circulation>? Participated_Circulations { get; set; } = [];
        public List<OrderContact>? OrderContacts { get; set; } = [];
    


        //All Contacts with them ,vendor works (Supplier Contacts, Sub Supplier Contacts, Pidec Contacts,Owner Contacts)
        //public List<Contact> contacts { get; set; }
        //public List<Order> Orders { get; set; }
    

        private List<InspectionNotification.Entities.InspectionNotification> receivedInspectionNotifications = [];
        public IReadOnlyList<InspectionNotification.Entities.InspectionNotification>? ReceivedInspectionNotifications => receivedInspectionNotifications;



        private List<InspectionReport.InspectionReport> inspectionReports = [];
        public IReadOnlyList<InspectionReport.InspectionReport>? InspectionReports => inspectionReports;

    }
}
