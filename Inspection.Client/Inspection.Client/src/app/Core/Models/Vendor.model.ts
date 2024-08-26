export interface Vendor{

  id:string,
  name : string ,
  projects:[],
  contacts:[],
  orders:[],
  createdDate:Date | undefined,
  modifiedDate:Date | undefined,
  createdBy:string | undefined,
  modifiedBy:string | undefined
}