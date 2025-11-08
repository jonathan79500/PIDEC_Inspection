import { Attachment } from "./Attachment.model";
import { Contact } from "./Contact.model";
import { Inspection } from "./Inspection.model";

export interface InspectionReport{

  inspectionReportId:string,
  inspectionReportNumber:string,
  currentStatus : number,
  inspectorContacts:Contact[],
  relatedInspectionNotification:Inspection,
  relatedInspectionNotificationId: string,
  inspectionReportAttachments: Attachment[]
}
