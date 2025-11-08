import { Address } from "./Address.model";
import { ObjectTypes } from "./ObjectTypes.enum";

export interface Contact{

  id:string,
  address : Address ,
   phone:string,
   fax:string,
  name:string,
  email:string,
   homePage:string,
   attention:string,
   contactType:number,
   mobile:string,
   cc:string,
   userId:string,
   isVisible:boolean,
   password:string,
   belongedTo: number,

  // createdDate:Date | undefined,
  // modifiedDate:Date | undefined,
  // createdBy:string | undefined,
  // modifiedBy:string | undefined
}
