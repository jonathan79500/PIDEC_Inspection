using Inspection.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.InspectionNotification.Entities
{
    public class NotificationNumber:BaseDomainEntity<Guid>
    {
        public NotificationNumber() { }

        
        public string ProjectNumber { get; set; }
        public string InfoPart { get; set; } = "INF | NTF";
        public string OrderNumber { get;  set; }
        public int Version { get;  set; }
        public int Revision { get;  set; }


        public NotificationNumber CreateNewVersion()
        {

            NotificationNumber newNotificationNumber = new NotificationNumber()
            {
                ProjectNumber = this.ProjectNumber,
                InfoPart = this.InfoPart,
                OrderNumber = this.OrderNumber,
                Version = Version + 1,
                Revision = 0
            };

            return newNotificationNumber;
        }


        public static NotificationNumber CreateFromString(string strNotificationNumber)
        {
            string prjPart = strNotificationNumber.Substring(0,4);
            string ordPart = strNotificationNumber.Substring(5, strNotificationNumber.IndexOf("NFT-")-6);
            int IndexForStartOfVersionPart = strNotificationNumber.IndexOf("NFT-") + 4;
            int IndexForEndOfVersionPart = strNotificationNumber.IndexOf(".");
            int LengthOfVersionPart = IndexForEndOfVersionPart - IndexForStartOfVersionPart;
            int IndexForStartRevisionPart = IndexForEndOfVersionPart + 1;
            int LengthOfRevisionPart = strNotificationNumber.Length - IndexForStartRevisionPart;
            int versionPart = int.Parse(strNotificationNumber.Substring(IndexForStartOfVersionPart, LengthOfVersionPart));
            int revisionPart = int.Parse(strNotificationNumber.Substring(IndexForStartRevisionPart, LengthOfRevisionPart));

            NotificationNumber newNotificationNumber = new NotificationNumber()
            {
                InfoPart = "NFT",
                ProjectNumber = prjPart,
                OrderNumber = ordPart,
                Version = versionPart,
                Revision = revisionPart
            };

            return newNotificationNumber;
        }

        public static NotificationNumber CreateFromNull(string projectNumber, string orderNumber,string infoPart)
        {

            NotificationNumber newNotificationNumber = new NotificationNumber()
            {
                InfoPart = infoPart,
                ProjectNumber = projectNumber,
                OrderNumber = orderNumber,
                Version = 1,
                Revision = 0
            };

            return newNotificationNumber;
        }

        public NotificationNumber CreateNewVersion(string projectNumber,  string orderNumber, string infoPart)
        {

            NotificationNumber newNotificationNumber = new NotificationNumber()
            {
                InfoPart = infoPart,
                ProjectNumber = projectNumber,
                OrderNumber = orderNumber,
                Version =  1,
                Revision = 0
            };

            return newNotificationNumber;
        }

        public NotificationNumber GenerateNewRevision(string projectNumber, string orderNumber, string infoPart, int version, int revision)
        {

            NotificationNumber newNotificationNumber = new NotificationNumber()
            {
                InfoPart = infoPart,
                ProjectNumber = projectNumber,
                OrderNumber = orderNumber,
                Version = version,
                Revision = revision + 1
            };

            return newNotificationNumber;
        }

        public NotificationNumber IncreaseVersion()
        {
            this.Version++;
            return this;
        }

        public NotificationNumber IncreaseRevision()
        {
            this.Revision++;
            return this;
        }

        public override string ToString()
        {
            return this.ProjectNumber + "-" + this.OrderNumber+"-"+"NFT"+"-"+this.Version.ToString()+"." +this.Revision.ToString();                
        }
    }
}
