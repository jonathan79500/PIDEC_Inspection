using Inspection.Domain.Entities.Contacts;
using Inspection.Domain.Entities.InspectionNotification.Entities;
using Inspection.Domain.Entities.Orders;
using Inspection.Domain.Entities.Projects;
using System;
using System.Collections.Generic;
using System.Data;


namespace Inspection.Domain.Entities.Common
{
    public class BasicQCMemberInfo
    {
        public BasicQCMemberInfo() { }

        public string Name { get; set; }=string.Empty;
        public string UserName { get; set; }=string.Empty;
        public int DomainId { get; set; }
        public List<Project> Projects { get; set; } = [];
        public List<InspectionType> InspectionTypes { get; set; }
        public List<InspectionSubject> InspectionSubjects { get; set; }
        public List<OrderInspectionSubject> OrderInspectionSubjects { get; set; }
        public List<Circulation> Participated_Circulations { get; set; }
        public List<OrderContact> OrderContacts { get; set; }   
        
        public List<Contact> UsefulContacts { get; set; }

        //All Contacts with them ,vendor works (Supplier Contacts, Sub Supplier Contacts, Pidec Contacts,Owner Contacts)
        //public List<Contact> contacts { get; set; }
        public List<Order> Orders { get; set; }
        

        private List<Order> edmsOrders { get; set; } = [];
        public IReadOnlyList<Order> EDMSOrders => edmsOrders;

        private List<InspectionNotification.Entities.InspectionNotification> receivedInspectionNotifications = [];
        public IReadOnlyList<InspectionNotification.Entities.InspectionNotification> ReceivedInspectionNotifications=>receivedInspectionNotifications;

        public BasicQCMemberInfo MergeWithASet(OrderContact? qcMemberInfo)
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
                    //foreach (ProjectQCContactPerson project in qcMemberInfo.Projects)
                    //    this.projects.Add(project!.Project);
                    // if (qcMemberInfo!.Projects != null)


                    //foreach (OrderContact order in qcMemberInfo.Order)
                    //    this.Orders.Add(order!.Order);
                    //if (qcMemberInfo!.Orders != null)


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
            //if(this.Orders == null)
            //    this.Orders = new List<Order>();

            //foreach (Order orderQC in orders)
            //{
            //    this.Orders!.Add(orderQC.Order);
            //}
           // this.orders = orders;
        }
        //public void SetProjects(List<projec> projects)
        //{
        //    if(this.projects == null)
        //        this.projects = new List<Project>();

        //    foreach(ProjectQCContactPerson projectQC in projects)
        //        this.projects!.Add(projectQC.Project);
        //}

        public void SetEdmsOrders(List<Order> edmsOrders) {
            this.edmsOrders = edmsOrders;
        }

    }
}
