import { Address } from "./Address.model";
import { ObjectTypes } from "./ObjectTypes.enum";

export interface InspectionNotificationContact{

  id:string,
  address : Address ,
   phone:string,
   fax:string,
  name:string,
  email:string,
   homePage:string,
   note:string,
   contactType:number,
   mobile:string,
   cc:string
   sequenceNo:number,
  inspectionNotificationId:string,
  userId:string,
  isVisible: boolean,
  password:string,
  belongedTo:number
}
