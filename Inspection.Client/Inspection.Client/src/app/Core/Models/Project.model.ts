import { Contact } from "./Contact.model";

export class Project{

  id:string;
  title : string ;
  description:string;
  number:string;
  fSId:string;

  contacts :Contact[];
  pidecQCContacts: Contact[];
  ownerContacts :Contact[];
  mcContacts :Contact[];
  pidecInspectors:Contact[];
  tpInspectors :Contact[];

  createdDate:Date | undefined;
  modifiedDate:Date | undefined;
  createdBy:string | undefined;
  modifiedBy:string | undefined;

  toString()
  {
    return this.title;
  }


}

export class Project11{

  id:string;
  title : string ;
  description:string;
  number:string;
  fsid:string;

  contacts:Contact[];
  ownerContacts :Contact[];
  mcContacts :Contact[];
  pidecInspectors:Contact[];
  tpInspectors :Contact[];

  createdDate:Date | undefined;
  modifiedDate:Date | undefined;
  createdBy:string | undefined;
  modifiedBy:string | undefined;

  toString()
  {
    return this.title;
  }


}
