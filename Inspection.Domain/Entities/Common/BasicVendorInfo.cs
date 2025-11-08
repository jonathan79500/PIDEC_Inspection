

using System;
using System.Collections.Generic;
using System.Data;
using Inspection.Domain.Entities.Contacts;
using Inspection.Domain.Entities.InspectionNotification.Entities;
using Inspection.Domain.Entities.Orders;
using Inspection.Domain.Entities.Projects;

namespace Inspection.Domain.Entities.Common
{
    public class BasicVendorInfo//: BasicUserInfo
    {
        public BasicVendorInfo() { }
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
        public List<OrdersettingException>? OrderSettingsExceptions { get; set; } = [];
        public List<ProjectContact>? ProjectContacts { get; set; } = [];


        //All Contacts with them ,vendor works (Supplier Contacts, Sub Supplier Contacts, Pidec Contacts,Owner Contacts)
        //public List<Contact> contacts { get; set; }
        //public List<Order> Orders { get; set; }


        private List<Order> edmsOrders { get; set; } = [];
        public IReadOnlyList<Order>? EDMSOrders => edmsOrders;

        private List<InspectionNotification.Entities.InspectionNotification> receivedInspectionNotifications = [];
        public IReadOnlyList<InspectionNotification.Entities.InspectionNotification>? ReceivedInspectionNotifications => receivedInspectionNotifications;

        public BasicVendorInfo MergeWithASet(Contact? vendorInfo)
        {
            if (this.Projects == null)
            {
                this.Projects = new List<Project>();
            
            }
            if (this.InspectionTypes == null)
            {
                this.InspectionTypes = new List<InspectionType>();
                
            }
            if(this.Orders == null)
            {
                this.Orders = new List<Order>();
            
            }

            try
            {
                if (vendorInfo?.UserName == null || vendorInfo?.UserName == "")
                {
                    return this;
                }
                else
                {

                    //if (vendorInfo!.Projects != null)
                    //    this.projects.AddRange(vendorInfo!.Projects).ToList());


                    //if (vendorInfo!.Orders != null)
                    //    this.orders.AddRange(vendorInfo!.Orders);
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
            catch { 
            

                return this;
            }
            return this;
        }

    }
}
