import { OrderSettingsException } from './../../../Core/Models/OrderSettingsException.model';
import { Project } from './../../../Core/Models/Project.model';
import { join } from 'node:path';
import { Circulation } from '../../../Core/Models/Circulation.model';
import { FormContact } from '../../../Core/FormTypes/InspectionFormTypes';
import { InspectionSubject } from '../../../Core/Models/InspectionSubject.model';
import { Owner } from '../../../Core/Models/Owner.model';
import { Contact } from '../../../Core/Models/Contact.model';
import { ChangeDetectorRef, Component, effect, Inject, inject, Injector, OnInit, runInInjectionContext, ViewEncapsulation } from '@angular/core';
import { Inspection } from '../../../Core/Models/Inspection.model';
import { InspectionService, queryNewNotificationNumberResult } from '../../../Core/Services/inspection.service';
import { Order } from '../../../Core/Models/Order.model';
import { AbstractControl, FormArray, FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule, formatDate } from '@angular/common';
import { UploadFilesComponent } from '../../../Core/General/Components/upload-files/upload-files.component';
import { MatDatepickerInputEvent, MatDatepickerModule} from '@angular/material/datepicker';
import { MatDatepickerModulePersian } from 'ngx-persian-datepicker';
import { MAT_DATE_FORMATS, MatDateFormats, MatNativeDateModule } from '@angular/material/core';
import { toGregorian, toJalaali } from 'jalaali-js';
//import {GregorianDateWithMonthNamePipe} from '../../../Core/General/Pipes/gregorian-date-with-month-name.pipe';

import {    ColDef,ColGroupDef,  DataChangedEvent,  FirstDataRenderedEvent,  GridApi,
  GridOptions,
  PaginationNumberFormatterParams,
  _isVisible,
  createGrid,} from "ag-grid-community";
import "ag-grid-community/styles/ag-grid.css";
import "ag-grid-community/styles/ag-theme-quartz.css";

import "ag-grid-community/styles/ag-grid.css";
import "ag-grid-community/styles/ag-theme-quartz.css";
import { IDatepickerTheme, IActiveDate, NgPersianDatepickerModule } from 'ng-persian-datepicker';
 import { darkTheme} from '../../../Themes/darkTheme';
import { CFileUploadComponent } from "../../../Core/General/Components/cfile-upload/cfile-upload.component";
import { ContactpersonSelectorComponent } from '../../contactperson-selector/contactperson-selector.component';
import {
  MAT_DIALOG_DATA,
  MatDialog,
  MatDialogActions,
  MatDialogClose,
  MatDialogContent,
  MatDialogRef,
  MatDialogTitle,
} from '@angular/material/dialog';
import { UserService } from '../../../Core/Services/user.service';
import { Vendor } from '../../../Core/Models/Vendor.model';
import { InspectionType } from '../../../Core/Models/InspectionType.model';
import { ActivatedRoute } from '@angular/router';
import { map } from 'rxjs';
import { BasicVendorInfo } from '../../../Core/Models/BasicVendorInfo.model';
import { Address } from '../../../Core/Models/Address.model';
import { Toast, ToastrService } from 'ngx-toastr';
import { currentstateStore } from '../../../Stores/currentstate.store';
import { vendorInfoStore } from '../../../Stores/vendorInfo.store';
import * as FormTypes from '../../../Core/FormTypes/InspectionFormTypes';
import { ContactTypes } from '../../../Core/Models/ContactTypes.enum';
import { UserTypes } from '../../../Core/Models/UserTypes.enum';
import { inspect } from 'node:util';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatTooltipModule } from '@angular/material/tooltip';
import { Attachment } from '../../../Core/Models/Attachment.model';
import { userInfo } from 'node:os';
import { ObjectTypes } from '../../../Core/Models/ObjectTypes.enum';
import { ProjectContact } from '../../../Core/Models/ProjectContact.model';



function fillLarge() {
  setWidthAndHeight("100%");
}

function fillMedium() {
  setWidthAndHeight("60%");
}

function fillExact() {
  setSize("100%","850px")
  //setWidthAndHeight("800px");
}

function setWidthAndHeight(size: string) {
  var eGridDiv = document.querySelector<HTMLElement>("#myGrid")! as any;
  eGridDiv.style.setProperty("width", size);
  eGridDiv.style.setProperty("height", size);
}
function setSize(width: string,height:string) {
  var eGridDiv = document.querySelector<HTMLElement>("#myGrid")! as any;
  eGridDiv.style.setProperty("width", width);
  eGridDiv.style.setProperty("height", height);
}
let gridApi: GridApi<Inspection> ;
const gridOptions: GridOptions<Inspection> = {
  columnDefs: [
    { field: "name", width: 150 },
    // { field: "inspectionSubject", width: 90 }
  ],
  domLayout:'normal'
};



export const MY_FORMATS: MatDateFormats = {
  parse: {
    dateInput: 'LL',
  },
  display: {
    dateInput: 'MMMM d, yyyy',
    monthYearLabel: 'MMM yyyy',
    dateA11yLabel: 'MMMM d, yyyy',
    monthYearA11yLabel: 'MMMM yyyy',
  }
};

@Component({
    selector: 'app-newinspection',
    imports: [CommonModule, ReactiveFormsModule, FormsModule,  NgPersianDatepickerModule, MatDatepickerModule,MatFormFieldModule,MatTooltipModule],
    templateUrl: './vnewinspection.component.html',
    styleUrl: './vnewinspection.component.css',
    encapsulation: ViewEncapsulation.None,
    providers: [
      { provide: MAT_DATE_FORMATS, useValue: MY_FORMATS }, // <-- This line sets the custom date format for mat-datepicker
    ]
})
export class VnewinspectionComponent implements OnInit{

  initialInspectionInfo: queryNewNotificationNumberResult;
  fgNewInspection !:FormGroup;
  newInspection !:Inspection ;
  attachedDocs: File[];
  formData : FormData;
  readonly dialog = inject(MatDialog);
  injector = inject(Injector);
  Project:Project;
  currentProject : Project;
  Orders:Order[];
  currentVendorData:Vendor;
  inspectionForm : FormTypes.InspectionForm ;
  currentContacts : Contact[];
  currentInspectionSubjects :InspectionSubject[];
  currentInspectionTypes:InspectionType[];
  editMode :boolean = true;

 // dateFormat: string = 'YYYY MMMM DD';
uiIsVisible: boolean = true;

uiTheme: IDatepickerTheme =darkTheme;
uiYearView: boolean = true;
uiMonthView: boolean = true;
uiHideAfterSelectDate: boolean = false;
uiHideOnOutsideClick: boolean = false;
uiTodayBtnEnable: boolean = false;
public LTRMARK: string = "&lrm;";

timeEnable: boolean = false;
timeMeridian: boolean = false;
currentAppState = inject(currentstateStore);
currentVendorStore = inject(vendorInfoStore);
realIssueDate :Date=new Date(Date.now());
isManualUpdateIssueDate: boolean = false;
isMAnualUpdateReqInspectionDate:boolean = false;
isManualUpdateRequestInspectionDateStart :boolean=false;
isManualUpdateRequestInspectionDateEnd :boolean=false;
isManualUpdateIssueDate1: boolean = false;
formattedPersianIssueDate1: string = '';
newInspectionType: string = '';
newInspectionSubject:string ='';
currentCirculation : Circulation;
orderSettingsExceptions:OrderSettingsException[];
currentOrderExceptionContacts: ProjectContact[];
// Add this to your component
private gregorianMonthNames = [
  'January', 'February', 'March', 'April', 'May', 'June',
  'July', 'August', 'September', 'October', 'November', 'December'
];

 persianMonthNames = [
    'Farvardin', 'Ordibehesht', 'Khordad', 'Tir', 'Mordad', 'Shahrivar',
    'Mehr', 'Aban', 'Azar', 'Dey', 'Bahman', 'Esfand'
  ];

    formattedGregorianIssueDate: string = '';
  formattedPersianIssueDate: string = '';


  formattedGregorianReqStartDate: string = '';
  formattedPersianReqStartDate: string = '';


  formattedGregorianReqEndDate: string = '';
  formattedPersianReqEndDate: string = '';

  constructor(private activatedRoute: ActivatedRoute,private inspectionService:InspectionService,private formBuilder:FormBuilder,private toastr:ToastrService,private cdr: ChangeDetectorRef) {

    this.formData = new FormData();
    this.initialInspectionInfo = activatedRoute.snapshot.data['inspectionInfo'];
    this.currentVendorData = this.currentAppState.vendorData();
    this.currentProject = this.currentAppState.currentproject();
    if(this.currentAppState.currentuser().userType == UserTypes.Vendor)
    {
       this.orderSettingsExceptions = this.currentAppState.vendorBasicInfo().orderSettingsExceptions;
      this.Project = this.currentAppState.vendorBasicInfo().projects[0];
      this.Orders = this.currentAppState.vendorBasicInfo().orders;//this.currentVendorData['orders'];

    }
    else if(this.currentAppState.currentuser().userType == UserTypes.PIDEC_TO)
    {

    }
  }

toggleEditMode(on: boolean) {
  this.editMode = on;
  const CtrlRequestInspectionPersianDateStart = this.inspectionForm.get('RequestInspectionPersianDateStart');

  const CtrlRequestInspectionPersianDateEnd = this.inspectionForm.get('RequestInspectionPersianDateEnd');

  const CtrlpersianIssueDate = this.inspectionForm.get('persianIssueDate');


  if (on) {
    CtrlRequestInspectionPersianDateStart?.enable();
    CtrlRequestInspectionPersianDateEnd?.enable();
    CtrlpersianIssueDate?.enable();
  } else {
    CtrlRequestInspectionPersianDateStart?.disable();
    CtrlRequestInspectionPersianDateEnd?.disable();
    CtrlpersianIssueDate?.disable();
  }
}
  ngAfterViewInit() {
  //console.log('Form:', this.inspectionForm);
  //console.log('modifiedDate:', this.inspectionForm.get('ModifiedDate')?.value);
}

  ngOnInit(): void {

    let DateNow = new Date(Date.now());

    this.activatedRoute.paramMap.subscribe(params=>{

    if(params.keys.length>0)
      {
        if(params.keys[0]=='notificationNumber')
        {
      var inspectionNotificationNumber = params.get('notificationNumber').toString();

      let obj :{} = {'notificationNumber':inspectionNotificationNumber};
      this.formData.append('notificationNumber',JSON.stringify(obj));
      this.inspectionService.getInspectionByNotificationNumber(inspectionNotificationNumber).subscribe((inspection)=>{
        let lastInspectionIndex = inspection.inspectionNotifications.length-1;
        let lastInspectionNotificationId = inspection.inspectionNotifications[lastInspectionIndex].id;
        let relatedInspectionContact =  inspection.contacts.filter((contact)=>contact.inspectionNotificationId == lastInspectionNotificationId);
        let relatedInspectionSubjects = inspection.subjects.filter((subject)=>subject.inspectionNotificationId == lastInspectionNotificationId);
        let relatedInspectionTypes = inspection.types.filter((type)=>type.inspectionNotificationId == lastInspectionNotificationId);

       this.generateInspectionFromValue(inspection.inspectionNotifications[lastInspectionIndex]);

        relatedInspectionContact.forEach((relContact)=>{
          this.contactsFormArray.push(this.generateContactFromContactValue(relContact));
        })

        relatedInspectionSubjects.forEach((relSubject)=>{
          this.inspectionSubjectsFormArray.push(this.generateInspectionSubjectFromValue(relSubject));
        })
        relatedInspectionTypes.forEach((relType)=>{
          this.inspectionTypesFormArray.push(this.generateInspectionTypeFromValue(relType));
        })

      });
    }
    else if(params.keys[0]=='id')
        {
           var inspectionNotificationId = params.get('id').toString();

      let obj :{} = {'inspectionNotificationId':inspectionNotificationId};
      this.formData.append('inspectionNotificationId',JSON.stringify(obj));
      this.inspectionService.getInspectionById(inspectionNotificationId).subscribe((inspection)=>{

       this.formData.append('readInspection',JSON.stringify(inspection));

      });
        }
    }
    else
    {
        this.inspectionForm = this.generateInspectionForm();

    }
    });

    runInInjectionContext(this.injector,()=>{
      effect(()=>{
         this.activatedRoute.paramMap.subscribe(params=>{

        // if(this.currentAppState.currentuser().userType == UserTypes.Vendor)
        //   {
             if(params.keys[0]=='id')
              {
              var inspectionNotificationId = params.get('id').toString();
               let strReadInspection = this.formData.get("readInspection");
               let readInspectionObj:Inspection;
               if(strReadInspection != null)
               {
                  readInspectionObj = JSON.parse(strReadInspection.toString());
                  this.generateInspectionFromValue(readInspectionObj);
               }
               else
               {
                 this.inspectionService.getInspectionById(inspectionNotificationId).subscribe((inspection)=>{
                    this.generateInspectionFromValue(inspection);
                   });
               }
              }
              else // When we are creating new Inspection and select an order
              {

                let currentOrder:Order = this.currentAppState.currentorder();
                this.currentOrderExceptionContacts = this.orderSettingsExceptions.filter((ose)=>ose.orderId== currentOrder.id).map((ose)=>ose.projectContact);
               // let exceptedProjContacts = this.currentAppState.vendorBasicInfo().orderSettingsExceptions.filter((orderSetting)=>orderSetting.orderId == currentOrder.id);

                this.initializeInspectionForm(currentOrder)

                  this.inspectionForm.patchValue({
          'order':{
            name:currentOrder.name,
            no:currentOrder.no,
            createdBy:'',
            createdDate:currentOrder.createdDate,
            id:currentOrder.id,
            modifiedBy:currentOrder.modifiedBy,
            modifiedDate:currentOrder.modifiedDate,
            project:this.currentProject,
            title:currentOrder.title,
            edmsId:currentOrder.edmsId,
            equipmentTagNo:''  ,
            itP_NO:currentOrder.itP_NO,
            //contacts: { currentContacts.filter((contact)=>contact.contactType==(ContactTypes.PIDEC_TO || ContactTypes.PIDEC_CC))},
            inspectionSubjects:currentOrder.inspectionSubjects
          },
          // 'owner':{
          //   domainId:1,
          //   userName:'owner1',
          //   project:currentOrder.project
          // },
          'ownerContactPerson': this.currentContacts.filter((contact)=>contact.contactType==ContactTypes.Owner)[0],
          'mcContactPerson':this.currentContacts.filter((contact)=>contact.contactType==ContactTypes.MC)[0],
          'contacts':  this.currentContacts,
          'pidecContacts':this.currentContacts.filter((contact)=>contact.contactType==ContactTypes.QC_Manager || contact.contactType==ContactTypes.QC_Lead),
          'pidecInspectors':this.currentContacts.filter((contact)=>contact.contactType==ContactTypes.PIDEC_Inspector),
          'thirdPartInspectors':this.currentContacts.filter((contact)=>contact.contactType==ContactTypes.ThirdPart_Inspector),
              });



       let pidecContacts = this.currentContacts?.filter(
            (contact) => contact.contactType === ContactTypes.QC_Manager || contact.contactType === ContactTypes.QC_Lead
          );

      if(pidecContacts != null)
        {
          pidecContacts.forEach(contact => this.contactsFormArray.push(
            this.formBuilder.group({
              id: [contact.id],
              phone: [contact.phone],
              fax: [contact.fax],
              name: [contact.name],
              email: [contact.email],
              homePage: [contact.homePage],
              attention: [contact.attention],
              contactType: [contact.contactType],
              mobile: [contact.mobile],
              cc: [contact.cc],
              userId:[contact.userId],
              isVisible : [true],
              address: contact!.address != null ? this.generateAddressFromAddressValue(contact.address) : this.generateAddress(),
              password:[contact.password],
              belongedTo:[contact.belongedTo]
            })
          ));
        }


        let ownerContacts = this.currentContacts?.filter(
          (contact) => contact.contactType === ContactTypes.Owner);

        if(ownerContacts != null)
          {
            ownerContacts.forEach(contact => this.ownerContact.patchValue(
              {
                id: contact.id,
                phone: contact.phone,
                fax: contact.fax,
                name: contact.name,
                email: contact.email,
                homePage: contact.homePage,
                attention: contact.attention,
                contactType: contact.contactType,
                mobile: contact.mobile,
                cc: contact.cc,
                address: contact.address
              })
            )

          }

          let mcContacts = this.currentContacts?.filter(
            (contact) => contact.contactType === ContactTypes.MC);

          if(mcContacts != null)
            {
              mcContacts.forEach(contact => this.mcContact.patchValue(
                {
                  id: contact.id,
                  phone: contact.phone,
                  fax: contact.fax,
                  name: contact.name,
                  email: contact.email,
                  homePage: contact.homePage,
                  attention: contact.attention,
                  contactType: contact.contactType,
                  mobile: contact.mobile,
                  cc: contact.cc,
                  address: contact!.address
                })
              )

            }
          }

      if(this.currentInspectionTypes !=null && this.currentInspectionTypes !=undefined)
      {
          this.currentInspectionTypes.forEach((inspectionType)=>

            this.inspectionTypesFormArray.push(this.formBuilder.group({
              id:[inspectionType.id],
              name:[inspectionType.name],
              selected:[false]
              }))
          );}

       if(this.currentInspectionSubjects != null && this.currentInspectionSubjects != undefined)
       {
        this.currentInspectionSubjects.forEach((subject)=>
        this.inspectionSubjectsFormArray.push(
          this.formBuilder.group({
            name:[subject.name],
            id:[subject.id],
            selected : [false]
          })
       ));
      }



       // }
      });
      })

  })





  }

LocalFormatDate(date:Date)
{
  const gregorianDay = date.getDate();
      const gregorianMonth = date.getMonth();
      const gregorianYear = date.getFullYear();
      const gregorianMonthName = this.getGregorianMonthName(gregorianMonth);

      // Set formatted Gregorian date with month name
      let formattedGregorianDate = `${gregorianDay} ${gregorianMonthName} ${gregorianYear}`;
      return  formattedGregorianDate;
}

   // Utility to create a new form group for an inspection type
  createInspectionType(name: string, selected: boolean = false): FormGroup {
    return this.formBuilder.group({
      name: [name, Validators.required],
      selected: [true]
    });
  }
    // Adds a new InspectionType if the input is not empty
  addInspectionType(): void {
    const trimmedName = this.newInspectionType.trim();
    if (trimmedName) {
      this.inspectionTypesFormArray.push(this.createInspectionType(trimmedName, false));
      // Optionally clear the input after adding
      this.newInspectionType = '';
    }
  }


    // Utility to create a new form group for an inspection subject
  createInspectionSubject(name: string, selected: boolean = false): FormGroup {
    return this.formBuilder.group({
      name: [name, Validators.required],
      selected: [true]
    });
  }
    // Adds a new InspectionType if the input is not empty
  addInspectionSubject(): void {
    const trimmedName = this.newInspectionSubject.trim();
    if (trimmedName) {
      this.inspectionSubjectsFormArray.push(this.createInspectionSubject(trimmedName, false));
      // Optionally clear the input after adding
      this.newInspectionType = '';
    }
  }

removeToContact(contactIndex:number):void
{
  this.contactsFormArray.removeAt(contactIndex);

}

  addNewToContact():void{

   let rawContact = this.formBuilder.group({
      id:'00000000-0000-0000-0000-000000000000',
      attention:'',
      cc:'',
      email:'',
      fax:'',
      name:'',
      phone:'',
      mobile:'',
      address:this.generateAddress(),
      contactType:ContactTypes.QC_Lead,
      homePage:'',
      userId:'',
      isVisible:[true],
      password:'',
      belongedTo:ObjectTypes.Undefined
    })
    this.contactsFormArray.push(rawContact);
    this.pidecContactsFormArray.push(rawContact);
  }

  changeContactEmail(indexOfelement)
  {
    if(this.contactsFormArray.controls[indexOfelement].get('email').value != '')
       this.contactsFormArray.controls[indexOfelement].patchValue({contactType:ContactTypes.QC_Manager}) ;
  }
  onInspectionTypeChange(inspectionTypeIndex : any):void{
    // inspectionType.selected = !inspectionType.selected;
    // const inspectionType = this.inspectionTypesFormArray.at(inspectionTypeIndex);
    //   inspectionType.patchValue({
    //     // id:inspectionType.value.id,
    //     // name:inspectionType.value.name,
    //     selected: !inspectionType.value.selected });
    }

    onInspectionSubjectChange(inspectionSubjectIndex : any):void{
      //inspectionSubject.selected = !inspectionSubject.selected;
      // const subject = this.inspectionSubjectsFormArray.at(inspectionSubjectIndex);
      // subject.patchValue({
      //   // id:subject.value.id,
      //   // name: subject.value.name,
      //   selected: !subject.value.selected });
    }


    generateInspectionForm():FormTypes.InspectionForm{

      let DateNow = new Date(Date.now());
    let formattedDateNow =  this.LocalFormatDate(DateNow);
    this.formattedGregorianIssueDate = formattedDateNow;
    this.formattedGregorianReqEndDate = formattedDateNow;
    this.formattedGregorianReqStartDate = formattedDateNow;
    let currentUserName= this.currentAppState.currentuser().userName;

      const gregorianDate: Date = DateNow;
      const { jy, jm, jd } = toJalaali(gregorianDate.getFullYear(), gregorianDate.getMonth() + 1, gregorianDate.getDate());

        const formattedPersianDate = `${jy}/${jm}/${('0' + jd).slice(-2)}`;


        let currentOrderContacts = this.currentAppState.vendorBasicInfo().orderContacts;
      return this.formBuilder.group({

          id:'00000000-0000-0000-0000-000000000000',
          attachments: this.formBuilder.array<FormTypes.FormAttachment>([this.generateAttachment()]),
          contacts : this.formBuilder.array<FormTypes.FormContact>([this.generateContact()]),
          pidecContacts: this.formBuilder.array<FormTypes.FormContact>([this.generateContact()]),
          pidecInspectors : this.formBuilder.array<FormTypes.FormContact>([this.generateContact()]),
          thirdPartInspectors:this.formBuilder.array<FormTypes.FormContact>([this.generateContact()]),
          location: this.generateLocation(),
          order:this.generateOrder(),
        //  owner:this.generateOwner(),
         // vendor:this.generateVendor(),
          ownerContactPerson: this.generateContact(),
          mcContactPerson:this.generateContact(),
          supplierContactPerson:this.generateContact(),
          subSupplierContactPerson:this.generateContact(),
          sub_supplier:'',
          ownerDocNo:'',
          pidecDocNo:'',
          supplier:'',
          workProgress:new Date().toLocaleDateString('fa-IR-u-nu-latn', {
            year: 'numeric',
            day: '2-digit',
            month: 'long'
          }),
          notificationNumber:this.initialInspectionInfo.newNotificationNumber,

          CreatedBy: [currentUserName],
          ModifiedBy :[currentUserName],
          CreatedDate:[ DateNow],
          ModifiedDate : [DateNow],
          issueDate:DateNow,
          persianIssueDate:formattedPersianDate ,
          RequestInspectionDateStart: DateNow,
          RequestInspectionDateEnd:DateNow,
          RequestInspectionPersianDateStart:formattedPersianDate,
          RequestInspectionPersianDateEnd:formattedPersianDate,
          RequestInspectionDueDate:0,
          RequestInspectionTimesInPeriod:0,
          RequestInspectionPeriodDays:0,
          inspectionSubjects:this.formBuilder.array<FormTypes.FormInspectionSubject>([this.generateInspectionSubject()]),
          inspectionTypes: this.formBuilder.array<FormTypes.FormInspectionType>([this.generateInspectionType()]),

        });
    }

  generateInspectionType():FormTypes.FormInspectionType{
    return this.formBuilder.group({
      id:'00000000-0000-0000-0000-000000000000',
      name:'',
      selected : new FormControl<boolean>(false)
    });
  }

  generateInspectionSubject():FormTypes.FormInspectionSubject{
    return this.formBuilder.group({
      id:'00000000-0000-0000-0000-000000000000',
      name:'',
      selected : new FormControl<boolean>(false)
    });
  }

  generateAttachment():FormTypes.FormAttachment{
    return this.formBuilder.group({
      id:'00000000-0000-0000-0000-000000000000',
      version:'' ,
      name:'',
      content: []
    });
  }

  generateOrder():FormTypes.FormOrder{
    let currentOrderNo:string = '';
    let currentOrderName:string = '';
    let currentOrderProjectId:string = '';
    let currentOrder:Order = this.currentAppState.currentorder();

    if( currentOrder!= null && undefined)
    {
      currentOrderNo = currentOrder.no;
      currentOrderName = currentOrder.name;

    }

    return this.formBuilder.group({
      createdBy:'',
      createdDate: new Date(Date.now()),
      id:'00000000-0000-0000-0000-000000000000',
      modifiedBy:'',
      modifiedDate:new Date(Date.now()),
      edmsId:currentOrder.edmsId,
      name:currentOrderName,
      no:currentOrderNo,
      version:'0',
      project:this.currentAppState.currentproject(),
      title:currentOrderName,
      equipmentTagNo:'',
      itP_NO:currentOrder.itP_NO,
      exceptionContacts:this.formBuilder.array<FormTypes.FormContact>([this.generateContact()]),
      contacts:this.formBuilder.array<FormTypes.FormContact>([this.generateContact()]),
      inspectionSubjects:this.formBuilder.array<FormTypes.FormInspectionSubject>([this.generateInspectionSubject()]),
    })
  }

  generateOrderFromValue(order:Order):FormTypes.FormOrder{
    let currentOrderNo:string = '';
    let currentOrderName:string = '';
    let currentOrderProjectId:string = '';
    let currentOrder:Order = order;

    if( currentOrder!= null && currentOrder!=undefined)
    {
      currentOrderNo = currentOrder.no!;
      currentOrderName = currentOrder.name!;
    }

    return this.formBuilder.group({
      createdBy:order.createdBy,
      createdDate: order.createdDate,
      id:order.id,
      modifiedBy:order.modifiedBy,
      modifiedDate:order.modifiedDate,
      edmsId:order.edmsId,
      name:currentOrderName,
      no:currentOrderNo,
      project:this.currentAppState.currentproject(),
      version:currentOrder.version,
      itP_NO:currentOrder.itP_NO,
      exceptionContacts:this.formBuilder.array<FormTypes.FormContact>([this.generateContact()]),
      contacts:this.formBuilder.array<FormTypes.FormContact>([]),
      inspectionSubjects:this.formBuilder.array<FormTypes.FormInspectionSubject>([]),
      title:currentOrderName,
      equipmentTagNo:order.equipmentTagNo
    })
  }

  generateProject():FormTypes.FormProject
  {
    return this.formBuilder.group({
      createdBy:'',
      createdDate: new Date(Date.now()),
      id:'00000000-0000-0000-0000-000000000000',
      modifiedBy:'',
      modifiedDate:new Date(Date.now()),
      description:this.currentVendorData.orders[0].project.description,
      fSId:this.currentVendorData.orders[0].project.fSId,
      number:this.currentVendorData.orders[0].project.number,
      title:this.currentVendorData.orders[0].project.title,
      //vendors:this.formBuilder.array<FormTypes.FormVendor>([]),
      mcContacts:this.formBuilder.array<FormTypes.FormContact>([]),
      ownerContacts:this.formBuilder.array<FormTypes.FormContact>([]),
      tpInspectors:this.formBuilder.array<FormTypes.FormContact>([]),
      pidecInspectors:this.formBuilder.array<FormTypes.FormContact>([]),

    });
  }
  generateVendor():FormTypes.FormVendor{
console.log('current vendor:',this.currentVendorData);
    return this.formBuilder.group({
      domainId:this.currentVendorData.domainId,
      id:this.currentVendorData.id,
      name:this.currentVendorData.name,
     // orders:this.formBuilder.array<FormOrder>([this.generateOrder()]),
     orders:this.formBuilder.array<FormTypes.FormOrder>([]),
      // project:this.generateProject(),
      project:{},
      //projects:this.formBuilder.array<FormProject>([this.generateProject()]),
      projects:this.formBuilder.array<FormTypes.FormProject>([]),
      userName:this.currentVendorData.userName,

    })
  }
  generateVendorFromValue(vendor:Vendor):FormTypes.FormVendor{
    console.log('current vendor v:',vendor);
    return this.formBuilder.group({
      domainId:vendor.domainId,
      id:vendor.id,
      name:vendor.name,
      userName:vendor.userName,
     orders:this.formBuilder.array<FormTypes.FormOrder>([]),
      project:{},
      projects:this.formBuilder.array<FormTypes.FormProject>([]),

    })
  }


  generateOwner():FormTypes.FormOwner{
    return this.formBuilder.group({
      id:'00000000-0000-0000-0000-000000000000',
      name:'',
      userName:'OwnerTest',
      domainId:0,
      ownerJobNo:'',
      pidecJobNo:'',
      project:{},
    })
  }

  generateContact():FormTypes.FormContact{
    return this.formBuilder.group({
      id:'00000000-0000-0000-0000-000000000000',
      attention:'',
      cc:'',
      email:'',
      fax:'',
      name:'',
      phone:'',
      mobile:'',
      address:this.generateAddress(),
      contactType:0,
      homePage:'',
      userId:'',
      isVisible:[true],
      password:'',
      belongedTo: ObjectTypes.Undefined
    })
  }

  initializeInspectionForm(order:Order):void{
     let currentOrder:Order = order;
    let currentOrderContacts;
    let currentProjectContacts;


     currentOrderContacts = this.currentAppState.vendorBasicInfo().orderContacts?.filter((ordercontact)=>ordercontact.orderId==currentOrder.id);

     const idsToExclude = new Set(this.currentOrderExceptionContacts.map(item => item.id));

     currentProjectContacts = this.currentAppState.vendorBasicInfo().projectContacts?.filter((projectContact)=>projectContact.projectId==this.currentProject.id && !idsToExclude.has(projectContact.id));

       this.currentContacts = currentOrderContacts.map((ordercontact)=>ordercontact.contact)
       this.currentContacts = this.currentContacts.concat(currentProjectContacts.map((projectcontact)=>projectcontact.contact));

        let currentOrderInspectionSubjects = this.currentAppState.vendorBasicInfo().orderInspectionSubjects?.filter((orderInspectionSubject)=>orderInspectionSubject.orderId == currentOrder.id);
        this.currentInspectionSubjects = currentOrderInspectionSubjects//this.currentAppState.vendorBasicInfo().orderInspectionSubjects?.filter((orderInspectionSubject)=>orderInspectionSubject.orderId == currentOrder.id)
        .map((orderInspectionSubject)=>orderInspectionSubject.inspectionSubject);

         this.currentInspectionTypes = this.currentVendorStore.inspectionTypes();
      //   this.currentInspectionSubjects = this.currentVendorStore.inspectionSubjects();

  }

  CheckIfInspectionIsActiveForCurrentUser(circulations: Circulation[]): boolean{

    let currentUserId = this.currentAppState.currentuser().id
    for(let i=0; i<circulations.length;i++)
    {
      if( circulations[i].performerContactPerson.userId ==currentUserId && circulations[i].performedActionDateGregorian == null && circulations[i].actionType == null)
      {
        this.currentCirculation = circulations[i];
        return true;
      }
    }
    return false;

    // let sortedCirculations = circulations.sort((a,b)=>{
    //   const dateA = new Date(a.createdDate);
    //   const dateB = new Date(b.createdDate);
    //   return dateA.getTime() - dateB.getTime();
    //   });


    // let lastIndexOfCirculations = circulations.length-1;
    // if(sortedCirculations.length == 1 && sortedCirculations[0].performerContactPerson.userId == this.currentAppState.currentuser().id && sortedCirculations[0].actionType.code ==0)
    //   return true;
    // else if(sortedCirculations[lastIndexOfCirculations].performedActionDateGregorian != null && sortedCirculations[lastIndexOfCirculations].performedActionDateGregorian != undefined )
    //   return false;
    // else
    // {
    //   if(sortedCirculations[lastIndexOfCirculations].performerContactPerson.name == this.currentAppState.currentuser().userName)
    //     return true;
    //   else
    //     return false;
    // }
  }
generateInspectionFromValue(inspection :Inspection):void
{


  //first should initialize form based on inspection order

this.initializeInspectionForm(inspection.order);

  this.inspectionForm =  this.formBuilder.group({

    id:inspection.id,
    attachments: this.formBuilder.array<FormTypes.FormAttachment>([this.generateAttachment()]),
    contacts : this.formBuilder.array<FormTypes.FormContact>([this.generateContact()]),
    location: inspection!.location != undefined ? this.generateLocationFromValue(inspection!.location!) : this.generateLocation(),
    order:inspection.order !=undefined ? this.generateOrderFromValue(inspection.order) : this.generateOrder(),

    //owner:this.generateOwner(),
    pidecContacts:this.formBuilder.array<FormTypes.FormContact>([this.generateContact()]),
    pidecInspectors : this.formBuilder.array<FormTypes.FormContact>([this.generateContact()]),
    thirdPartInspectors: this.formBuilder.array<FormTypes.FormContact>([this.generateContact()]),
    //vendor:inspection.vendor !=undefined ? this.generateVendorFromValue(inspection.vendor): this.generateVendor(),
    ownerContactPerson: this.generateContact(),
    mcContactPerson: this.generateContact(),
    supplierContactPerson:this.generateContact(),
    subSupplierContactPerson:this.generateContact(),


    notificationNumber:inspection.notificationNumber,
    ownerDocNo:'',
    pidecDocNo:'',
    supplier:'',
    sub_supplier:'',
    workProgress:'',

    CreatedBy: inspection.createdBy,
    ModifiedBy: inspection.modifiedBy,
    CreatedDate:inspection.createdDate,
    ModifiedDate :inspection.modifiedDate,
    issueDate:inspection.issueDate,
    persianIssueDate: inspection.persianIssueDate,

    RequestInspectionDateEnd:inspection.requestInspectionDateEnd,
    RequestInspectionDateStart:inspection.requestInspectionDateStart,

    RequestInspectionDueDate: inspection.requestInspectionDueDate,
    RequestInspectionPeriodDays: inspection.requestInspectionPeriodDays,
    RequestInspectionTimesInPeriod: inspection.requestInspectionPeriodDays,

    RequestInspectionPersianDateEnd: inspection.requestInspectionPersianDateEnd,
    RequestInspectionPersianDateStart: inspection.requestInspectionPersianDateStart,


    inspectionSubjects:this.formBuilder.array<FormTypes.FormInspectionSubject>([this.generateInspectionSubject()]),//this.generateInspectionSubjectsFormArrayFromValue(inspection.inspectionSubjects),
    inspectionTypes: this.formBuilder.array<FormTypes.FormInspectionType>([this.generateInspectionType()]),//this.generateInspectionTypesFormArrayFromValue(inspection.inspectionTypes)

  });


 if(this.CheckIfInspectionIsActiveForCurrentUser(inspection.circulations) == true)
 {
    this.editMode = true;
    this.toggleEditMode(true);
  //   this.inspectionForm.patchValue({

  // })
 }
 else{
  this.editMode = false;
   this.toggleEditMode(false);
 }
      inspection.contacts.forEach((relContact)=>{
          this.contactsFormArray.push(this.generateContactFromContactValue(relContact));
        });

         if(this.currentInspectionTypes !=null && this.currentInspectionTypes !=undefined)
      {
          this.currentInspectionTypes.forEach((inspectionType)=>

            this.inspectionTypesFormArray.push(this.formBuilder.group({
              id:[inspectionType.id],
              name:[inspectionType.name],
              selected:[false]
              }))
          );}

       if(this.currentInspectionSubjects != null && this.currentInspectionSubjects != undefined)
       {
        this.currentInspectionSubjects.forEach((subject)=>
        this.inspectionSubjectsFormArray.push(
          this.formBuilder.group({
            name:[subject.name],
            id:[subject.id],
            selected : [false]
          })
       ));
      }

      this.generateInspectionSubjectsFormArrayFromValue(inspection.inspectionSubjects);
      this.generateInspectionTypesFormArrayFromValue(inspection.inspectionTypes);
}

generateInspectionSubjectsFormArrayFromValue(inspectionSubjects: InspectionSubject[]):void//FormArray<FormTypes.FormInspectionSubject>
{

  inspectionSubjects!.forEach(inspectionSubject=>
  {
    const index = this.inspectionSubjectsFormArray.value.findIndex(
    (item: FormTypes.FormInspectionSubject) => item?.['id'] === inspectionSubject.id
    );
    if(index != -1)
      this.inspectionSubjectsFormArray.at(index).patchValue({
        selected:true
      });
    else
      this.inspectionSubjectsFormArray.push(this.generateInspectionSubjectFromValue(inspectionSubject));
  }
  );


  // let a : FormArray<FormTypes.FormInspectionSubject>;
  // if(inspectionSubjects != undefined)
  //   inspectionSubjects!.forEach(inspectionSubject=>a.push(this.generateInspectionSubjectFromValue(inspectionSubject)));

  // if(a == undefined  || a==null)
  //   return this.formBuilder.array<FormTypes.FormInspectionSubject>([this.generateInspectionSubject()])
  //  return a;
}

generateInspectionTypesFormArrayFromValue(inspectionTypes: InspectionType[]):void//FormArray<FormTypes.FormInspectionType>
{

   inspectionTypes!.forEach(inspectionType=>
  {

      const index = this.inspectionTypesFormArray.value.findIndex(
    (item: FormTypes.FormInspectionType) => item?.['id'] === inspectionType.id
    );
    if(index != -1)
      this.inspectionTypesFormArray.at(index).patchValue({
        selected:true
      });
    else
      this.inspectionTypesFormArray.push(this.generateInspectionTypeFromValue(inspectionType));

  }
  );

}
  generateInspectionTypeFromValue(inspectionType:InspectionType):FormTypes.FormInspectionType
  {
return this.formBuilder.group({

  id:inspectionType.id,
  name:inspectionType.name,
  selected:inspectionType.selected?? false
});
  }


  generateInspectionSubjectFromValue(inspectionSubject:InspectionSubject):FormTypes.FormInspectionSubject
  {
return this.formBuilder.group({

  id:inspectionSubject.id,
  name:inspectionSubject.name,
  selected:inspectionSubject.selected
});
  }
  generateAddress():FormTypes.FormAddress{
       return this.formBuilder.group({
        id:'00000000-0000-0000-0000-000000000000',
         city:'',
         country:'',
         state:'',
         streetLine1:'',
         streetLine2:'',
         zipCode:''
       })
     }

    generateAddressFromAddressValue(address:Address):FormTypes.FormAddress{
        return this.formBuilder.group({
          id:address.id,
          city:address.city,
          country:address.country,
          state:address.state,
          streetLine1:address.streetLine1,
          streetLine2:address.streetLine2,
          zipCode:address.zipCode
        })
      }

  generateContactFromContactValue(contact:Contact):FormTypes.FormContact{
    return this.formBuilder.group({
      id:contact.id,
      attention:contact.attention,
      cc:contact.cc,
      email:contact.email,
      fax:contact.fax,
      name:contact.name,
      phone:contact.phone,
      contactType:contact.contactType,
      mobile:contact.mobile,
      homePage:contact.homePage,
      address:this.generateAddressFromAddressValue(contact.address),
      userId:contact.userId,
      isVisible:[true],
      password:contact.password,
      belongedTo:contact.belongedTo
    })
  }
  generateLocation():FormTypes.FormLocation{
    return this.formBuilder.group({
      address:'',
      city:'',
      country:'',
      state:'',
      streetLine1:'',
      streetLine2:'',
      zipCode:''
    })
  }
  generateLocationFromValue(location:Address):FormTypes.FormLocation{
    return this.formBuilder.group({
      address:'',//location.state!,
      city:location.city!,
      country:location.country!,
      state:location.state!,
      streetLine1:location.streetLine1!,
      streetLine2:location.streetLine2!,
      zipCode:location.zipCode!
    })
  }

onDocPicked(event: any) {

  this.attachedDocs = event.target.files;
  this.formData.delete('attachments');

  for(var i = 0; i<this.attachedDocs.length;i++)
    this.formData.append('attachments',this.attachedDocs[i]);


  console.log(this.attachedDocs);
}
AttachDocument() {
throw new Error('Method not implemented.');

}
selectProject() {
throw new Error('Method not implemented.');
}
selectOwner() {
throw new Error('Method not implemented.');
}


get inspectionSubjectsFormArray(): FormArray {
  if(this.inspectionForm != null && this.inspectionForm != undefined)
  {
   if(this.inspectionForm.get('inspectionSubjects')== null && this.inspectionForm.get('inspectionSubjects')==undefined)
    this.inspectionForm.addControl('inspectionSubjects', new FormArray([]));
  return (<FormArray>this.inspectionForm.get('inspectionSubjects'));
  }
  return new FormArray([]);
}

get filteredInspectionSubjectsFormArray() : FormGroup[]  {

     return ((this.inspectionForm?.get('inspectionSubjects') as FormArray)?.controls
      ?.filter(ctrl => ctrl.get('name')?.value?.toString().trim() !== '') || []) as FormGroup[];
   }
get inspectionTypesFormArray(): FormArray {
  if(this.inspectionForm != null && this.inspectionForm != undefined)
  {
  if(this.inspectionForm.get('inspectionTypes')== null && this.inspectionForm.get('inspectionTypes')==undefined)
    this.inspectionForm.addControl('inspectionTypes', new FormArray([]));
  //return this.inspectionForm.get('inspectionTypes') as FormArray;
  return (<FormArray>this.inspectionForm.get('inspectionTypes'));
  }
 return new FormArray([]);
}

get filteredInspectionTypesFormArray() :FormGroup[] {

    return ((this.inspectionForm?.get('inspectionTypes') as FormArray)?.controls
    ?.filter(ctrl => ctrl.get('name')?.value?.toString().trim() !== '') || []) as FormGroup[];
}

get filteredContactsFormArray():FormGroup[]{
   return ((this.inspectionForm?.get('contacts') as FormArray)?.controls
    ?.filter(ctrl => ctrl.get('email')?.value?.toString().trim() !== '' || ctrl.get('id')?.value?.toString().trim() !== '00000000-0000-0000-0000-000000000000') || []) as FormGroup[];

}
get contactsFormArray() :FormArray<FormTypes.FormContact> {

   if(this.inspectionForm.get('contacts')== null && this.inspectionForm.get('contacts')==undefined)
    this.inspectionForm.addControl('contacts', new FormArray([]));
    return this.inspectionForm?.get('contacts') as FormArray ?? new FormArray([]);

}

get pidecContactsFormArray() :FormGroup[]{
let a = this.contactsFormArray?.controls
  ?.filter(ctrl => ctrl.get('contactType')?.value?.toString().trim() === ContactTypes.QC_Manager.toString().trim()
  || ctrl.get('contactType')?.value?.toString().trim() === ContactTypes.QC_Lead.toString().trim()) as FormGroup[];
if(a==undefined || a==null || a.length==0)
{
  let pc = this.generateContact();
  pc.get('contactType').setValue(ContactTypes.QC_Manager);
    this.contactsFormArray.push(pc);

}
    return a;
}

get mcContact() :FormTypes.FormContact | undefined{
  return (this.inspectionForm?.controls?.mcContactPerson as FormTypes.FormContact) ;
}

get ownerContact() :FormTypes.FormContact | undefined{
  return (this.inspectionForm?.controls?.ownerContactPerson as FormTypes.FormContact) ;
}



 /**
   * Get Persian month name by month number (1-12)
   */
  getPersianMonthName(monthNumber: number): string {
    return this.persianMonthNames[monthNumber - 1];
  }
// Get Gregorian month name (0-11)
getGregorianMonthName(monthIndex: number): string {
  return this.gregorianMonthNames[monthIndex];
}

 setPersianIssueDate(event: any) {
  this.isManualUpdateIssueDate = true;

    try {
      // Get the selected Gregorian date
      const gregorianDate = new Date(event.value);

      // Format Gregorian date with month name
      const gregorianDay = gregorianDate.getDate();
      const gregorianMonth = gregorianDate.getMonth();
      const gregorianYear = gregorianDate.getFullYear();
      const gregorianMonthName = this.getGregorianMonthName(gregorianMonth);

      // Set formatted Gregorian date with month name
      this.formattedGregorianIssueDate = `${gregorianDay} ${gregorianMonthName} ${gregorianYear}`;

      // Convert to Persian
      const { jy, jm, jd } = toJalaali(gregorianYear, gregorianMonth + 1, gregorianDay);

      // Format Persian date with numeric representation for the input
      const numericPersianDate = `${jy}/${String(jm).padStart(2, '0')}/${String(jd).padStart(2, '0')}`;

      // Format Persian date with month name for display
      const persianMonthName = this.getPersianMonthName(jm);
      this.formattedPersianIssueDate = `${jd} ${persianMonthName} ${jy}`;

      // Update Persian date in form
      //this.inspectionForm?.controls?.persianIssueDate?.patchValue(numericPersianDate);
      this.inspectionForm.get('persianIssueDate').setValue(numericPersianDate);

      this.toastr.success('Dates synchronized successfully');
    } catch (error) {
      console.error('Error in setPersianIssueDate:', error);
      this.toastr.error('Error updating Persian date');
    } finally {
      this.cdr.detectChanges();
      // Reset flag after a short delay
      setTimeout(() => {
        this.isManualUpdateIssueDate = false;
      }, 100);
    }
  }


  onIssueDateSelect(event: any): void {
    if (this.isManualUpdateIssueDate) {
      // Prevent event handler from running if the update is manual
      this.isManualUpdateIssueDate = false;
      return;
    }

    try {
      // Get Gregorian date from the event
      const viaGregorianDate = new Date(event.gregorian);

      // Update Gregorian date in form
      this.isManualUpdateIssueDate = true;
      //this.inspectionForm.get('issueDate').setValue(viaGregorianDate);

      this.inspectionForm?.controls?.issueDate?.patchValue(viaGregorianDate);
      // Format Gregorian date with month name
      const gregorianDay = viaGregorianDate.getDate();
      const gregorianMonth = viaGregorianDate.getMonth();
      const gregorianYear = viaGregorianDate.getFullYear();
      const gregorianMonthName = this.getGregorianMonthName(gregorianMonth);

      // Set formatted Gregorian date with month name
      this.formattedGregorianIssueDate = `${gregorianDay} ${gregorianMonthName} ${gregorianYear}`;

      // Reset flag after a short delay
      setTimeout(() => {
        this.isManualUpdateIssueDate = false;
      }, 100);
    } catch (error) {
      console.error('Error in onIssueDateSelect:', error);
      this.toastr.error('Error updating Gregorian date');
    }
  }

//---------------End For Methods For IssueDate and PersianIssueDate-------------------

//-----------------Methods For RequestInspectionPersianDateStart and RequestInspectionDateStart-------------------
onRequestInspectionDateStartSelect(event: any): void {

     if (this.isManualUpdateRequestInspectionDateStart) {
      // Prevent event handler from running if the update is manual
      this.isManualUpdateRequestInspectionDateStart = false;
      return;
    }

    try {
      // Get Gregorian date from the event
      const viaGregorianDate = new Date(event.gregorian);

      // Update Gregorian date in form
      this.isManualUpdateRequestInspectionDateStart = true;
       this.inspectionForm?.controls?.RequestInspectionDateStart?.patchValue(viaGregorianDate);

      // Format Gregorian date with month name
      const gregorianDay = viaGregorianDate.getDate();
      const gregorianMonth = viaGregorianDate.getMonth();
      const gregorianYear = viaGregorianDate.getFullYear();
      const gregorianMonthName = this.getGregorianMonthName(gregorianMonth);

      // Set formatted Gregorian date with month name
      this.formattedGregorianReqStartDate = `${gregorianDay} ${gregorianMonthName} ${gregorianYear}`;
      this.CalculateInspectionDuration();
      // Reset flag after a short delay
      setTimeout(() => {
        this.isManualUpdateRequestInspectionDateStart = false;
      }, 100);
    } catch (error) {
      console.error('Error in onIssueDateSelect:', error);
      this.toastr.error('Error updating Gregorian date');
    }

  }

  setRequestInspectionPersianDateStart(event: any) {
    this.isManualUpdateRequestInspectionDateStart = true;

    try {
      // Get the selected Gregorian date
      const gregorianDate = new Date(event.value);

      // Format Gregorian date with month name
      const gregorianDay = gregorianDate.getDate();
      const gregorianMonth = gregorianDate.getMonth();
      const gregorianYear = gregorianDate.getFullYear();
      const gregorianMonthName = this.getGregorianMonthName(gregorianMonth);

      // Set formatted Gregorian date with month name
      this.formattedGregorianReqStartDate = `${gregorianDay} ${gregorianMonthName} ${gregorianYear}`;

      // Convert to Persian
      const { jy, jm, jd } = toJalaali(gregorianYear, gregorianMonth + 1, gregorianDay);

      // Format Persian date with numeric representation for the input
      const numericPersianDate = `${jy}/${String(jm).padStart(2, '0')}/${String(jd).padStart(2, '0')}`;

      // Format Persian date with month name for display
      const persianMonthName = this.getPersianMonthName(jm);
      this.formattedPersianReqStartDate = `${jd} ${persianMonthName} ${jy}`;

      // Update Persian date in form
      this.inspectionForm.get('RequestInspectionPersianDateStart').setValue(numericPersianDate);

      this.toastr.success('Dates synchronized successfully');
    } catch (error) {
      console.error('Error in setPersianReqStartDate:', error);
      this.toastr.error('Error updating Persian date');
    } finally {
      this.cdr.detectChanges();
      // Reset flag after a short delay
      setTimeout(() => {
        this.isManualUpdateRequestInspectionDateStart = false;
      }, 100);
    }
  }
//--------------------------------------------------------------------------------------------------------------------

//-----------------Methods For RequestInspectionPersianDateStart and RequestInspectionDateStart-------------------
onRequestInspectionDateEndSelect(event: any): void {
if (this.isManualUpdateRequestInspectionDateEnd) {
      // Prevent event handler from running if the update is manual
      this.isManualUpdateRequestInspectionDateEnd = false;
      return;
    }

    try {
      // Get Gregorian date from the event
      const viaGregorianDate = new Date(event.gregorian);

      // Update Gregorian date in form
      this.isManualUpdateRequestInspectionDateEnd = true;
       this.inspectionForm?.controls?.RequestInspectionDateEnd?.patchValue(viaGregorianDate);

      // Format Gregorian date with month name
      const gregorianDay = viaGregorianDate.getDate();
      const gregorianMonth = viaGregorianDate.getMonth();
      const gregorianYear = viaGregorianDate.getFullYear();
      const gregorianMonthName = this.getGregorianMonthName(gregorianMonth);

      // Set formatted Gregorian date with month name
      this.formattedGregorianReqEndDate = `${gregorianDay} ${gregorianMonthName} ${gregorianYear}`;

      this.CalculateInspectionDuration();

      // Reset flag after a short delay
      setTimeout(() => {
        this.isManualUpdateRequestInspectionDateEnd = false;
      }, 100);
    } catch (error) {
      console.error('Error in onReqEndDateSelect:', error);
      this.toastr.error('Error updating Gregorian date');
    }
}

setRequestInspectionPersianDateEnd(event: any) {
  this.isManualUpdateRequestInspectionDateEnd = true;

    try {
      // Get the selected Gregorian date
      const gregorianDate = new Date(event.value);

      // Format Gregorian date with month name
      const gregorianDay = gregorianDate.getDate();
      const gregorianMonth = gregorianDate.getMonth();
      const gregorianYear = gregorianDate.getFullYear();
      const gregorianMonthName = this.getGregorianMonthName(gregorianMonth);

      // Set formatted Gregorian date with month name
      this.formattedGregorianReqEndDate = `${gregorianDay} ${gregorianMonthName} ${gregorianYear}`;

      // Convert to Persian
      const { jy, jm, jd } = toJalaali(gregorianYear, gregorianMonth + 1, gregorianDay);

      // Format Persian date with numeric representation for the input
      const numericPersianDate = `${jy}/${String(jm).padStart(2, '0')}/${String(jd).padStart(2, '0')}`;

      // Format Persian date with month name for display
      const persianMonthName = this.getPersianMonthName(jm);
      this.formattedPersianReqEndDate = `${jd} ${persianMonthName} ${jy}`;

      // Update Persian date in form
      this.inspectionForm.get('RequestInspectionPersianDateEnd').setValue(numericPersianDate);

      this.toastr.success('Dates synchronized successfully');
    } catch (error) {
      console.error('Error in setPersianReqEndDate:', error);
      this.toastr.error('Error updating Persian date');
    } finally {
      this.cdr.detectChanges();
      // Reset flag after a short delay
      setTimeout(() => {
        this.isManualUpdateRequestInspectionDateEnd = false;
      }, 100);
    }
}
//--------------------------------------------------------------------------------------------------------------------

CalculateInspectionDuration():void{

  let startDate = this.inspectionForm.controls.RequestInspectionDateStart.value;
  let endDate = this.inspectionForm.controls.RequestInspectionDateEnd.value;
//this.inspectionForm.controls.RequestInspectionDueDate
 const start = new Date(startDate).setHours(0, 0, 0, 0);
  const end = new Date(endDate).setHours(0, 0, 0, 0);

  const msInDay = 1000 * 60 * 60 * 24;

  this.inspectionForm.controls.RequestInspectionDueDate.setValue( Math.floor((end - start) / msInDay));
}

  formatPersianDate(year: number, month: number, day: number): string {
    const monthName = this.getPersianMonthName(month);
    return `${year} ${monthName} ${('0' + day).slice(-2)}`;
  }

  SaveNewInspection():void
  {
    console.log("in save :", this.inspectionForm.getRawValue());
    const rawFormValue = this.inspectionForm.getRawValue();
    rawFormValue.contacts = rawFormValue.contacts.concat(rawFormValue.ownerContactPerson);
    rawFormValue.contacts = rawFormValue.contacts.concat(rawFormValue.mcContactPerson);
    rawFormValue.contacts = rawFormValue.contacts.concat(rawFormValue.pidecInspectors);
    rawFormValue.contacts = rawFormValue.contacts.concat(rawFormValue.thirdPartInspectors);

      const filteredInspectionForm = {
      ...rawFormValue,

       createdDate : rawFormValue.CreatedDate ?? new Date(Date.now()),
      createdBy:rawFormValue.CreatedBy ?? this.currentAppState.currentuser().userName,

      modifiedDate:new Date(Date.now()),
      modifiedBy:this.currentAppState.currentuser().userName,

      inspectionSubjects: this.inspectionForm
        .get('inspectionSubjects')
        .value.filter((subject) => subject.selected && subject.name !='') as InspectionSubject[]
        ,
        inspectionTypes: this.inspectionForm
          .get('inspectionTypes')
          .value.filter((inspetionType) => inspetionType.selected && inspetionType.name !='') as InspectionType[]// Keep only selected subjects
        ,
        contacts:rawFormValue.contacts
        //this.inspectionForm.get('contacts').value
        .filter((contact) => contact.email !='' && contact.contactType !=ContactTypes.Undefined) as Contact[]
          ,
        attachments:this.inspectionForm
          .get('attachments')
          .value.filter((attachment) => attachment.name !='' && attachment.content.length !=0) as Attachment[]
        //    ,
        //  circulations: [this.currentCirculation]
    };


     this.formData.delete('Inspection');
     this.formData.delete('Project');
     this.formData.delete('currentCirculation');
    this.formData.append('Inspection',JSON.stringify(filteredInspectionForm));
    this.formData.append('Project',JSON.stringify(this.Project ))
    this.formData.append('currentCirculation',JSON.stringify(this.currentCirculation));

    this.inspectionService.saveNewInspectionFull(this.formData).subscribe((sentInspection)=>{
      this.editMode = false;
      this.toggleEditMode(false);
      this.inspectionForm.patchValue({
        ...this.inspectionForm.getRawValue(),
        id:sentInspection.id,
        contacts:sentInspection.contacts,
        inspectionSubjects: sentInspection.inspectionSubjects,
        inspectionTypes:sentInspection.inspectionTypes,
        attachments :sentInspection.attachments
      })
    });
  }

  DraftNewInspection():void
  {
    console.log("in draft :", this.inspectionForm.getRawValue());

    const filteredInspectionForm = {
      ...this.inspectionForm.getRawValue(),

      createdDate : Date.now(),
      createdBy:this.currentAppState.currentuser().userName,
      modifiedDate:Date.now(),
      modifiedBy:this.currentAppState.currentuser().userName,

      inspectionSubjects: this.inspectionForm
      .get('inspectionSubjects')
        .value.filter((subject) => subject.selected && subject.name !='') as InspectionSubject[]
        ,
        inspectionTypes: this.inspectionForm
          .get('inspectionTypes')
          .value.filter((inspetionType) => inspetionType.selected && inspetionType.name !='') as InspectionType[]// Keep only selected subjects
        ,
        contacts:this.inspectionForm
          .get('contacts')
          .value.filter((contact) => contact.email !='' && contact.contactType !=ContactTypes.Undefined) as Contact[]
          ,
        attachments:this.inspectionForm
          .get('attachments')
          .value.filter((attachment) => attachment.name !='' && attachment.content.length !=0) as Attachment[]
    };

    this.formData.delete('Inspection');
    this.formData.append('Inspection',JSON.stringify(filteredInspectionForm));

    this.inspectionService.draftNewInspectionFull(this.formData).subscribe((savedNewInspection : Inspection)=>{

    const insTypesformArray = this.inspectionForm?.get('inspectionTypes') as FormArray;

if (insTypesformArray) {
  // Loop backward to avoid index shifting issues while removing
  for (let i = insTypesformArray.length - 1; i >= 0; i--) {
    const control = insTypesformArray.at(i);
    const id = control?.get('id')?.value?.toString().trim();
    if (id == undefined ||  id === '00000000-0000-0000-0000-000000000000') {
      insTypesformArray.removeAt(i);
    }
  }
}


let findedInsType = null;
    for(let j= 0; j<savedNewInspection.inspectionTypes.length;j++)
    {
      findedInsType =  insTypesformArray.controls?.find((insType)=>insType.get('id').value?.toString().trim()===savedNewInspection.inspectionTypes.at(j).id);
      if(findedInsType  != null)
        findedInsType.selected = true;
      else
      {
        savedNewInspection.inspectionTypes.at(j).selected = true;
        this.inspectionTypesFormArray.push(savedNewInspection.inspectionTypes.at(j));
      }

    }

    const insSubjectsformArray = this.inspectionForm?.get('inspectionSubjects') as FormArray;

if (insSubjectsformArray) {
  // Loop backward to avoid index shifting issues while removing
  for (let i = insSubjectsformArray.length - 1; i >= 0; i--) {
    const control = insSubjectsformArray.at(i);
    const id = control?.get('id')?.value?.toString().trim();
    if (id == undefined ||  id === '00000000-0000-0000-0000-000000000000') {
      insSubjectsformArray.removeAt(i);
    }
  }
}


let findedInsSubject = null;
    for(let j= 0; j<savedNewInspection.inspectionSubjects.length;j++)
    {
      findedInsSubject =  insTypesformArray.controls?.find((insSubject)=>insSubject.get('id').value?.toString().trim()===savedNewInspection.inspectionSubjects.at(j).id);
      if(findedInsSubject  != null)
        findedInsSubject.selected = true;
      else
      {
        savedNewInspection.inspectionSubjects.at(j).selected = true;
        this.inspectionSubjectsFormArray.push(savedNewInspection.inspectionSubjects.at(j));
      }

    }
      this.inspectionForm.patchValue({
        ...this.inspectionForm.getRawValue(),
        id:savedNewInspection.id,
        contacts:savedNewInspection.contacts,
       // inspectionSubjects: savedNewInspection.inspectionSubjects,
       // inspectionTypes:savedNewInspection.inspectionTypes,
        attachments :savedNewInspection.attachments
      })
  });

  }

}
