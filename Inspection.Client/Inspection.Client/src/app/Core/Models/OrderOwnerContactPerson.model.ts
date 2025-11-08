import { Contact } from "./Contact.model";
import { Order } from "./Order.model";
import { Vendor } from "./Vendor.model";

export interface OrderQwnerContactPerson{

  id:string,
  name:string,
   order:Order | undefined,
   vendor:Vendor,
   ownerContactPerson:Contact,
  
}