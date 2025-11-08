import { Order } from "./Order.model";
import { Project } from "./Project.model";

export interface Vendor{

  id:string,
  name : string ,
  userName:string,
  domainId:number,
  projects:[],
  contacts:[],
  orders:Order[], 
}