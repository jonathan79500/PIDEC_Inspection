using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Infrastructure.Mail.Helpers
{
    public static class EmailAttributes
    {
        // public static string ServerUrl = "http://localhost:26237/";
        public static string ServerUrl = "http://pcmsv2.pidec.com/app/shared/PcmsLayOut.html";
        // public static string ServerUrl = "https://epcms.pidec.com/app/shared/PcmsLayOut.html";
        public static string UrlPrefix = "";
        public static string MailServerURL = "w2k3srv-app-106.pidec.biz";//"mail.pidec.com";//"w2k3srv-app-108.pidec.biz";
        public static string MailServerUserName = "PcmsAgent";//@"pidec-co\QHSE";
        public static string MailServerPassword = "smcpad*42611";//"QHSE123";
        public static string AgentMailAddress = "PcmsAgent@pidec.com";//"QHSE@pidec.com";
        public static string SignatureAddress = "http://portal.pidec.com/personnel/images/signature/";
        //public static string SignatureAddress = "https://epcms.pidec.com/signatures/";

        public static string BackwardSignatureAddress = SignatureAddress + "Backward.png";
        public static string DenySignatureAddress = SignatureAddress + "deny.jpg";
        public static string ConfirmSignatureAddress = SignatureAddress + "9999.png";
        public static string ForwardSignatureAddress = SignatureAddress + "Forward.png";
        public static string CancelSignatureAddress = SignatureAddress + "Cancel.jpg";
        public static string PcmsPersonnelEmails = ",PcmsPersonnel";
        public static string PcmsSecrataryPersonnelEmails = ",hosseininejad.negin";

    }
}
