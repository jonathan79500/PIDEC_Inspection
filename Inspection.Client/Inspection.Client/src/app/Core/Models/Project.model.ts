export interface Project{

  id:string,
  title : string ,
   description:string,
   number:string,
  fSId:string,
  vendors:[]
  createdDate:Date | undefined,
  modifiedDate:Date | undefined,
  createdBy:string | undefined,
  modifiedBy:string | undefined
}