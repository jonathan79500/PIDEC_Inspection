import { inspect } from "util";
import { Inspection } from "./Inspection.model";
import { InspectionSubject } from "./InspectionSubject.model";
import { InspectionType } from "./InspectionType.model";
import { Order } from "./Order.model";
import { Project } from "./Project.model";
import { Circulation } from "./Circulation.model";
import { OrderInspectionSubject } from "./OrderInspectionSubject.model";
import { OrderMCContactPerson } from "./OrderMCContactPerson.model";
import { OrderQwnerContactPerson } from "./OrderOwnerContactPerson.model";
import { Contact } from "./Contact.model";

export interface BasicQCMemberInfo
{
  name:string,
  userName:string,
  domainId:number,
  projects :Project[],
  orders:Order[],
  eDMSOrders:Order[],
  inspectionTypes:InspectionType[],
  inspectionSubjects:InspectionSubject[],
  participated_Circulations:Circulation[],
  orderMCContactPersons:OrderMCContactPerson[],
  orderOwnerContactPersons:OrderQwnerContactPerson[],
  receivedInspectionNotifications :Inspection[],
  orderInspectionSubjects: OrderInspectionSubject[],
  isLoading : boolean,
  usefulContacts : Contact[]
}
