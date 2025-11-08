import { Contact } from "./Contact.model"
import { InspectionSubject } from "./InspectionSubject.model"
import { Project } from "./Project.model"
import { VendorDocument } from "./VendorDocument.model"

export interface Order{

  id:string | '00000000-0000-0000-0000-000000000000',
   project : Project ,
   no:string | '',
   title:string | '',
  name:string | '',
  version:string | '',
  equipmentTagNo:string | '',
  edmsId:string | '',
  itP_NO:string | '',
  isAccessibleByEmployer : boolean,
  vendorCompanyName :string,
  hasReadPermission : boolean,
  inspectionSubjects:InspectionSubject[] ,
  exceptionContacts :Contact[],
  contacts:Contact[],
  vendorDocuments:VendorDocument[],
  createdDate:Date | undefined,
  modifiedDate:Date | undefined,
  createdBy:string | undefined,
  modifiedBy:string | undefined
}
