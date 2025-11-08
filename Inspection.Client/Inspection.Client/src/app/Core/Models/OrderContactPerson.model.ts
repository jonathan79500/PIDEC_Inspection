import { Contact } from "./Contact.model";
import { Order } from "./Order.model";
import { Vendor } from "./Vendor.model";

export interface OrderContactPerson{

  id:string,
  orderId:string,
   order:Order | undefined,
   contactId:string
   contact:Contact,
  
}