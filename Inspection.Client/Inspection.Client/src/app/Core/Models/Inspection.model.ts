import { Circulation } from './Circulation.model';
import { Address } from "./Address.model";
import { Order } from "./Order.model";
import { Vendor } from "./Vendor.model";
import { InspectionType } from "./InspectionType.model";
import { InspectionSubject } from "./InspectionSubject.model";
import { Contact } from "./Contact.model";
import { Attachment } from "./Attachment.model";

export interface Inspection{

  id:string,
  name:string,
   order:Order | undefined,
  //  vendor:Vendor,
  createdDate:Date |undefined,
  modifiedDate:Date |undefined,
  createdBy: string,
  modifiedBy :string,
  circulations : Circulation[],
  contacts: Contact[],
   notificationNumber:string,
   description:string,
   ownerId :string,
   inspectionSubjects: InspectionSubject[],
   inspectionTypes:InspectionType[],
   attachments:Attachment[],
   contractShippingDueDate:Date | undefined,
   actualShippingDueDate:Date | undefined,
   proposedDate:Date | undefined,
   location: Address,

   issueDate :Date |undefined,
   persianIssueDate : string |undefined,

    requestInspectionPersianDateStart : string |undefined,
    requestInspectionDateStart :Date |undefined,

    requestInspectionPersianDateEnd : string |undefined,
    requestInspectionDateEnd :Date |undefined,

    requestInspectionDueDate: number |undefined,
    requestInspectionTimesInPeriod: number |undefined,
    requestInspectionPeriodDays: number |undefined
}
