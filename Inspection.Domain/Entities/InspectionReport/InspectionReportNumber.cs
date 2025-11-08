using Inspection.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.InspectionReport
{
    public class InspectionReportNumber : BaseDomainEntity<Guid>
    {
        public InspectionReportNumber() { }


        public string ProjectNumber { get; set; }
        public string InfoPart { get; set; } = "IVR | PL";
        public string NotificationNumber { get; set; }
        public int Version { get; set; }
        public int Revision { get; set; }


        public InspectionReportNumber CreateNewVersion()
        {

            InspectionReportNumber newReportNumber = new InspectionReportNumber()
            {
                Version = Version + 1,
                Revision = 0
            };

            return newReportNumber;
        }

        public static InspectionReportNumber CreateFromString(string strNotificationNumber)
        {
            string prjPart = strNotificationNumber.Substring(0, 4);
            string ordPart = strNotificationNumber.Substring(5,Math.Max(strNotificationNumber.IndexOf("IVR-") , strNotificationNumber.IndexOf("PL-")) - 6);
            int IndexForStartOfVersionPart = Math.Max(strNotificationNumber.IndexOf("IVR-"), strNotificationNumber.IndexOf("PL-")) + 4;
            int IndexForEndOfVersionPart = strNotificationNumber.IndexOf(".");
            int LengthOfVersionPart = IndexForEndOfVersionPart - IndexForStartOfVersionPart;
            int IndexForStartRevisionPart = IndexForEndOfVersionPart + 1;
            int LengthOfRevisionPart = strNotificationNumber.Length - IndexForStartRevisionPart;
            int versionPart = int.Parse(strNotificationNumber.Substring(IndexForStartOfVersionPart, LengthOfVersionPart));
            int revisionPart = int.Parse(strNotificationNumber.Substring(IndexForStartRevisionPart, LengthOfRevisionPart));

            if (strNotificationNumber.IndexOf("IVR-") > 0)
            {
                InspectionReportNumber newNotificationNumber = new InspectionReportNumber()
                {


                    InfoPart = "IVR",
                    ProjectNumber = prjPart,
                    NotificationNumber = ordPart,
                    Version = versionPart,
                    Revision = revisionPart
                };
                return newNotificationNumber;
            }
            else if (strNotificationNumber.IndexOf("PL-") > 0)
            {
                InspectionReportNumber newNotificationNumber = new InspectionReportNumber()
                {


                    InfoPart = "PL",
                    ProjectNumber = prjPart,
                    NotificationNumber = ordPart,
                    Version = versionPart,
                    Revision = revisionPart
                };
                return newNotificationNumber;
            }
            return null;
        }

        public static InspectionReportNumber CreateFromNull(string projectNumber, string notificationNumber, string infoPart)
        {

            InspectionReportNumber newNotificationNumber = new InspectionReportNumber()
            {
                InfoPart = infoPart,
                ProjectNumber = projectNumber,
                NotificationNumber = notificationNumber,
                Version = 1,
                Revision = 0
            };

            return newNotificationNumber;
        }

        public InspectionReportNumber CreateNewVersion(string projectNumber, string notificationNumber, string infoPart)
        {

            InspectionReportNumber newNotificationNumber = new InspectionReportNumber()
            {
                InfoPart = infoPart,
                ProjectNumber = projectNumber,
                NotificationNumber = notificationNumber,
                Version = 1,
                Revision = 0
            };

            return newNotificationNumber;
        }

        public InspectionReportNumber GenerateNewRevision(string projectNumber, string notificationNumber, string infoPart, int version, int revision)
        {

            InspectionReportNumber newReportNumber = new InspectionReportNumber()
            {
                InfoPart = infoPart,
                ProjectNumber = projectNumber,
                NotificationNumber = notificationNumber,
                Version = version,
                Revision = revision + 1
            };

            return newReportNumber;
        }

        public InspectionReportNumber IncreaseVersion()
        {
            this.Version++;
            return this;
        }

        public override string ToString()
        {            
            return this.ProjectNumber + "-" + this.NotificationNumber + "-" + InfoPart + "-" + this.Version.ToString() + "." + this.Revision.ToString();
        }
    }
}
