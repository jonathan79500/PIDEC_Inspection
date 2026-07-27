using Inspection.Domain.Entities.InspectionNotification.Entities;
using Inspection.Domain.Entities.Orders;
using Inspection.Domain.Entities.Projects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.Common
{
   
        public class BasicPIDECLeadsInfo
        {

            public BasicPIDECLeadsInfo() { }
        public string Name { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public int DomainId { get; set; }
        public List<Project> Projects { get; set; } = [];
        public List<InspectionType> InspectionTypes { get; set; }
        public List<InspectionSubject> InspectionSubjects { get; set; }
        public List<OrderInspectionSubject> OrderInspectionSubjects { get; set; }
        public List<Circulation> Participated_Circulations { get; set; }
        public List<OrderContact> OrderContacts { get; set; }
        public List<DocumentType> DocumentTypes { get; set; }
        public List<DocumentTypeVisibility> DocumentTypesVisibilities { get; set; }
        //All Contacts with them ,vendor works (Supplier Contacts, Sub Supplier Contacts, Pidec Contacts,Owner Contacts)
        //public List<Contact> contacts { get; set; }
        public List<Order> Orders { get; set; }


        private List<Order> edmsOrders { get; set; } = [];
        public IReadOnlyList<Order> EDMSOrders => edmsOrders;

        private List<InspectionNotification.Entities.InspectionNotification> receivedInspectionNotifications = [];
        public IReadOnlyList<InspectionNotification.Entities.InspectionNotification> ReceivedInspectionNotifications => receivedInspectionNotifications;

        public BasicPIDECLeadsInfo MergeWithASet(OrderContact? qcMemberInfo)
        {
            if (this.Projects == null)
            {
                this.Projects = new List<Project>();

            }
            //if (this.inspectionTypes == null)
            //{
            //    this.inspectionTypes = new List<InspectionType>();

            //}
            if (this.Orders == null)
            {
                this.Orders = new List<Order>();

            }

            try
            {
                if (qcMemberInfo?.Contact.UserName == null || qcMemberInfo?.Contact.UserName == "")
                {
                    return this;
                }
                else
                {
                  
                }
            }
            catch (NullReferenceException ane)
            {
                return this;
            }
            catch (NoNullAllowedException ane)
            {
                return this;
            }
            catch
            {


                return this;
            }
            return this;
        }

        public void SetOrders(List<OrderContact>? orders)
        {
            
        }
        
        public void SetEdmsOrders(List<Order> edmsOrders)
        {
            this.edmsOrders = edmsOrders;
        }

    }
    
}
