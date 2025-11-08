import { Contact } from "./Contact.model";
import { Order } from "./Order.model";
import { Vendor } from "./Vendor.model";

export interface OrderSubSupplierContactPerson{

  id:string,
  name:string,
   order:Order | undefined,
   vendor:Vendor,
   subSupplierContactPerson:Contact,
  
}