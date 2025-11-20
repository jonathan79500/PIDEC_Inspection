import { Attachment } from "./Attachment.model";
import { Contact } from "./Contact.model";
import { Inspection } from "./Inspection.model";

export interface InspectionReport{

  inspectionReportId:string,
  inspectionReportNumber:string,
  currentStatus : number, // Conditional Approved=0 | Approved=1 | Rejected=2 | mismatchRecognition=3 | WaitingForAction=4 | null = -1
  inspectorContacts:Contact[],
  relatedInspectionNotification:Inspection,
  relatedInspectionNotificationId: string,
  inspectionReportAttachments: Attachment[],
  createdBy:string,
  issueDate: Date,
  persianIssueDate:string,
  ReportNotes : string,
  ReportType: number //Packing List = 1| Not Packing=0
  //InspectorType : number //ThirdPart Inspector = 3 | SecondPart Inspector = 2
}
