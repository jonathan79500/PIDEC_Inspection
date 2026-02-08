import { Attachment } from "./Attachment.model";
import { Contact } from "./Contact.model";
import { Inspection } from "./Inspection.model";
import { Person } from "./Person.model";

export interface InspectionReport{

  inspectionReportId:string,
  inspectionReportNumber:string,
  currentStatus : number, // None=0 | Conditional Approved=1 | Approved=2 | Rejected=3 |  WaitingForAction=4 | mismatchRecognition=5 |ResultBasedOnTable = 6|  null = -1
  inspectorContacts:Contact[],
  //relatedInspectionNotification:Inspection,
  relatedInspectionNotificationId: string,
  inspectionReportAttachments: Attachment[],
  requestIdOfRelatedInspectionNotification  : string,
  inspectors: Person[],
  revisionId : string,
  createdBy:string,
  inspectionDate : Date,
  issueDate: Date,
  persianIssueDate:string,
  ReportNotes : string,
  ReportType: number //Packing List = 1| Not Packing=0
  //InspectorType : number //ThirdPart Inspector = 3 | SecondPart Inspector = 2
}
