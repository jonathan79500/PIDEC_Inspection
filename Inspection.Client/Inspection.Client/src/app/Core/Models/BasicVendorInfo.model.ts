import { Circulation } from "./Circulation.model";
import { Contact } from "./Contact.model";
import { Inspection } from "./Inspection.model";
import { InspectionSubject } from "./InspectionSubject.model";
import { InspectionType } from "./InspectionType.model";
import { Order } from "./Order.model";
import { OrderContactPerson } from "./OrderContactPerson.model";
import { OrderInspectionSubject } from "./OrderInspectionSubject.model";
import { OrderQwnerContactPerson } from "./OrderOwnerContactPerson.model";
import { OrderSettingsException } from "./OrderSettingsException.model";
import { Project } from "./Project.model";
import { ProjectContact } from "./ProjectContact.model";

export interface BasicVendorInfo{

  name:string,
  userName:string,
  domainId:number,
  projects :Project[],
  orders:Order[],
  inspectionTypes:InspectionType[],
  inspectionSubjects:InspectionSubject[],
  participated_Circulations:Circulation[],
  orderContacts:OrderContactPerson[],
  projectContacts :ProjectContact[],
  orderSettingsExceptions:OrderSettingsException[],
  receivedInspectionNotifications :Inspection[],
  orderInspectionSubjects: OrderInspectionSubject[],
  isLoading : boolean
}
