import { Order } from "./Order.model";

export interface Inspection{

  name:string,
  // order:Order | undefined,
   orderId:string,
   city:string,
   notificationNumber:string,
   inspectionSubject:string | undefined,
   ownerId :string,
   inspectionTypeId:string,
   contractShippingDueDate:Date | undefined,
   ActualShippingDueDate:Date | undefined,
   proposedDate:Date | undefined,  
    locationId: string,
}