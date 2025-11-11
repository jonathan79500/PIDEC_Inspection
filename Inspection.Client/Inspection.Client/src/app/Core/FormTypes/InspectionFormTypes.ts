import { Owner } from './../Models/Owner.model';
import { FormArray, FormControl, FormGroup } from "@angular/forms"
import { Project } from "../Models/Project.model"
import { Address } from "../Models/Address.model"
import { ObjectTypes } from '../Models/ObjectTypes.enum';



export type FormOrderManagement = FormGroup<{
  order:FormOrder,
  newInspectionSubject : FormInspectionSubject
}>

export type FormAttachment = FormGroup<{
  id:FormControl<string>,
  version : FormControl<string>,
  name :FormControl<string>,
  content:FormControl<Uint8Array[]>
}>

export type FormInspectionType = FormGroup<{
  id:FormControl<string>,
  name : FormControl<string>,
  selected:FormControl<boolean>
}>

export type FormInspectionSubject = FormGroup<{
  id:FormControl<string> ,
  name : FormControl<string>,
  selected:FormControl<boolean>
}>

export type FormProject = FormGroup<{
  id:FormControl<string>,
  title :FormControl<string>,
   description:FormControl<string>,
   number:FormControl<string>,
  fSId:FormControl<string>,
  // vendors:FormArray<FormVendor>,
  ownerContacts :FormArray<FormContact>,
  mcContacts :FormArray<FormContact>,
  pidecInspectors:FormArray<FormContact>,
  tpInspectors :FormArray<FormContact>,
  createdDate:FormControl<Date>,
  modifiedDate:FormControl<Date>,
  createdBy:FormControl<string>,
  modifiedBy:FormControl<string>
}>

export type FormVendor = FormGroup<{
  id : FormControl<string>,
  project: FormControl<{}>,
  userName: FormControl<string>,
  domainId: FormControl<number>,
 name: FormControl<string>,
 orders:FormArray<FormOrder>,
 projects:FormArray<FormProject>
}>


export type FormOrder = FormGroup<{
  id : FormControl<string> | null,
  project: FormControl<Project>,
    no: FormControl<string> | null,
    title: FormControl<string> | null,
    name: FormControl<string> | null,
    equipmentTagNo :FormControl<string> | null,
    edmsId:FormControl<string> | null,
    itP_NO:FormControl<string> | null,
    exceptionContacts:FormArray<FormContact> | null,
    // vendorContactPerson:FormContact,
    contacts :FormArray<FormContact> | null,
    inspectionSubjects:FormArray<FormInspectionSubject> | null,
    version:FormControl<string> | null,
    createdDate:FormControl<Date>,
    modifiedDate: FormControl<Date>,
    createdBy: FormControl<string>,
    modifiedBy: FormControl<string>
}>

export type FormOwner = FormGroup<{
  id:FormControl<string>,
  name :FormControl<string>,
  userName:FormControl<string>,
  domainId:FormControl<number>,
  project : FormControl<{}>,
  ownerJobNo: FormControl<string>,
  pidecJobNo:FormControl<string>
}>

export type FormContact = FormGroup<{
  id:FormControl<string>,
  contactType :FormControl<number>,//for example "To", "CC","Bcc"
  email:FormControl<string>,
  attention :FormControl<string>,
  name :FormControl<string>,
  phone:FormControl<string>,
  mobile:FormControl<string>,
  fax:FormControl<string>,
  cc:FormControl<string>,
  homePage:FormControl<string>,
  address:FormAddress,
  userId:FormControl<string>,
  isVisible:FormControl<boolean>,
  password:FormControl<string>,
  belongedTo :FormControl<number>
}>

export type FormUser = FormGroup<{
  id:FormControl<string>,
  contactType :FormControl<number>,//for example "To", "CC","Bcc"
  email:FormControl<string>,
  attention :FormControl<string>,
  name :FormControl<string>,
  phone:FormControl<string>,
  mobile:FormControl<string>,
  fax:FormControl<string>,
  cc:FormControl<string>,
  homePage:FormControl<string>,
  address:FormAddress,
  userId:FormControl<string>,
  isVisible:FormControl<boolean>,
  userName:FormControl<string>,
  password:FormControl<string>
}>

export type FormLocation = FormGroup<
{
  country : FormControl<string>,
  city : FormControl<string>,
  address :  FormControl<string>
   state :FormControl<string>,
  streetLine1 :FormControl<string>,
  streetLine2 :FormControl<string>,
  zipCode :FormControl<string>
}>

export type FormAddress=FormGroup<{
  id:FormControl<string>,
  country:FormControl<string>,
  state :FormControl<string>,
  city :FormControl<string>,
  streetLine1 :FormControl<string>,
  streetLine2 :FormControl<string>,
  zipCode :FormControl<string>
}>

export type InspectionForm = FormGroup<{
  id:FormControl<string>,
  //vendor:FormVendor,
  ownerDocNo:FormControl<string>,
  pidecDocNo:FormControl<string>,
  attachments: FormArray<FormAttachment>,
  order : FormOrder,
  //owner : FormOwner,
  contacts : FormArray<FormContact>,
  supplier : FormControl<string>,
  sub_supplier :FormControl<string>,
  location: FormLocation,
  supplierContactPerson: FormContact,
  subSupplierContactPerson: FormContact,
  pidecContacts:FormArray<FormContact>,
  pidecInspectors : FormArray<FormContact>,
  thirdPartInspectors:FormArray<FormContact>,
  ownerContactPerson: FormContact,
  mcContactPerson:FormContact,
  workProgress :  FormControl<string>,
  notificationNumber: FormControl<string>,
  RequestInspectionPersianDateStart :FormControl<string>,
  RequestInspectionDateStart :FormControl<Date>,

  RequestInspectionPersianDateEnd :FormControl<string>,
  RequestInspectionDateEnd :FormControl<Date>,

  RequestInspectionDueDate:FormControl<number>,
  RequestInspectionTimesInPeriod:FormControl<number>,
  RequestInspectionPeriodDays:FormControl<number>,

  issueDate :FormControl<Date>,
  persianIssueDate :FormControl<string>,
  // requestInspectionDate:FormControl<Date>,
  // persianRequestInspectionDate:FormControl<string>,
  inspectionTypes:FormArray<FormInspectionType>,
  inspectionSubjects :FormArray<FormInspectionSubject>,
  CreatedDate: FormControl<Date>,
  ModifiedDate: FormControl<Date>,
  CreatedBy: FormControl<string>,
  ModifiedBy: FormControl<string>,
}>

export type InspectionReportForm = FormGroup<{
   inspectionReportId:FormControl<string>,
    inspectionReportNumber:FormControl<string>,
    currentStatus : FormControl<number>,
    inspectorContacts:FormArray<FormContact>,
    relatedInspectionNotification:InspectionForm,
    relatedInspectionNotificationId: FormControl<string>,
    inspectionReportAttachments: FormArray<FormAttachment>
}>
