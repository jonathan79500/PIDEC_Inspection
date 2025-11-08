import { Contact } from "./Contact.model";
import { ObjectTypes } from "./ObjectTypes.enum";

export interface Letter{

  id:string,
  number:string,
  ReferenceLetterId:string,
  ToContacts:Contact[],
  ccContacts:Contact[],
  Body:string,
  RelatedItemType: ObjectTypes,
  RelatedItemId : string
}
