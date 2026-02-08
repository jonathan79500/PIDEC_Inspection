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
        public string ReportNumber { get; set; }
        public int Version { get; set; }
        public int Revision { get; set; }
        public string InspectorType { get; set; } = "TP | SP";//TP = ThirdPart , SP = SecondPart Inspector


        public InspectionReportNumber CreateNewVersion()
        {

            InspectionReportNumber newReportNumber = new InspectionReportNumber()
            {
                Version = Version + 1,
                Revision = 0
            };

            return newReportNumber;
        }

        public static InspectionReportNumber CreateFromString(string strReportNumber)
        {
            string prjPart = strReportNumber.Substring(0, 4);
            string ordPart = strReportNumber.Substring(5,Math.Max(strReportNumber.IndexOf("IVR-") , strReportNumber.IndexOf("PL-")) - 6);
            int IndexForStartOfVersionPart = Math.Max(strReportNumber.IndexOf("IVR-"), strReportNumber.IndexOf("PL-")) + 4;
            int IndexForEndOfVersionPart = strReportNumber.IndexOf(".");
            int LengthOfVersionPart = IndexForEndOfVersionPart - IndexForStartOfVersionPart;
            int IndexForStartRevisionPart = IndexForEndOfVersionPart + 1;
            int LengthOfRevisionPart = strReportNumber.Length - IndexForStartRevisionPart;
            int versionPart = int.Parse(strReportNumber.Substring(IndexForStartOfVersionPart, LengthOfVersionPart));
            int revisionPart = int.Parse(strReportNumber.Substring(IndexForStartRevisionPart, LengthOfRevisionPart));

            if (strReportNumber.IndexOf("IVR-") > 0)
            {
                InspectionReportNumber newReportNumber = new InspectionReportNumber()
                {


                    InfoPart = "IVR",
                    ProjectNumber = prjPart,
                    NotificationNumber = ordPart,
                    Version = versionPart,
                    Revision = revisionPart
                };
                return newReportNumber;
            }
            else if (strReportNumber.IndexOf("PL-") > 0)
            {
                InspectionReportNumber newReportNumber = new InspectionReportNumber()
                {


                    InfoPart = "PL",
                    ProjectNumber = prjPart,
                    NotificationNumber = ordPart,
                    Version = versionPart,
                    Revision = revisionPart
                };
                return newReportNumber;
            }
            return null;
        }

        public static InspectionReportNumber CreateFromNull(string projectNumber, string NotificationNumber, string reportNumber, string infoPart)
        {

            InspectionReportNumber newReportNumber = new InspectionReportNumber()
            {
                InfoPart = infoPart,
                ProjectNumber = projectNumber,
                NotificationNumber = NotificationNumber,
                ReportNumber = reportNumber,
                Version = 1,
                Revision = 0
            };

            return newReportNumber;
        }

        public InspectionReportNumber CreateNewVersion(string projectNumber, string NotificationNumber, string reportNumber, string infoPart)
        {

            InspectionReportNumber newReportNumber = new InspectionReportNumber()
            {
                InfoPart = infoPart,
                ProjectNumber = projectNumber,
                NotificationNumber = NotificationNumber,
                ReportNumber = reportNumber,
                Version = 1,
                Revision = 0
            };

            return newReportNumber;
        }

        public InspectionReportNumber GenerateNewRevision(string projectNumber,string NotificationNumber, string reportNumber, string infoPart, int version, int revision)
        {

            InspectionReportNumber newReportNumber = new InspectionReportNumber()
            {
                InfoPart = infoPart,
                ProjectNumber = projectNumber,
                NotificationNumber = NotificationNumber,
                ReportNumber = reportNumber,
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
