import { Order } from "./Order.model";

export interface Inspection{

  name:string,
  order:Order | undefined,
  city:string,
  notificationNumber:string,
  inspectionSubject:string | undefined,
  contractShippingDueDate:Date | undefined,
  ActualShippingDueDate:Date | undefined,
  proposedDate:Date | undefined,
  ownerId:string | undefined,
  locationId:string | undefined
}