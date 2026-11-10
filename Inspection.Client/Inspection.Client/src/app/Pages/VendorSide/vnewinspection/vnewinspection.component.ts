import { Address } from './../../../Core/Models/Address.model';

import { Contact } from './../../../Core/Models/Contact.model';
import { ActionType } from './../../../Core/Models/ActionType.model';
import { Revision } from './../../../Core/Models/Revision.model';
import { InspectionSubject } from './../../../Core/Models/InspectionSubject.model';
import { InspectionForm, FormContact } from './../../../Core/FormTypes/InspectionFormTypes';
import { NewNotificationNumberQueryParams } from './../../../Core/Services/inspection.service';
import { Circulation } from './../../../Core/Models/Circulation.model';
import { qCMembersbasicinfoResolver } from './../../../Core/Resolvers/qcmembersbasicinfo.resolver';
import { OrderSettingsException } from './../../../Core/Models/OrderSettingsException.model';
import { Project } from './../../../Core/Models/Project.model';
import { join } from 'node:path';
import { Owner } from '../../../Core/Models/Owner.model';
import { ChangeDetectorRef, Component, effect, Inject, inject, Injector, OnInit, runInInjectionContext, ViewEncapsulation, AfterViewInit, ViewChild } from '@angular/core';
import { Inspection } from '../../../Core/Models/Inspection.model';
import { InspectionService, queryNewNotificationNumberResult } from '../../../Core/Services/inspection.service';
import { Order } from '../../../Core/Models/Order.model';
import { AbstractControl, Form, FormArray, FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule, formatDate } from '@angular/common';
import { UploadFilesComponent } from '../../../Core/General/Components/upload-files/upload-files.component';
import { MatDatepicker, MatDatepickerInputEvent, MatDatepickerModule} from '@angular/material/datepicker';
import { MatDatepickerModulePersian } from 'ngx-persian-datepicker';
import { MAT_DATE_FORMATS, MatDateFormats, MatNativeDateModule } from '@angular/material/core';
import { toGregorian, toJalaali } from 'jalaali-js';
import { IsActiveForUserPipe } from '../../../Core/General/Pipes/is-active-for-user.pipe';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import {MatChipsModule} from '@angular/material/chips';
import {MatButtonModule} from '@angular/material/button';
import {MatInputModule} from '@angular/material/input';
import {MatAutocompleteModule} from '@angular/material/autocomplete';
import {MatSelectModule} from '@angular/material/select';
import {MatCheckboxModule} from '@angular/material/checkbox';
//import {GregorianDateWithMonthNamePipe} from '../../../Core/General/Pipes/gregorian-date-with-month-name.pipe';


import { IDatepickerTheme, IActiveDate, NgPersianDatepickerModule } from 'ng-persian-datepicker';
 import { darkTheme} from '../../../Themes/darkTheme';
import {
  MatDialog,
  MatDialogModule
} from '@angular/material/dialog';
import { UserService } from '../../../Core/Services/user.service';
import { Vendor } from '../../../Core/Models/Vendor.model';
import { InspectionType } from '../../../Core/Models/InspectionType.model';
import { ActivatedRoute, Router } from '@angular/router';
import { map } from 'rxjs';
import { BasicVendorInfo } from '../../../Core/Models/BasicVendorInfo.model';
import { Toast, ToastrService } from 'ngx-toastr';
import { currentstateStore } from '../../../Stores/currentstate.store';
import { vendorInfoStore } from '../../../Stores/vendorInfo.store';
import * as FormTypes from '../../../Core/FormTypes/InspectionFormTypes';
import { ContactTypes } from '../../../Core/Models/ContactTypes.enum';
import { UserTypes } from '../../../Core/Models/UserTypes.enum';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatTooltipModule } from '@angular/material/tooltip';
import { Attachment } from '../../../Core/Models/Attachment.model';
import { userInfo } from 'node:os';
import { ObjectTypes } from '../../../Core/Models/ObjectTypes.enum';
import { ProjectContact } from '../../../Core/Models/ProjectContact.model';
import { InspectionreportComponent } from '../../InspectorSide/inspectionreport/inspectionreport.component';
import { InspectionReportService } from '../../../Core/Services/inspectionReport.service';
import { InspectionReport } from '../../../Core/Models/InspectionReport.model';
import { User } from '../../../Core/Models/User.model';
import { environment } from '../../../../environments/environments';

import { inspect, isUndefined } from 'node:util';
import { CirculationModalComponent } from "../circulation-modal/circulation-modal.component";
import { WorkflowServiceService } from '../../../Core/WorkflowEngine/workflow-service.service';
import { InspectionWorkflowViewModel } from '../../../Core/WorkflowEngine/InspectionWorkflowViewModel.model';
import { CirculationVM } from '../../../Core/Models/CirculationVM.model';
import { DocumentManagerDialogComponent } from '../../../Shared/document-manager-dialog/document-manager-dialog.component';
import { DocumentOwnerType } from '../../../Core/Models/Document/DocumentOwnerType.model';
import { OrderInspectionSubject } from '../../../Core/Models/OrderInspectionSubject.model';
import { InspectorService } from '../../../Core/Services/inspector.service'; // TODO: fix path to your real file
import { Inspector, SkillOption } from '../../../Core/Models/Inspector.model'; // TODO: fix path to your real file
import { InspectorQuickAddData, InspectorQuickAddDialogComponent } from '../inspector-quick-add-dialog/inspector-quick-add-dialog.component';





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


export interface CirculationViewModel {
    original: Circulation;
    current: Circulation;
}

@Component({
    selector: 'app-newinspection',
    imports: [CommonModule, ReactiveFormsModule, FormsModule, MatChipsModule, MatButtonModule, MatIconModule, MatCardModule, IsActiveForUserPipe, NgPersianDatepickerModule, MatDatepickerModule, MatFormFieldModule, MatTooltipModule, MatInputModule, MatAutocompleteModule, MatSelectModule, MatCheckboxModule,MatDialogModule],
    templateUrl:'./vnewinspection.component.html',
    styleUrl: './vnewinspection.component.css',
    encapsulation: ViewEncapsulation.None,
    providers: [
      { provide: MAT_DATE_FORMATS, useValue: MY_FORMATS }, // <-- This line sets the custom date format for mat-datepicker
    ]
})
export class VnewinspectionComponent implements OnInit,AfterViewInit{


  initialInspectionInfo: queryNewNotificationNumberResult;
  fgNewInspection !:FormGroup;
  newInspection !:Inspection ;
  attachedDocs: File[];
  formData : FormData;
  circulationData: FormData;
  internalCorrespondenceFormData:FormData;
  requestCirculations:Circulation[];
  requestCirculationsVM: CirculationViewModel[];
  allInternalCorrespondences:Circulation[];
  allCirculationsInCurrentRevision :Circulation[];
  vendorCirculation : Circulation;
  QC_Circulations : Circulation[];
  readonly dialog = inject(MatDialog);
  injector = inject(Injector);
  Project:Project;
  currentProject? : Project;
  Orders:Order[];
  currentVendorData:Vendor;
  inspectionForm : FormTypes.InspectionForm ;
  private currentContacts : Contact[];
  currentInspectionSubjects :InspectionSubject[];
  currentInspectionTypes:InspectionType[];
  editMode :boolean = true;
  attachments: Attachment[] = [];
  inspectionReports : InspectionReport[];
  currentUserId : string = '';
  currentUser : User;
  QC_AlreadyHasAction : boolean= false;
  IsNewRequest :boolean = true;
  currentRevision : Revision;
  ShowQCComment :boolean=false;

  workflow :InspectionWorkflowViewModel | null = null;
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
currentCirculation : Circulation = null;
orderSettingsExceptions:OrderSettingsException[];
currentOrderExceptionContacts: ProjectContact[];
currentUserRole: UserTypes = UserTypes.UndefinedUser;
// control for combo box input
  subjectCtrl = new FormControl('');
  itpRowNoCtrl = new FormControl('');
allSubjects: string[] = [];
disableAddButton :boolean=false;
inspectionActions: string[] = [];
RequestId :string ="";
InternalReceiversContacts:Contact[]= [];

// --- Inspector picker state (pidecInspectors / thirdPartInspectors rows) ---
// Approved inspectors already loaded for a given inspection-company Contact id.
private inspectorsByCompanyContact = new Map<string, Inspector[]>();
// The in-progress search text typed into each row's autocomplete, keyed by that row's FormGroup.
private inspectorSearchTerms = new Map<AbstractControl, string>();


orderInspectionSubjects : OrderInspectionSubject[];

// Add this to your component
private gregorianMonthNames = [
  'January', 'February', 'March', 'April', 'May', 'June',
  'July', 'August', 'September', 'October', 'November', 'December'
];

  finglishpersianMonthNames = [
    'Farvardin', 'Ordibehesht', 'Khordad', 'Tir', 'Mordad', 'Shahrivar',
    'Mehr', 'Aban', 'Azar', 'Dey', 'Bahman', 'Esfand'
  ];

   persianMonthNames = [
    'فروردین', 'اردیبهشت', 'خرداد', 'تیر', 'مرداد', 'شهریور',
    'مهر', 'آبان', 'آذر', 'دی', 'بهمن', 'اسفند'
  ];
    formattedGregorianIssueDate: string = '';
  formattedPersianIssueDate: string = '';


  formattedGregorianReqStartDate: string = '';
  formattedPersianReqStartDate: string = 'QWEQW';


  formattedGregorianReqEndDate: string = '';
  formattedPersianReqEndDate: string = '';

  isInspector:boolean = false;
  isConfirmedByQC : boolean = false;

  constructor(private router: Router,private workflowService: WorkflowServiceService,private activatedRoute: ActivatedRoute,private inspectionReportService:InspectionReportService,private inspectionService:InspectionService,private inspectorService:InspectorService,private formBuilder:FormBuilder,private toastr:ToastrService,private cdr: ChangeDetectorRef) {

    this.formData = new FormData();
    this.circulationData = new FormData();

    this.currentProject = this.currentAppState.currentproject();
    this.currentUser = this.currentAppState.currentuser();

    this.currentUserRole = this.currentUser.userType;
    this.currentUserId = this.currentUser.id;

    this.currentVendorData = this.currentAppState.vendorData();

    if(this.currentAppState.currentuser().userType == UserTypes.Vendor)
    {

       this.orderSettingsExceptions = this.currentAppState.vendorBasicInfo().orderSettingsExceptions;
       let vendorBasicInfo = this.currentAppState.vendorBasicInfo();
       this.currentInspectionTypes = vendorBasicInfo.inspectionTypes;
       this.currentInspectionSubjects = vendorBasicInfo.inspectionSubjects;
      // this.orderInspectionSubjects =this.currentAppState.vendorBasicInfo().orderInspectionSubjects;



      this.Orders = this.currentAppState.vendorBasicInfo().orders;//this.currentVendorData['orders'];

    }
    else if(this.currentAppState.currentuser().userType == UserTypes.PIDEC_TO || this.currentAppState.currentuser().userType == UserTypes.PIDEC_CC)
    {
      let qCUserBasicInfo = this.currentAppState.qCMemberBasicInfo();

      this.currentInspectionTypes = qCUserBasicInfo.inspectionTypes;
     //  this.orderInspectionSubjects =qCUserBasicInfo.orderInspectionSubjects;

    }
    else if(this.currentAppState.currentuser().userType == UserTypes.PIDEC_Inspector)
    {

      let inspectorBasicInfo = this.currentAppState.inspectorBasicInfo();
       this.isInspector = true;
    }

  }



 BuildInternalCorrespondenceReceivers(contacts: Contact[]): Contact[]
 {

 this.InternalReceiversContacts = contacts.filter(contact =>
    contact.contactType === 8 ||
    contact.contactType === 9 ||
    contact.contactType === 10 ||
    contact.contactType === 11
);

    return this.InternalReceiversContacts;
 }
   isActiveForCurrentUser(circulation: Circulation) : boolean{
  if(circulation!.performerContactPerson!.id == this.workflow.currentCirculation!.performerContactPerson.id
     && (circulation.performedActionDateGregorian == null || circulation.performedActionDateGregorian.getFullYear() == 1) && !this.QC_AlreadyHasAction)
     {
      if((circulation.performerContactType == ContactTypes.QC_Lead || circulation.performerContactType == ContactTypes.QC_Manager) && (circulation.childCirculations == null || circulation.childCirculations.length==0))
      {

      const newChildCirculation :Circulation = this.generateNewCirculation();
      newChildCirculation.revisionId = circulation.revisionId;

      let  MaxSequenceNo = circulation.sequenceNo;
      newChildCirculation.sequenceNo = MaxSequenceNo+1;

        circulation.childCirculations = [newChildCirculation];
      }
    return true;
     }
  return false;
}


generateNewCirculation():Circulation{

  return {
        id: '00000000-0000-0000-0000-000000000000',
        actionType: 0,
        attachments: [],
        childCirculations: [],
        comment: '',
        createdBy: '',
        createdDate: new Date(),
        description: '',
        isHidden: false,
        modifiedDate: new Date(),
        performedActionDate: '',
        performedActionDateGregorian: new Date(),
        performerContactPerson: {
          id: '',
          address: undefined,
          phone: '',
          fax: '',
          name: '',
          email: '',
          homePage: '',
          note: '',
          contactType: 0,
          mobile: '',
          cc: '',
          userId: '',
          isVisible: false,
          password: '',
          belongedTo: 0
        },
        revisionId: '',
        performerPN: '',
        sequenceNo: 0,
        performerContactType: ContactTypes.Undefined,
        qCActionType: 0,
        previousActionType: 0,
        creatorComment:'',
        parentId:''
      };
}
 openReportDialog() {

  let dialogRef;
  this.inspectionReportService.getInspectionReportsByInspectionRequestId(this.RequestId).subscribe((reports)=>{

  //this.inspectionReportService.getInspectionReportsByInspectionId
    dialogRef = this.dialog.open(InspectionreportComponent, {
    width: '60vw',   // 80% of viewport width
    maxWidth: '100vw',
    maxHeight:'60vh',
    panelClass:'report-dialog',
      data: {
        notificationNumber: this.inspectionForm.get('notificationNumber')?.value,
        notificationDate: this.inspectionForm.get('RequestInspectionDateStart')?.value,
        reportsVersions : this.inspectionReports
      }
    });

       dialogRef.afterClosed().subscribe(result => {
    if (result) {
      this.handleReportSubmission(result);
    }
  });

  });


  }

  handleReportSubmission(reportData: any) {

    console.log('report data is sent to QC');
  }


  onInspectionActionChange(event: any) {
     const value = event.target.value;
     if (event.target.checked)
      {
         this.inspectionActions.push(value);
      }
      else
      {
        this.inspectionActions = this.inspectionActions.filter( x => x !== value );
      }
      const commaSeparated = this.inspectionActions.join(',');
      this.inspectionForm.get('inspectionAction')?.setValue(commaSeparated);
  }

//---------------------Comment Section Actions---------------------------

onAction(circulation: Circulation,actionType: number) {

     circulation.actionType = actionType;
    circulation.comment =  circulation.comment;


         this.circulationData.delete('Attachments');
     this.circulationData.delete('AttachmentIds');
     this.circulationData.delete('AttachmentNames');
     this.circulationData.delete('AttachmentVersions');
     this.circulationData.delete('AttachmentDescriptions');
     this.circulationData.delete('AttachmentDocNos');
     this.circulationData.delete('AttachmentStatuses');
      this.circulationData.delete('HasContent');


     if(circulation.attachments !=null)
     {
   circulation.attachments.forEach((attachment)=>{
    let name=''
    if(attachment != null)
      name = attachment.name;
 if (name && name.trim() !== '') {


    this.circulationData.append('AttachmentIds', attachment.id);
    this.circulationData.append('AttachmentNames', attachment.name);
    this.circulationData.append('AttachmentVersions', attachment.version);
    this.circulationData.append('AttachmentDescriptions', attachment.description);
    this.circulationData.append('AttachmentDocNos',attachment.docNo);
    this.circulationData.append('AttachmentStatuses',attachment.status);

    if (attachment.content) {
    this.circulationData.append('Attachments', attachment.content);
    this.circulationData.append('HasContent', 'true');
  } else {
    // 🔥 placeholder to keep index alignment
    this.circulationData.append(
      'Attachments',
      new Blob(['fakeContent'], { type: 'application/octet-stream' }),
      attachment.name
    );
    this.circulationData.append('HasContent', 'false');
  }

  }
  });
      }
this.circulationData.delete("circulation");
   circulation.attachments = null;
   let childCirculations =structuredClone( circulation.childCirculations);
   circulation.childCirculations = null;

   circulation.performedActionDateGregorian = new Date(Date.now());
   this.circulationData.append('circulation',JSON.stringify(circulation));



  this.inspectionService.saveCirculationData(this.circulationData).subscribe((savedCirculation)=>{
    if(savedCirculation)
    {

      circulation.actionType= savedCirculation.actionType;
      circulation.performedActionDate = savedCirculation.performedActionDate;
      circulation.performedActionDateGregorian = savedCirculation.performedActionDateGregorian;
      circulation.attachments = savedCirculation.attachments;
      circulation.childCirculations = childCirculations;
      this.toggleEditMode(false);
    }
  });

}

onCommentFilesPicked(event: Event,circulation: any): void {
  const input = event.target as HTMLInputElement;
  if (!input.files) return;
 if (!circulation.attachments) {
        circulation.attachments = [];
      }
  Array.from(input.files).forEach(file => {
    return  circulation.attachments.push({
      id: crypto.randomUUID(),
      name: file.name,
      description: '',
      downloadUrl: '', // fill after upload
      version: '0',
      content: file
    });
  });

  // Reset input so same file can be re-added if needed
  input.value = '';
}

 shouldShowButton(circulation: Circulation, action: 'approve' | 'backward' | 'reject' | 'upload'): boolean {
  if (!this.editMode || this.inspectionForm.get('id').value == '00000000-0000-0000-0000-000000000000') return false;
  if (!this.isActiveForCurrentUser(circulation)) return false;

  if(!(circulation.performerContactType == ContactTypes.QC_Lead || circulation.performerContactType == ContactTypes.QC_Manager)
    && action=='backward') return false;
  return action === 'approve' || action === 'backward' || action === 'reject' || action === 'upload';
 }

  showAll = true;
//-------------------End Of Comment Section Actions-------------------------

private setInspectionControlsEnabled(enable: boolean): void {

    const controls = [

        'notificationNumber',
        'requestInspectionPersianDateStart',
        'requestInspectionPersianDateEnd',
        'requestInspectionDueDate',
        'requestInspectionDateStart',
        'requestInspectionDateEnd',
        'persianIssueDate',
        'inspectionTimeComment',
        'inspectionTypes',
        'inspectionSubjects',
        'locations',
        'contacts',
        'orderName',
        'equipmentTagNo'

    ];

    controls.forEach(controlName => {

        const control = this.inspectionForm.get(controlName);

        if (!control) {
            return;
        }

        enable ? control.enable() : control.disable();

    });

}
private removeTemporaryRows(): void {

    const emptyId = '00000000-0000-0000-0000-000000000000';

    this.removePlaceholder(this.inspectionSubjectsFormArray, emptyId);

    this.removePlaceholder(this.inspectionTypesFormArray, emptyId);

    this.removePlaceholder(this.inspectionLocationFormArray, emptyId);

    this.removePlaceholder(this.contactsFormArray, emptyId);

}
private removePlaceholder(
    formArray: FormArray,
    id: string): void {

    const index = formArray.controls.findIndex(c =>
        c.get('id')?.value === id);

    if (index >= 0) {
        formArray.removeAt(index);
    }

}
toggleEditMode(editMode: boolean): void {

    this.editMode = editMode;
    const enableInspectionFields =
        editMode &&
        (!this.workflow || (this.workflow && !this.workflow.currentCirculation) || (this.workflow && this.workflow.currentCirculation && this.workflow.currentCirculation.sequenceNo === 0));

    if(!enableInspectionFields)
    {
    this.inspectionForm.controls.order.controls.title.disable();
    this.setInspectionControlsEnabled(enableInspectionFields);
    this.removeTemporaryRows();
    }
else
{
    const inspectionTypes = this.inspectionForm.get('inspectionTypes');
    const inspectionSubjects = this.inspectionForm.get('inspectionSubjects');

  const notificationNumberCtrl =  this.inspectionForm.get('notificationNumber');
  const CtrlRequestInspectionPersianDateStartCtrl = this.inspectionForm.get('RequestInspectionPersianDateStart');

  const CtrlInspectionTimeComment = this.inspectionForm.get('InspectionTimeComment');

  const CtrlRequestInspectionPersianDateEndCtrl = this.inspectionForm.get('RequestInspectionPersianDateEnd');

  const CtrlpersianIssueDateCtrl = this.inspectionForm.get('persianIssueDate');
  const RequisitionTitle = this.inspectionForm.controls['order'].get('title');

  const locations = this.inspectionForm.get('locations');
  const contacts = this.inspectionForm.get('contacts');
  const orderName = this.inspectionForm.get('orderName');
  const equipmentTagNo = this.inspectionForm.get('equipmentTagNo');


  RequisitionTitle?.disable();

  if (!this.workflow || (this.workflow && ( this.workflow.currentCirculation?.sequenceNo==0 || this.workflow.currentCirculation == null || this.workflow.currentCirculation == undefined))) {


    notificationNumberCtrl?.enable();
    CtrlRequestInspectionPersianDateStartCtrl?.enable();
    CtrlRequestInspectionPersianDateEndCtrl?.enable();
    CtrlpersianIssueDateCtrl?.enable();
    CtrlInspectionTimeComment?.enable();
    inspectionTypes?.enable();
    inspectionSubjects?.enable();
    locations?.enable();
    contacts?.enable();
    orderName?.enable();
    equipmentTagNo?.enable();

   }
}
}

    ngAfterViewInit() {  }

  sortInspections(inspections: Inspection[]):Inspection[]{
      // return latest revision of Inspection
      let sorted = inspections.sort((a,b)=>{
      // const revisionNumA = Date(a.revisions[0].submittedDate);
      // const revisionNumB = Number(b.revisions[0].revisionNumber);

      const createdDateA = Date.parse(a.createdDate.toString());
      const createdDateB = Date.parse(b.createdDate.toString());
      return createdDateB - createdDateA;
      });
      return sorted;
    }

  ngOnInit(): void {

    this.initialInspectionInfo = this.activatedRoute.snapshot.data['inspectionInfo'];

    let countOfInspections : number=0;
    let confirmedInspectionsByQC :Circulation[];
   let [mainNo, rev] = this.initialInspectionInfo != null ? this.initialInspectionInfo.newNotificationNumber.split('.') : ["", ""];

    runInInjectionContext(this.injector,()=>{
      effect(()=>{
         this.activatedRoute.paramMap.subscribe(params=>{

            if(params.keys.length>0)
              this.IsNewRequest = false;

             if(params.keys[0]=='RequestId')
              {
              this.RequestId = params.get('RequestId').toString();
              var inspectionNotificationId = params.get('InspectionId').toString();

                 this.inspectionService.getInspectionByRequestId(this.RequestId).subscribe((inspectioncmplx)=>{

                    let sortedInspections = this.sortInspections(inspectioncmplx.inspection);
                    countOfInspections = sortedInspections.length;
                    if(countOfInspections>0)
                    {
                      confirmedInspectionsByQC = sortedInspections[0].revisions[0].circulations.filter((circ)=>(circ.performerContactType== ContactTypes.QC_Lead || circ.performerContactType==ContactTypes.QC_Manager) && circ.performedActionDateGregorian.toString().trim()!='0001-01-01T00:00:00' && circ.actionType ==1);
                      if(confirmedInspectionsByQC != null && confirmedInspectionsByQC.length>0)
                        this.isConfirmedByQC = true;

                      let currentOrderInspectionSubjects = sortedInspections[0].order.inspectionSubjects;
                      if(currentOrderInspectionSubjects != null)
                           this.allSubjects = currentOrderInspectionSubjects.map(s => s.name);


                    }
                    this.generateInspectionFromValue(sortedInspections);
                    this.inspectionReports = inspectioncmplx.relatedReports;
                });

              }
              else // When we are creating new Inspection and select an order
              {
                this.IsNewRequest = true;
                let currentOrder:Order = this.currentAppState.currentorder();
                this.currentOrderExceptionContacts = this.orderSettingsExceptions.filter((ose)=>ose.orderId== currentOrder.id).map((ose)=>ose.projectContact);

                this.initializeFormData(currentOrder);


                  this.inspectionForm.patchValue({
                    notificationNumber: this.initialInspectionInfo.newNotificationNumber,
                    revNumber: this.revNumber,
                    mainNumber:mainNo,
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
            inspectionSubjects:currentOrder.inspectionSubjects
          },

          'ownerContactPerson': this.currentContacts!.filter((contact)=>contact.contactType==ContactTypes.Owner)[0],
          'mcContactPerson':this.currentContacts!.filter((contact)=>contact.contactType==ContactTypes.MC)[0],
          'contacts':  this.currentContacts!,
          'pidecContacts':this.currentContacts!.filter((contact)=>contact.contactType==ContactTypes.QC_Manager || contact.contactType==ContactTypes.QC_Lead),
          'pidecInspectors':this.currentContacts!.filter((contact)=>contact.contactType==ContactTypes.PIDEC_Inspector),
          'thirdPartInspectors':this.currentContacts!.filter((contact)=>contact.contactType==ContactTypes.ThirdPart_Inspector),
              });

              this.toggleEditMode(true);

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
              note: [contact.note],
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
                note: contact.note,
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
                  note: contact.note,
                  contactType: contact.contactType,
                  mobile: contact.mobile,
                  cc: contact.cc,
                  address: contact!.address
                })
              )

            }


      if(this.currentInspectionTypes !=null && this.currentInspectionTypes !=undefined)
      {
          this.currentInspectionTypes.forEach((inspectionType)=>

            this.inspectionTypesFormArray!.push(this.formBuilder.group({
              id:[inspectionType.id],
              name:[inspectionType.name],
              selected:[false]
              }))
          );}


          let currentOrderInspectionSubjects = this.currentAppState.vendorBasicInfo().orderInspectionSubjects?.filter((orderInspectionSubject)=>orderInspectionSubject.orderId == currentOrder.id);
        this.currentInspectionSubjects = currentOrderInspectionSubjects
        .map((orderInspectionSubject)=>orderInspectionSubject.inspectionSubject);
          if(this.currentInspectionSubjects != null)
              this.allSubjects = this.currentInspectionSubjects.map(s => s.name);
        }

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

trackByIndex(index: number, item: any): number {
  return index;
}
trackById(index: number, group: FormGroup): string {
  return group.get('id')?.value;
}
addInspectionSubject() {
    this.inspectionSubjectsFormArray.push(
      this.formBuilder.group({
        id: [crypto.randomUUID()],
        name: [''],
        itpRowNo:[''],
        selected: [false]
      })
    );
}

addInternalCorrespondence() {
    this.inspectionSubjectsFormArray.push(
      this.formBuilder.group({
        id: [crypto.randomUUID()],
        name: [''],
        itpRowNo:[''],
        selected: [false]
      })
    );
}

changeITPRow(newValue : string ,id:string){

 const indexOfNullId = this.inspectionSubjectsFormArray.controls.findIndex(ctrl => (ctrl.get('id')?.value === id));
    if(indexOfNullId>=0)
      this.inspectionSubjectsFormArray.at(indexOfNullId).patchValue({
        //id: (id==='00000000-0000-0000-0000-000000000000' ? crypto.randomUUID(): id),
        itpRowNo:newValue,
        // name : this.subjectCtrl.value?.trim()
      })
}

removeInsSubject(insSubject: FormTypes.FormInspectionSubject): void {

  const idx = this.inspectionSubjectsFormArray.controls.indexOf(insSubject);
  if(this.inspectionSubjectsFormArray.controls.length>1)
  {
    if (idx >= 0) {
      this.inspectionSubjectsFormArray.removeAt(idx);
    }
  }
  else
      this.inspectionSubjectsFormArray.at(idx).reset({name:'',itpRowNo:''});
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

removeToContact(contact: FormContact): void {

  const idx = this.contactsFormArray.controls.indexOf(contact);
  let countOfVisibleContacts = this.contactsFormArray.controls.filter((cnt)=>{
    // if(cnt.get('id')?.value == this.currentCirculation.performerContactPerson.id)
    //  cnt.patchValue({isVisible: false});
    if(this.workflow.currentCirculation !=undefined)
      return (cnt.get('id')?.value== true && cnt.get('id')?.value != this.workflow.currentCirculation?.performerContactPerson.id)
    else
      return cnt.get('isVisible')?.value== true
  } ).length;
  if(this.IsNewRequest == true && countOfVisibleContacts>1)
    countOfVisibleContacts--;
  if(countOfVisibleContacts>1)
  {
    if (idx >= 0) {
      this.contactsFormArray.removeAt(idx);
    }
  }
  else
    this.contactsFormArray.at(idx).reset({  id:'00000000-0000-0000-0000-000000000000',
      note:'',
      cc:'',
      email:'',
      fax:'',
      name:'',
      phone:'',
      mobile:'',
      address:this.generateAddress(),
      contactType:ContactTypes.Supplier,
      homePage:'',
      userId:'',
      isVisible:[true],
      password:'',
      belongedTo:ObjectTypes.Undefined});

}

  addNewToContact():void{

   let rawContact = this.formBuilder.group({
      id:'00000000-0000-0000-0000-000000000000',
      note:'',
      cc:'',
      email:'',
      fax:'',
      name:'',
      phone:'',
      mobile:'',
      address:this.generateAddress(),
      contactType:ContactTypes.Supplier,
      homePage:'',
      userId:'',
      isVisible:[true],
      password:'',
      belongedTo:ObjectTypes.Undefined
    })
    this.contactsFormArray.push(rawContact);
    //this.contactsFormArray.push(rawContact);
    //this.pidecContactsFormArray.push(rawContact);
  }


removeAddress(location: FormTypes.FormAddress): void {
  const idx = this.inspectionLocationFormArray.controls.indexOf(location);
  if(this.inspectionLocationFormArray.controls.length>1)
  {
  if (idx >= 0) {
    this.inspectionLocationFormArray.removeAt(idx);
  }
}
else
    this.inspectionLocationFormArray.at(idx).reset({ id:'00000000-0000-0000-0000-000000000000',
      country:'',
      state:'',
      city:'',
      streetLine1:'',
      streetLine2:'',
      zipCode:''});
}


  addNewAddress():void{

   let rawAddress = this.formBuilder.group({
      id:'00000000-0000-0000-0000-000000000000',
      country:'',
      state:'',
      city:'',
      streetLine1:'',
      streetLine2:'',
      zipCode:''
    })
    this.inspectionLocationFormArray.push(rawAddress);
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

    setIssueDate(issueDate: Date):void{
    let formattedDateNow =  this.LocalFormatDate(issueDate);
    this.formattedGregorianIssueDate = formattedDateNow;
     const gregorianDate: Date = issueDate;
      const { jy, jm, jd } = toJalaali(gregorianDate.getFullYear(), gregorianDate.getMonth() + 1, gregorianDate.getDate());

      const formattedPersianDate = `${jy}/${jm}/${('0' + jd).slice(-2)}`;
       const persianMonthName = this.getPersianMonthName(jm);
      this.formattedPersianIssueDate = `${jd} ${persianMonthName} ${jy}`;
      //this.formattedPersianIssueDate = formattedPersianDate;
    }

    GetPersianMonthName(finglishName:string):string
    {
      switch(finglishName.toLowerCase().trim()){
        case "farvardin":
          return "فروردین";
        case "ordibehesht":
          return "اردیبهشت";
        case "khordad":
          return "خرداد";
        case "tir":
          return "تیر";
        case "mordad":
          return "مرداد";
        case "shahrivar":
          return "شهریور";
        case "mehr":
          return "مهر";
        case "aban":
          return "آبان";
        case "azar":
          return "آذر";
        case "dey":
          return "دی";
        case "bahman":
          return "بهمن";
        case "esfand":
          return "اسفند";

      }
       return "";
    }

    generateInspectionForm():FormTypes.InspectionForm{



      let DateNow = new Date(Date.now());
      this.setIssueDate(DateNow);
    let formattedDateNow =  this.LocalFormatDate(DateNow);
    this.formattedGregorianIssueDate = formattedDateNow;
    this.formattedGregorianReqEndDate = formattedDateNow;
    this.formattedGregorianReqStartDate = formattedDateNow;
    let currentUserName= this.currentAppState.currentuser().userName;

      const gregorianDate: Date = DateNow;
      const { jy, jm, jd } = toJalaali(gregorianDate.getFullYear(), gregorianDate.getMonth() + 1, gregorianDate.getDate());

        const formattedPersianDate = `${jy}/${jm}/${('0' + jd).slice(-2)}`;


        let currentOrderContacts = this.currentAppState.vendorBasicInfo().orderContacts;
        this.formattedPersianReqStartDate = formattedPersianDate;
        this.formattedPersianReqEndDate = formattedPersianDate;
        //this.vendorAttachments =  this.formBuilder.array<FormTypes.FormAttachment>([this.generateAttachment()]);
      return this.formBuilder.group({

          id:['00000000-0000-0000-0000-000000000000'],
          //attachments: this.formBuilder.array<FormTypes.FormAttachment>([this.generateAttachment()]),
          contacts : this.formBuilder.array<FormTypes.FormContact>([this.generateContact()]),
          pidecContacts: this.formBuilder.array<FormTypes.FormContact>([this.generateContact()]),
          pidecInspectors : this.formBuilder.array<FormTypes.FormContact>([this.generateInspectorContact(ContactTypes.PIDEC_Inspector)]),
          thirdPartInspectors:this.formBuilder.array<FormTypes.FormContact>([this.generateInspectorContact(ContactTypes.ThirdPart_Inspector)]),
          locations: this.formBuilder.array<FormTypes.FormAddress>([this.generateAddress()]),
          order:this.generateOrder(),
          equipmentTagNo:[''],
          orderName:[''],
        //  owner:this.generateOwner(),
         // vendor:this.generateVendor(),
          ownerContactPerson: this.generateContact(),
          mcContactPerson:this.generateContact(),
          supplierContactPerson:this.generateContact(),
          subSupplierContactPerson:this.generateContact(),
          sub_supplier:[''],
          ownerDocNo:[''],
          pidecDocNo:[''],
          supplier:[''],
          revNumber:[''],
          mainNumber:[''],
          workProgress:new Date().toLocaleDateString('fa-IR-u-nu-latn', {
            year: 'numeric',
            day: '2-digit',
            month: 'long'
          }),
          notificationNumber:[this.initialInspectionInfo.newNotificationNumber],

          createdBy: [currentUserName],
          modifiedBy :[currentUserName],
          createdDate:[ DateNow],
          modifiedDate : [DateNow],
          issueDate:[DateNow],
          persianIssueDate:[formattedPersianDate] ,
          requestInspectionDateStart: [DateNow],
          requestInspectionDateEnd:[DateNow],
          requestInspectionPersianDateStart:[formattedPersianDate],
          requestInspectionPersianDateEnd:[formattedPersianDate],
          requestInspectionDueDate:[1],
          inspectionTimeComment:[''],
          inspectionAction:[''],
          inspectionSubjects:this.formBuilder.array<FormTypes.FormInspectionSubject>([this.generateInspectionSubject()]),
          inspectionTypes: this.formBuilder.array<FormTypes.FormInspectionType>([this.generateInspectionType()]),

        });

    }

  generateInspectionType():FormTypes.FormInspectionType{
    return this.formBuilder.group({
      id:['00000000-0000-0000-0000-000000000000'],
      name:[''],
      selected : new FormControl<boolean>(false)
    });
  }

  generateInspectionSubject():FormTypes.FormInspectionSubject{
    return this.formBuilder.group({
      id:['00000000-0000-0000-0000-000000000000'],
      name:[''],
      itpRowNo:[''],
      selected : new FormControl<boolean>(false)
    });
  }

  generateAttachment():FormTypes.FormAttachment{
    return this.formBuilder.group({
      id:'00000000-0000-0000-0000-000000000000',
      version:[''] ,
      name:[''],
      content: [],
      description:[''],
      downloadUrl:[''],
      docNo:[''],
      status:[''],
      pdfSize:[''],
      edmsId:['']
    });
  }

  generateAttachmentFromValue(attachment:Attachment):FormTypes.FormAttachment{

    return this.formBuilder.group({
      id: [attachment.id],
      version:[attachment.version],
      content:[attachment.content],
      description:[attachment.description],
      name:[attachment.name],
      downloadUrl:[attachment.downloadUrl],
       docNo:[attachment.docNo],
      status:[attachment.status],
      edmsId:[attachment.edmsId],
      pdfSize:[attachment.pdfSize]
    })
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
      project:order.project,//this.currentAppState.currentproject(),
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
      pidecQCContacts:this.formBuilder.array<FormTypes.FormContact>([]),
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
      id:['00000000-0000-0000-0000-000000000000'],
      note:[''],
      cc:[''],
      email:[''],
      fax:[''],
      name:[''],
      phone:[''],
      mobile:[''],
      address:this.generateAddress(),
      contactType:[5],
      homePage:[''],
      userId:[''],
      isVisible:[true],
      password:[''],
      belongedTo: [ObjectTypes.Undefined]
    })
  }

  initializeFormData(order:Order):void{
     let currentOrder:Order = order;
    //const currentOrderContacts;
    let currentProjectContacts =null;


      const currentOrderContacts = this.currentAppState.vendorBasicInfo().orderContacts?.filter((ordercontact)=>ordercontact.orderId==currentOrder.id);
     let idsToExclude = null;
      if(this.currentOrderExceptionContacts != undefined)
         idsToExclude = new Set(this.currentOrderExceptionContacts.map(item => item.id));

      if(idsToExclude != null)
          currentProjectContacts = this.currentAppState.vendorBasicInfo().projectContacts?.filter((projectContact)=>projectContact.projectId==this.currentProject.id && !idsToExclude.has(projectContact.id));
      else
          currentProjectContacts = this.currentAppState.vendorBasicInfo().projectContacts?.filter((projectContact)=>projectContact.projectId==this.currentProject.id);

       this.currentContacts = currentOrderContacts.map((ordercontact)=>ordercontact.contact);
       this.currentContacts = this.currentContacts.concat(currentProjectContacts.map((projectcontact)=>projectcontact.contact));

        let currentOrderInspectionSubjects = this.currentAppState.vendorBasicInfo().orderInspectionSubjects?.filter((orderInspectionSubject)=>orderInspectionSubject.orderId == currentOrder.id);
        this.currentInspectionSubjects = currentOrderInspectionSubjects
        .map((orderInspectionSubject)=>orderInspectionSubject.inspectionSubject);

         this.currentInspectionTypes = this.currentVendorStore.inspectionTypes();

          this.inspectionForm = this.generateInspectionForm();

  }


  VisibleCirculations(circulations:Circulation[]): Circulation[]
  {
    let result :Circulation[] = circulations;
    if(circulations !=null)
    {
    if(this.currentAppState.currentuser().userType == UserTypes.Vendor)
    {
      result = circulations.filter((cr)=> cr.performerContactType == ContactTypes.QC_Lead || cr.performerContactType == ContactTypes.QC_Manager )
    }
    else if(this.currentAppState.currentuser().userType == UserTypes.PIDEC_CC || this.currentAppState.currentuser().userType == UserTypes.PIDEC_TO)
    {
        result = circulations.filter((cr)=>  cr.performerContactType != ContactTypes.Vendor )
    }
    else if(this.currentAppState.currentuser().userType == UserTypes.Department_Lead )
    {
        result = circulations.filter((cr)=>  cr.performerContactType == ContactTypes.Department_Lead || cr.performerContactType == ContactTypes.Department_Originator || cr.performerContactType == ContactTypes.Vendor )
    }
    else if(this.currentAppState.currentuser().userType == UserTypes.MC )
    {
        result = circulations.filter((cr)=>  cr.performerContactType == ContactTypes.MC || cr.performerContactType == ContactTypes.Vendor )
    }
    else if(this.currentAppState.currentuser().userType == UserTypes.Owner )
    {
        result = circulations.filter((cr)=>  cr.performerContactType == ContactTypes.Owner || cr.performerContactType == ContactTypes.Vendor )
    }
  }
    return result;
  }



openDocuments(): void {

    this.dialog.open(DocumentManagerDialogComponent, {

        width: '1100px',

        maxHeight: '90vh',

        autoFocus: false,

        data: {

            ownerType: DocumentOwnerType.InspectionNotification,

            ownerId: this.workflow.currentCirculation.id,

            readOnly: !this.editMode

        }

    });

}

buildForm(vm: InspectionWorkflowViewModel): FormGroup {

    const form = this.createForm(vm.inspection,vm);

    this.inspectionForm = form;
    this.populateContacts(form, vm.inspection);

    this.populateLocations(vm);
    this.populateInspectionTypes(form, vm.inspection);
    this.populateInspectionSubjects(form, vm.inspection);
    this.attachments = vm.attachments;
    this.configureForm(form,vm.inspection, vm);
    this.BuildInternalCorrespondenceReceivers(vm.inspection.contacts);
    return form;
}

private initializeUI(): void {

    this.formattedPersianReqStartDate =
        this.ConvertGregorianDateToPersianDate(
            this.workflow.inspection.requestInspectionDateStart);

    this.formattedPersianReqEndDate =
        this.ConvertGregorianDateToPersianDate(
            this.workflow.inspection.requestInspectionDateEnd);

    const issueDate =
        new Date(this.workflow.inspection.issueDate);

    this.setIssueDate(issueDate);
    this.toggleEditMode(this.workflow.editMode);
}

private populateContacts(
    form: FormGroup,
    inspection: Inspection
): void {

    const contacts =
        form.get('contacts') as FormArray;

    inspection.contacts.forEach(c => {

        contacts.push(
            this.generateContactFromContactValue(c)
        );

    });

     this.inspectionForm.patchValue({ownerContactPerson: inspection.contacts!.filter((contact)=>contact.contactType==ContactTypes.Owner)[0]});
              this.inspectionForm.patchValue({mcContactPerson:inspection.contacts!.filter((contact)=>contact.contactType==ContactTypes.MC)[0]});
              this.inspectionForm.patchValue({contacts:  inspection.contacts!});
              this.inspectionForm.patchValue({pidecContacts:inspection.contacts!.filter((contact)=>contact.contactType==ContactTypes.QC_Manager || contact.contactType==ContactTypes.QC_Lead)});
              this.inspectionForm.patchValue({pidecInspectors:inspection.contacts!.filter((contact)=>contact.contactType==ContactTypes.PIDEC_Inspector)});
              this.inspectionForm.patchValue({thirdPartInspectors:inspection.contacts!.filter((contact)=>contact.contactType==ContactTypes.ThirdPart_Inspector)});
}

public CurrentCirculationHasCorrespondences(circulation :CirculationVM):boolean{

  return (circulation.current.childCirculations != null &&
              circulation.current.childCirculations.length > 0 &&
              circulation.current.childCirculations != null &&
              circulation.current.childCirculations.length > 1)
}

public currentUserIsQC(circulationvm:CirculationVM): boolean {

     return (circulationvm.current?.performerContactType == ContactTypes.QC_Manager ||
          circulationvm.current?.performerContactType == ContactTypes.QC_Lead) &&
        circulationvm.current.performerContactPerson.userId == this.currentUser.id

}

public currentPerformerIsQC(circulationvm:CirculationVM): boolean {

     return (circulationvm.current?.performerContactType == ContactTypes.QC_Manager ||
          circulationvm.current?.performerContactType == ContactTypes.QC_Lead) &&
        circulationvm.current.performerContactPerson.userId == this.currentUser.id && circulationvm.current.performedActionDateGregorian.getFullYear()==1

}

public CirculationActionType(circulationvm:CirculationVM): number {

    if(circulationvm.current.performedActionDateGregorian.getFullYear()==1)
      return -1;//It means Non Action is done
    return circulationvm.current.actionType;


}

public canEditLocations(circulation:Circulation): boolean {

    return this.editMode &&
           (!circulation ||
            circulation.performerContactType === ContactTypes.Vendor);

}
public canAddLocations(circulation:Circulation): boolean {

    return this.editMode &&
           (!circulation ||
            circulation.performerContactType === ContactTypes.Vendor);

}


public ContactIsHidden(contact:FormContact):boolean
{
  if(!this.canEditInspectionNotificationMtadata && contact.value.id !='00000000-0000-0000-0000-000000000000')
    return false;

  if(!this.canEditInspectionNotificationMtadata && contact.value.id =='00000000-0000-0000-0000-000000000000')
    return true;
  return false;
}

public LocationIsHidden(location:FormTypes.FormAddress):boolean
{
  if(!this.canEditInspectionNotificationMtadata && location.value.id =='00000000-0000-0000-0000-000000000000')
    return true;
  return false;
}

get canEditInspectionNotificationMtadata(): boolean {

  if (!this.workflow) {
    return true;
  }

  if (!this.workflow.editMode) {
    return false;
  }

  if (!this.workflow.currentCirculation) {
    return false;
  }

  return this.workflow.currentCirculation.performerContactType
      === ContactTypes.Vendor;
}

get IsReadOnlyInspectionNotificationMtadata(): boolean {

if(!this.workflow)
  return false;


  if (this.workflow) {

    if(this.workflow.editMode && this.workflow.currentCirculation != null &&
                this.workflow.currentCirculation?.performerContactType != ContactTypes.Vendor)
      return true;

  }

  return false;
}

get canShowSubmittData():boolean{

  if(this.inspectionForm.get('id').value != null && this.inspectionForm.get('id').value !=undefined && this.inspectionForm.get('id').value != '00000000-0000-0000-0000-000000000000')
  {
    return true;
  }
    if(!this.workflow)
    {
      return false;
    }

 if (this.workflow && !this.workflow.editMode) {
    return true;
  }

  if (this.workflow &&!this.workflow.currentCirculation) {
    return true;
  }

  return   (this.workflow && this.workflow.currentCirculation.sequenceNo > 0);
}

get canSubmitInspectionNotification():boolean{

   if (!this.workflow)
    {
      if(this.inspectionForm.get('id').value == null || this.inspectionForm.get('id').value== undefined || this.inspectionForm.get('id').value=='00000000-0000-0000-0000-000000000000')
        return true;
      else
        return false;
  }
  else //if (this.workflow )
  {
  if(!this.workflow.editMode) {
    return false;
  }

  if (!this.workflow.currentCirculation) {
    return true;
  }

  return this.workflow.currentCirculation.performerContactType
      === ContactTypes.Vendor;
}

}

  private populateLocations(vm: InspectionWorkflowViewModel): void {

    let locations: Address[]= vm.inspection.locations;
    const formArray = this.inspectionLocationFormArray;

    formArray.clear();

    if (!locations || locations.length === 0) {
        formArray.push(this.generateAddress());
        return;
    }

    locations.forEach(location => {
        formArray.push(
            this.generateAddressFromAddressValue(location)
        );
    });

    // Add an empty row for editing if needed
    if (this.editMode && this.canAddLocations(vm.currentCirculation)) {
        formArray.push(this.generateAddress());
    }


}

// private populateInspectionTypes(
//     form: FormGroup,
//     inspection: Inspection
// ): void {

//     const types =
//         form.get('inspectionTypes') as FormArray;

//     inspection.inspectionTypes.forEach(t => {

//         types.push(
//             this.generateInspectionTypeFromValue(t)
//         );

//     });

// }

private populateInspectionTypes(
  form: FormGroup,
  inspection: Inspection
): void {

  const types = form.get('inspectionTypes') as FormArray;

  types.clear();

  const savedTypeIds = new Set(
    inspection.inspectionTypes.map(t => t.id)
  );

  this.currentInspectionTypes.forEach(type => {

    const inspectionType: InspectionType = {
      ...type,
      selected: savedTypeIds.has(type.id)
    };

    types.push(
      this.generateInspectionTypeFromValue(inspectionType)
    );
  });
}

private populateInspectionSubjects(
    form: FormGroup,
    inspection: Inspection
): void {



   let currentOrderInspectionSubjects = inspection.order.inspectionSubjects;

          if(currentOrderInspectionSubjects != null)
              this.allSubjects = currentOrderInspectionSubjects.map(s => s.name);


    const subjects =
        form.get('inspectionSubjects') as FormArray;

    inspection.inspectionSubjects.forEach(s => {

        subjects.push(
            this.generateInspectionSubjectFromValue(s)
        );

    });

}

private configureForm(
    form: FormGroup,
    inspection: Inspection,
    vm: InspectionWorkflowViewModel
): void {

    if (inspection.inspectionAction) {

        this.inspectionActions = inspection.inspectionAction.split(',');
        form.get('inspectionAction')
            ?.setValue(
                inspection.inspectionAction
            );

    }

    if (!vm.editMode) {

        form.disable({
            emitEvent: false
        });

    }



}

private createForm(
    inspection: Inspection,
    vm: InspectionWorkflowViewModel
): FormGroup {



  const [mainNo, rev] = inspection.notificationNumber.split('.');

    return this.formBuilder.group({

        id: [inspection.id],

        contacts: this.formBuilder.array<FormTypes.FormContact>([this.generateContact()]),

        locations: this.formBuilder.array<FormTypes.FormLocation>([this.generateLocation()]),

        pidecContacts: this.formBuilder.array<FormTypes.FormContact>([this.generateContact()]),

        pidecInspectors: this.formBuilder.array<FormTypes.FormContact>([this.generateInspectorContact(ContactTypes.PIDEC_Inspector)]),

        thirdPartInspectors: this.formBuilder.array<FormTypes.FormContact>([this.generateInspectorContact(ContactTypes.ThirdPart_Inspector)]),

        inspectionSubjects: this.formBuilder.array<FormTypes.FormInspectionSubject>([this.generateInspectionSubject()]),

        inspectionTypes: this.formBuilder.array<FormTypes.FormInspectionType>([this.generateInspectionType()]),

        attachments: this.formBuilder.array<FormTypes.FormAttachment>([this.generateAttachment()]),

        order:
            inspection.order
                ? this.generateOrderFromValue(inspection.order)
                : this.generateOrder(),

        orderName: [inspection.orderName],

        equipmentTagNo: [inspection.equipmentTagNo],

        ownerContactPerson: this.generateContact(),

        mcContactPerson: this.generateContact(),

        supplierContactPerson: this.generateContact(),

        subSupplierContactPerson: this.generateContact(),

        revNumber: [vm.shownRevisionNumber],

        notificationNumber:
            [mainNo],

        ownerDocNo: [''],

        pidecDocNo: [''],

        supplier: [''],

        sub_supplier: [''],

        workProgress: [''],

        createdBy: [inspection.createdBy],

        modifiedBy: [inspection.modifiedBy],

        createdDate: [inspection.createdDate],

        modifiedDate: [inspection.modifiedDate],

        issueDate: [inspection.issueDate],

        persianIssueDate:
            [inspection.persianIssueDate],

        requestInspectionDateStart:
            [inspection.requestInspectionDateStart],

        requestInspectionDateEnd:
            [inspection.requestInspectionDateEnd],

        requestInspectionDueDate:
            [inspection.requestInspectionDueDate],

        inspectionTimeComment:
            [inspection.inspectionTimeComment],

        inspectionAction:
            [inspection.inspectionAction],

        requestInspectionPersianDateStart:
            [inspection.requestInspectionPersianDateStart],

        requestInspectionPersianDateEnd:
            [inspection.requestInspectionPersianDateEnd]

    });

}
build(vm: InspectionWorkflowViewModel): FormGroup {

    const inspection = vm.inspection;

    const form = this.createForm(inspection, vm);

    this.populateContacts(form, inspection);

    this.populateInspectionTypes(form, inspection);

    this.populateInspectionSubjects(form, inspection);

    //this.populateLocations(form, inspection);

    this.configureForm(form, inspection, vm);

    return form;
}

setCurrentProject(project: Project):void {
 if((project!= null && project != undefined) && (this.currentProject==undefined || this.currentProject==null ||   Object.keys(this.currentProject).length === 0))
      {
        this.currentProject= {} as Project;
        this.currentProject.fSId = project['fsId'];
        this.currentProject.id = project.id;
        this.currentProject.number = project.number;
        this.currentProject.description = project.description;
        this.currentProject.createdBy = project.createdBy;
        this.currentProject.createdDate = project.createdDate;
      }
}


generateInspectionFromValue(inspections :Inspection[]):void{


  const vm=this.workflowService.buildWorkflow(
        inspections,
        this.currentUser);

     this.setCurrentProject(vm.inspection.order.project);
    this.workflow=vm;
 this.inspectionForm =this.buildForm(this.workflow);

this.initializeUI();

}

 openCirculationModal(Circulation :Circulation){


  if(Circulation.childCirculations == null || Circulation.childCirculations== undefined || Circulation.childCirculations.length==0)
    Circulation.childCirculations= this.allCirculationsInCurrentRevision.filter((circ)=>circ.performerContactPerson.userId==Circulation.performerContactPerson.userId)


  const dialogRef = this.dialog.open(CirculationModalComponent, {

    width: '900px',

    height: '600px',

    data:  Circulation.childCirculations.sort((a,b)=>{
          return a.sequenceNo - a.sequenceNo;
      }),
     panelClass: 'circulation-dialog'

  });

  dialogRef.afterClosed().subscribe(result => {

    if (result) {

      console.log(result);

      // result is the selected Circulation

    }

  });

}

 get revNumber(): string
 {
   const raw_Revision = this.inspectionForm.get('revNumber')?.value ?? '';
   const padded = raw_Revision.padStart(2, "0");
   return padded;
 }
set revNumber(value: string) {
   const padded = value.padStart(2, "0");
  this.inspectionForm.get('revNumber')?.setValue(padded);
 }

 generateInspectionSubjectsFormArrayFromValue(inspectionSubjects: InspectionSubject[]):void//FormArray<FormTypes.FormInspectionSubject>
{
      if(inspectionSubjects != null && inspectionSubjects != undefined)
        {
 this.allSubjects = inspectionSubjects.map(s => s.name);
        }
  inspectionSubjects!.forEach(inspectionSubject=>
  {
    const index = this.inspectionSubjectsFormArray.value.findIndex(
    (item: FormTypes.FormInspectionSubject) => item?.['id'] === inspectionSubject.id
    );
    if(index != -1)
      this.inspectionSubjectsFormArray.at(index).patchValue({
        itpRowNo:inspectionSubject.itpRowNo,
        name : inspectionSubject.name,
        id: inspectionSubject.id,
        selected:true
      });
    else
      this.inspectionSubjectsFormArray.push(this.generateInspectionSubjectFromValue(inspectionSubject));
  }
  );

 }

 generateInspectionTypesFormArrayFromValue(inspectionTypes: InspectionType[]):void//FormArray<FormTypes.FormInspectionType>
{

   inspectionTypes!.forEach(inspectionType=>
  {

      const index = this.inspectionTypesFormArray.value.findIndex(
    (item: FormTypes.FormInspectionType) => item?.['id'] === inspectionType.id
    );
    if(index != -1)
      this.inspectionTypesFormArray.controls.at(index).patchValue({
        selected:true
      });
    else
      this.inspectionTypesFormArray.push(this.generateInspectionTypeFromValue(inspectionType));

  }
  );

 }
 ContactTypeSelect(cnt):void{

 }

  generateContactsFormArrayFromValue(contacts: Contact[]):void//FormArray<FormTypes.FormInspectionSubject>
{

  contacts!.forEach(contact=>
  {
    const index = this.contactsFormArray.value.findIndex(
    (item: FormTypes.FormContact) => item?.['id'] === contact.id
    );
    if(index != -1)
      this.contactsFormArray.at(index).patchValue({
        email: contact.email,
        contactType:contact.contactType,
        mobile: contact.mobile,
        phone:contact.phone,
        homePage:contact.homePage,
        name : contact.name,
        id: contact.id,

      });
    else
      this.contactsFormArray.push(this.generateContactFromContactValue(contact));
  }
  );

 }

 generateInspectionLocationsFormArrayFromValue(locations: Address[]):void//FormArray<FormTypes.FormInspectionSubject>
{

  locations!.forEach(location=>
  {
    const index = this.inspectionLocationFormArray.value.findIndex(
    (item: FormTypes.FormAddress) => item?.['id'] === location.id
    );
    if(index != -1)
      this.inspectionLocationFormArray.at(index).patchValue({
        country: location.country,
        state:location.state,
        city : location.city,
        streetLine1: location.streetLine1,
        streetLine2: location.streetLine2,
        zipCode : location.zipCode,
        id: location.id
      });
    else
      this.inspectionLocationFormArray.push(this.generateInspectionLocationFromValue(location));
  }
  );

 }

 generateInspectionLocationFromValue(location: Address):FormTypes.FormAddress
 {
     return this.formBuilder.group({
        country: location.country,
        state:location.state,
        city : location.city,
        streetLine1: location.streetLine1,
        streetLine2: location.streetLine2,
        zipCode : location.zipCode,
        id: location.id
     });
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

  id:[inspectionSubject.id],
  name:[inspectionSubject.name],
  itpRowNo:[inspectionSubject.itpRowNo],
  selected:[true]
  //selected:inspectionSubject.selected
});
  }
  generateAddress():FormTypes.FormAddress{
       return this.formBuilder.group({
        id:['00000000-0000-0000-0000-000000000000'],
         city:[''],
         country:[''],
         state:[''],
         streetLine1:[''],
         streetLine2:[''],
         zipCode:['']
       })
     }

    generateAddressFromAddressValue(address:Address):FormTypes.FormAddress{
        return this.formBuilder.group({
          id:[address.id],
          city:[address.city],
          country:[address.country],
          state:[address.state],
          streetLine1:[address.streetLine1],
          streetLine2:[address.streetLine2],
          zipCode:[address.zipCode]
        })
      }

  generateContactFromContactValue(contact:Contact):FormTypes.FormContact{
    return this.formBuilder.group({
      id:[contact.id],
      note:[contact.note],
      cc:[contact.cc],
      email:[contact.email],
      fax:[contact.fax],
      name:[contact.name],
      phone:[contact.phone],
      contactType:[contact.contactType],
      mobile:[contact.mobile],
      homePage:[contact.homePage],
      address:this.generateAddressFromAddressValue(contact.address),
      userId:[contact.userId],
      isVisible:[true],
      password:[contact.password],
      belongedTo:[contact.belongedTo]
  })
}

// ============================================================================
// Inspector picker (pidecInspectors / thirdPartInspectors rows)
//
// Each row IS the inspecting company's own Contact (ContactType PIDEC_Inspector(8)
// or ThirdPart_Inspector(9)). "assignedInspector*" controls are added on top of the
// normal Contact shape so SaveNewInspection()/DraftNewInspection() - which still
// concatenate these rows into rawFormValue.contacts - keep working unchanged.
//
// NOTE: when an existing inspection is loaded from the server, these extra
// assignedInspector* fields will be empty unless your API's contact payload for
// this inspection is extended to also return InspectorId/skills/status for these
// two arrays. Until then, a loaded (historical) row falls back to showing just
// name/phone/email, with no skill chips or status badge.
// ============================================================================

/** Builds one row for pidecInspectors/thirdPartInspectors: a normal contact row plus the
 *  controls used to show which saved Inspector (resume) was assigned to it. */
generateInspectorContact(contactType: number): FormGroup {
  const group = this.generateContact() as unknown as FormGroup;
  group.patchValue({ contactType });
  group.addControl('assignedInspectorId', this.formBuilder.control<string | null>(null));
  group.addControl('assignedInspectorPosition', this.formBuilder.control(''));
  group.addControl('assignedInspectorSkillsLabel', this.formBuilder.control(''));
  group.addControl('assignedInspectorStatus', this.formBuilder.control<number | null>(null));
  return group;
}

get pidecInspectorRows(): FormArray<FormGroup> {
  return this.inspectionForm.get('pidecInspectors') as FormArray<FormGroup>;
}

get thirdPartyInspectorRows(): FormArray<FormGroup> {
  return this.inspectionForm.get('thirdPartInspectors') as FormArray<FormGroup>;
}

addPidecInspectorRow(): void {
  this.pidecInspectorRows.push(this.generateInspectorContact(ContactTypes.PIDEC_Inspector));
}

addThirdPartyInspectorRow(): void {
  this.thirdPartyInspectorRows.push(this.generateInspectorContact(ContactTypes.ThirdPart_Inspector));
}

removeInspectorRow(array: FormArray<FormGroup>, group: FormGroup): void {
  const idx = array.controls.indexOf(group);
  if (array.controls.length > 1) {
    if (idx >= 0) array.removeAt(idx);
  } else {
    const contactType = group.get('contactType')?.value;
    array.at(idx).reset({
      id: '00000000-0000-0000-0000-000000000000',
      note: '', cc: '', email: '', fax: '', name: '', phone: '', mobile: '',
      address: this.generateAddress(), contactType, homePage: '', userId: '',
      isVisible: [true], password: '', belongedTo: ObjectTypes.Undefined,
      assignedInspectorId: null, assignedInspectorPosition: '',
      assignedInspectorSkillsLabel: '', assignedInspectorStatus: null,
    });
  }
}

/** Loads (and caches) the approved inspectors for this inspecting company, the first time a row for it is used. */
private ensureInspectorsLoaded(inspectionCompanyContactId: string): void {
  if (!inspectionCompanyContactId || inspectionCompanyContactId === '00000000-0000-0000-0000-000000000000') return;
  if (this.inspectorsByCompanyContact.has(inspectionCompanyContactId)) return;

  // TODO: confirm your InspectorService.getAll() supports these two filter params
  // (inspectionCompanyContactId, qcApprovalState). If not, add them server-side first.
  this.inspectorsByCompanyContact.set(inspectionCompanyContactId, []); // avoid duplicate in-flight requests
  this.inspectorService
    .getAll({ inspectionCompanyContactId, qcApprovalState: 1 /* Approved */ })
    .subscribe({
      next: (list) => this.inspectorsByCompanyContact.set(inspectionCompanyContactId, list),
      error: () => this.inspectorsByCompanyContact.delete(inspectionCompanyContactId),
    });
}

onInspectorRowFocus(group: FormGroup): void {
  this.ensureInspectorsLoaded(group.get('id')?.value);
}

onInspectorSearchInput(group: FormGroup, term: string): void {
  this.inspectorSearchTerms.set(group, term);
}

/** Options shown in this row's autocomplete: that company's approved inspectors matching the typed text. */
filteredInspectorsFor(group: FormGroup): Inspector[] {
  const companyContactId = group.get('id')?.value;
  const all = this.inspectorsByCompanyContact.get(companyContactId) ?? [];
  const term = (this.inspectorSearchTerms.get(group) ?? '').trim().toLowerCase();
  const matches = term
    ? all.filter((i) => (i.name ?? `${i.firstName} ${i.lastName}`).toLowerCase().includes(term))
    : all;
  return matches.slice(0, 15);
}

pickInspector(group: FormGroup, inspector: Inspector): void {
  group.patchValue({
    name: inspector.name ?? `${inspector.firstName} ${inspector.lastName}`,
    phone: inspector.phone ?? '',
    email: inspector.email ?? '',
    assignedInspectorId: inspector.inspectorId,
    assignedInspectorPosition: inspector.position ?? '',
    assignedInspectorSkillsLabel: (inspector.skills ?? []).map((s) => s.code).join(', '),
    assignedInspectorStatus: inspector.qcApprovalState,
  });
  this.inspectorSearchTerms.delete(group);
}

openInspectorQuickAdd(group: FormGroup, array: FormArray<FormGroup>): void {
  const inspectionCompanyContactId = group.get('id')?.value;
  if (!inspectionCompanyContactId || inspectionCompanyContactId === '00000000-0000-0000-0000-000000000000') {
    this.toastr.warning('Pick the inspecting company contact for this row first.');
    return;
  }

  const dialogRef = this.dialog.open(InspectorQuickAddDialogComponent, {
    width: '480px',
    data: { inspectionCompanyContactId } as InspectorQuickAddData,
  });

  dialogRef.afterClosed().subscribe((created: Inspector | null) => {
    if (!created) return;
    const cache = this.inspectorsByCompanyContact.get(inspectionCompanyContactId) ?? [];
    this.inspectorsByCompanyContact.set(inspectionCompanyContactId, [...cache, created]);
    this.pickInspector(group, created);
  });
}

/** 0 = Rejected, 1 = Approved, 2 = NoAction(pending) - see Inspector.QCApprovalState. */
qcStatusLabel(status: number | null): string {
  switch (status) {
    case 1: return 'Approved';
    case 0: return 'Rejected';
    case 2: return 'Pending review';
    default: return '';
  }
}

qcStatusClass(status: number | null): string {
  switch (status) {
    case 1: return 'qc-badge qc-approved';
    case 0: return 'qc-badge qc-rejected';
    case 2: return 'qc-badge qc-pending';
    default: return 'qc-badge';
  }
}

/** Only an Inspector-role user (see isInspector, set in the constructor) may attach/register inspectors. */
get canManageInspectors(): boolean {
  return this.isInspector && this.editMode;
}

  generateLocation():FormTypes.FormLocation{
    return this.formBuilder.group({
      address:[''],
      city:[''],
      country:[''],
      state:[''],
      streetLine1:[''],
      streetLine2:[''],
      zipCode:['']
    })
  }
  generateLocationFromValue(location:Address):FormTypes.FormLocation{
    return this.formBuilder.group({
      address:[''],//location.state!,
      city:[location.city!],
      country:[location.country!],
      state:[location.state!],
      streetLine1:[location.streetLine1!],
      streetLine2:[location.streetLine2!],
      zipCode:[location.zipCode!]
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


get inspectionSubjectsFormArray(): FormArray<FormGroup> {
  // if (this.inspectionForm) {
  //   let arr = this.inspectionForm.get('inspectionSubjects') as FormArray<FormGroup>;
  //   if (!arr) {
  //     arr = new FormArray<FormGroup>([]);
  //     this.inspectionForm.addControl('inspectionSubjects', arr);
  //   }
  //   return arr;
  // }
  // // if inspectionForm itself is not initialized, return an empty array just to avoid null errors
  // return new FormArray<FormGroup>([]);
  return  this.inspectionForm.get('inspectionSubjects') as FormArray<FormGroup>;
}



get filteredInspectionSubjectsFormArray() : FormGroup[]  {

     return ((this.inspectionForm?.get('inspectionSubjects') as FormArray)?.controls
      ?.filter(ctrl => ctrl.get('name')?.value?.toString().trim() !== '') || []) as FormGroup[];
   }

get inspectionTypesFormArray(): FormArray<FormGroup> {
  return (<FormArray>this.inspectionForm?.get('inspectionTypes')) as FormArray;
}

get filteredInspectionTypesFormArray() :FormGroup[] {

    return ((this.inspectionForm?.get('inspectionTypes') as FormArray)?.controls
    ?.filter(ctrl => ctrl.get('name')?.value?.toString().trim() !== '') || []) as FormGroup[];
}

get filteredContactsFormArray():FormGroup[]{
   return ((this.inspectionForm?.get('contacts') as FormArray)?.controls
    ?.filter(ctrl => ctrl.get('email')?.value?.toString().trim() !== '' || ctrl.get('id')?.value?.toString().trim() !== '00000000-0000-0000-0000-000000000000') || []) as FormGroup[];

}

get contactsFormArray() :FormArray<FormGroup> {

   if(this.inspectionForm.get('contacts')== null || this.inspectionForm.get('contacts')==undefined)
    this.inspectionForm.addControl('contacts', new FormArray([this.generateContact()]));
    return this.inspectionForm?.get('contacts') as FormArray ?? new FormArray([this.generateContact()]);

}
// filtered view only
get contactsControls(): FormContact[] {

   let arr :FormArray<FormGroup>;
   let showedContacts : FormContact[];
   if (this.inspectionForm) {
    arr = this.inspectionForm.get('contacts') as FormArray<FormGroup>;
    showedContacts= arr.controls.filter((cnt)=>cnt.get('userId').value=='' && (cnt.get('contactType').value==5 || cnt.get('contactType').value==6))  as FormContact[];
    if(arr && (arr.length==0 || showedContacts.length==0))
    {
      (this.inspectionForm.get('contacts') as FormArray<FormGroup>).push(this.generateContact());

    }
    else if (!arr) {
      arr = new FormArray<FormGroup>([this.generateContact()]);
      this.inspectionForm.addControl('contacts', arr);
    }
     showedContacts=(this.inspectionForm.get('contacts') as FormArray<FormGroup>).controls.filter((cnt)=>cnt.get('userId').value=='' && (cnt.get('contactType').value==5 || cnt.get('contactType').value==6)) as FormContact[]
    return showedContacts;
  }
  // if inspectionForm itself is not initialized, return an empty array just to avoid null errors
  return new FormArray([this.generateContact()]).controls;
}


get inspectionLocationFormArray(): FormArray<FormGroup> {
  return this.inspectionForm.get('locations') as FormArray;
    //return this.inspectionForm.get('locations') as FormArray<FormGroup>;
}
get inspectionLocations():FormArray<FormGroup>{
   if (this.inspectionForm) {
    let arr = this.inspectionForm.get('locations') as FormArray<FormGroup>;
    if(arr && arr.length==0)
    {
      arr.value.push(this.generateAddress());
    }
    else if (!arr) {
      arr = new FormArray<FormGroup>([]);
      this.inspectionForm.addControl('locations', arr);
    }
    return arr;
  }
  // if inspectionForm itself is not initialized, return an empty array just to avoid null errors
  return new FormArray<FormGroup>([]);

}

get pidecContactsFormArray(): FormArray<FormContact> {
  const filtered = this.contactsFormArray.controls.filter(ctrl =>
    ctrl.get('contactType')?.value?.toString().trim() === ContactTypes.QC_Manager.toString().trim() ||
    ctrl.get('contactType')?.value?.toString().trim() === ContactTypes.QC_Lead.toString().trim()
  );

  if (!filtered || filtered.length === 0) {
    const pc = this.generateContact();
    pc.get('contactType')?.setValue(ContactTypes.QC_Manager);
    this.contactsFormArray.push(pc);
    return new FormArray<FormContact>([pc]);
  }

  return new FormArray<FormContact>(filtered as FormContact[]);
}
// filtered view only
get pidecContactsControls(): FormContact[] {
  return this.contactsFormArray.controls.filter(ctrl =>
    [ContactTypes.QC_Manager, ContactTypes.QC_Lead].includes(
      ctrl.get('contactType')?.value
    )
  ) as FormContact[];
}

get mcContact() :FormTypes.FormContact | undefined{
  return (this.inspectionForm?.controls?.mcContactPerson as FormTypes.FormContact) ;
}

get ownerContact() :FormTypes.FormContact | undefined{
  return (this.inspectionForm?.controls?.ownerContactPerson as FormTypes.FormContact) ;
}


ConvertGregorianDateToPersianDate(gDate : Date):string {


    try {
    let gregorianDate : Date;
    let formattedGregorian;
    let formattedPersian;

    gregorianDate = new Date(gDate);
      // Format Gregorian date with month name
      const gregorianDay = gregorianDate.getDate();
      const gregorianMonth = gregorianDate.getMonth();
      const gregorianYear = gregorianDate.getFullYear();
      const gregorianMonthName = this.getGregorianMonthName(gregorianMonth);

      // Set formatted Gregorian date with month name
     formattedGregorian = `${gregorianDay} ${gregorianMonthName} ${gregorianYear}`;

      // Convert to Persian
      const { jy, jm, jd } = toJalaali(gregorianYear, gregorianMonth + 1, gregorianDay);

      // Format Persian date with numeric representation for the input
      const numericPersianDate = `${jy}/${String(jm).padStart(2, '0')}/${String(jd).padStart(2, '0')}`;

      // Format Persian date with month name for display
      const persianMonthName = this.getPersianMonthName(jm);
      formattedPersian = `${jd} ${persianMonthName} ${jy}`;

      return formattedPersian;


    } catch (error) {
      this.toastr.error('Error updating Persian date');
      return "";
    } finally {
      //return "";
    }
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
     // this.inspectionForm.get('RequestInspectionPersianDateStart').setValue(numericPersianDate);
      this.CalculateInspectionDuration();
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
      this.inspectionForm.get('requestInspectionPersianDateEnd').setValue(numericPersianDate);
      this.CalculateInspectionDuration();
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

  let startDate = this.inspectionForm.controls.requestInspectionDateStart.value;
  let endDate = this.inspectionForm.controls.requestInspectionDateEnd.value;
//this.inspectionForm.controls.RequestInspectionDueDate
 const start = new Date(startDate).setHours(0, 0, 0, 0);
  const end = new Date(endDate).setHours(0, 0, 0, 0);

  const msInDay = 1000 * 60 * 60 * 24;

  this.inspectionForm.controls.requestInspectionDueDate.setValue(( Math.floor((end - start) / msInDay)+1));
}

  formatPersianDate(year: number, month: number, day: number): string {
    const monthName = this.getPersianMonthName(month);
    return `${year} ${monthName} ${('0' + day).slice(-2)}`;
  }

 onFilesPicked(event: Event): void {
  const input = event.target as HTMLInputElement;
  if (!input.files) return;

  Array.from(input.files).forEach(file => {
    return  this.attachments.push({
      id: crypto.randomUUID(),
      name: file.name,
      description: '',
      downloadUrl: '', // fill after upload
      version: '0',
      content: file,
      docNo:'',
      status:'',
       pdfSize:'',
      edmsId:''
    });
  });

  // Reset input so same file can be re-added if needed
  input.value = '';
 }

 addFile(file: File) {
    this.attachments.push({
      id: crypto.randomUUID(),
      name: file.name,
      description: '',
      downloadUrl: '', // fill after upload
      version: '0',
      content: file,
      docNo:'',
      status:'',
      pdfSize:'',
      edmsId:''
    });
  }
 removeCirculationFile(activeCirculation : Circulation ,index: number) {
    activeCirculation.attachments.splice(index, 1);
  }

 removeFile(index: number) {
    this.attachments.splice(index, 1);
  }

 fetchFile(id){
    this.inspectionService.FetchFileFromServer(id);

  }

  get selectedInspectionSubjects(): InspectionSubject[]
  {
    return this.inspectionForm.get('inspectionSubjects').value.filter((subj)=>subj.name !='') as InspectionSubject[];
  }
  //-------------------End of File Upload Section---------------------

private toUtcISOString(date: Date | null): string | null {
  if (!date) {
    return null;
  }

  const utcDate = new Date(
    Date.UTC(
      date.getFullYear(),
      date.getMonth(),
      date.getDate(),
      date.getHours(),
      date.getMinutes(),
      date.getSeconds(),
      date.getMilliseconds()
    )
  );

  return utcDate.toISOString();
}


  SaveNewInspection():void
  {
    console.log("in save :", this.inspectionForm.getRawValue());
    const rawFormValue = this.inspectionForm.getRawValue();

  //  rawFormValue.contacts =rawFormValue.contacts.concat(rawFormValue.contacts.filter((cnt)=> cnt.contactType == 5 || cnt.contactType==6));
    rawFormValue.contacts = rawFormValue.contacts.concat(rawFormValue.ownerContactPerson);
    rawFormValue.contacts = rawFormValue.contacts.concat(rawFormValue.mcContactPerson);
    rawFormValue.contacts = rawFormValue.contacts.concat(rawFormValue.pidecInspectors);
    rawFormValue.contacts = rawFormValue.contacts.concat(rawFormValue.thirdPartInspectors);


      const filteredInspectionForm = {
      ...rawFormValue,

       createdDate : rawFormValue.createdDate ?? new Date(Date.now()),
      createdBy:rawFormValue.createdBy ?? this.currentAppState.currentuser().userName,

      modifiedDate:new Date(Date.now()),
      modifiedBy:this.currentAppState.currentuser().userName,

      inspectionSubjects: this.inspectionForm
        .get('inspectionSubjects')
        .value.filter((subject) => subject.itpRowNo !='' || subject.name !='') as InspectionSubject[]
        ,
        inspectionTypes: this.inspectionForm
          .get('inspectionTypes')
          .value.filter((inspetionType) => inspetionType.selected && inspetionType.name !='') as InspectionType[]// Keep only selected subjects
        ,
        locations: this.inspectionForm.get('locations').value.filter((location)=>location.city !='' && location.state != '' && location.state !=''),

        contacts:rawFormValue.contacts
        //this.inspectionForm.get('contacts').value
        .filter((contact) => contact.email !='' && contact.contactType !=ContactTypes.Undefined) as Contact[]
          ,

        requestInspectionDateStart: this.toUtcISOString(rawFormValue.requestInspectionDateStart),
        requestInspectionDateEnd : this.toUtcISOString(rawFormValue.requestInspectionDateEnd),
        // attachments:this.inspectionForm
        //   .get('attachments')
        //   .value.filter((attachment) => attachment.name !='' && attachment.content.size !=0) as Attachment[]
        //    ,
        //  circulations: [this.currentCirculation]
    };


   // filteredInspectionForm.attachments = null;
     this.formData.delete('inspection');
     this.formData.delete('project');
     this.formData.delete('currentCirculation');
     this.formData.delete('Attachments');
     this.formData.delete('AttachmentIds');
     this.formData.delete('AttachmentNames');
     this.formData.delete('AttachmentVersions');
     this.formData.delete('AttachmentDescriptions');
     this.formData.delete('AttachmentDocNos');
     this.formData.delete('AttachmentStatuses');
      this.formData.delete('HasContent');

    // this.formData.append('attachments',JSON.stringify(this.Attachments));

    if(filteredInspectionForm.notificationNumber.indexOf(".")<0)
      filteredInspectionForm.notificationNumber = filteredInspectionForm.notificationNumber.concat(".",filteredInspectionForm.revNumber);

   this.formData.append('inspection',JSON.stringify(filteredInspectionForm));
   this.formData.append('project',JSON.stringify(this.currentProject ))


    if(this.workflow && this.workflow.currentCirculation != null && this.workflow.currentCirculation != undefined)
   {
    this.workflow.currentCirculation.performedActionDateGregorian = null;
      this.workflow.currentCirculation.attachments = null;
      if((this.workflow.currentCirculation.previousActionType!= undefined) && (this.workflow.currentCirculation.previousActionType == 3 || this.workflow.currentCirculation.previousActionType != null || Number.parseInt(filteredInspectionForm.revNumber)>0))
      {
        this.workflow.currentCirculation.actionType = 1;
      }

      this.formData.append('currentCirculation',JSON.stringify(this.workflow.currentCirculation));
    }
    if(this.workflow && this.workflow.attachments)
    {
  this.workflow.attachments.forEach((attachment)=>{
     const name = attachment.name;
 if (name && name.trim() !== '') {

    this.formData.append('AttachmentIds', attachment.id);
    this.formData.append('AttachmentNames', attachment.name);
    this.formData.append('AttachmentVersions', attachment.version);
    this.formData.append('AttachmentDescriptions', attachment.description);
    this.formData.append('AttachmentDocNos',attachment.docNo);
    this.formData.append('AttachmentStatuses',attachment.status);

    if (attachment.content) {
    this.formData.append('Attachments', attachment.content);
    this.formData.append('HasContent', 'true');
  } else {
    // 🔥 placeholder to keep index alignment
    this.formData.append(
      'Attachments',
     new File(['fakeContent'], 'placeholder.txt', {
    type: 'application/octet-stream'
  }));
    this.formData.append('HasContent', 'false');
  }

  }
  }
);
    }
    else ///this is new inspection request that does not have any workflow yet
    {
this.attachments.forEach((attachment)=>{
     const name = attachment.name;
 if (name && name.trim() !== '') {

    this.formData.append('AttachmentIds', attachment.id);
    this.formData.append('AttachmentNames', attachment.name);
    this.formData.append('AttachmentVersions', attachment.version);
    this.formData.append('AttachmentDescriptions', attachment.description);
    this.formData.append('AttachmentDocNos',attachment.docNo);
    this.formData.append('AttachmentStatuses',attachment.status);

    if (attachment.content) {
    this.formData.append('Attachments', attachment.content);
    this.formData.append('HasContent', 'true');
  } else {
    // 🔥 placeholder to keep index alignment
    this.formData.append(
      'Attachments',
     new File(['fakeContent'], 'placeholder.txt', {
    type: 'application/octet-stream'
  }));
    this.formData.append('HasContent', 'false');
  }

  }
  }
);
    }

this.inspectionService
  .saveNewInspectionFull(this.formData)
  .subscribe({
    next: (result) => {
        this.inspectionForm.patchValue({

        id:result.inspection.id,
        contacts:result.inspection.contacts,
        inspectionSubjects: result.inspection.inspectionSubjects,
        inspectionTypes:result.inspection.inspectionTypes,
        locations: result.inspection.locations

      })

if(this.workflow )
{
      this.workflow.currentCirculation = result.currentCirculation;

  if(this.workflow.currentCirculation != null){
    this.workflow.currentCirculation.attachments.forEach((ra)=>{
     // this.Attachments.push(this.generateAttachmentFromValue(ra));
    })
 }
}
else
{

}
  this.toggleEditMode(false);

    },
    error: (error) => {
      this.toastr.success(error +" occured, please contact with Administrator","Send Error");
    },
    complete: () => {
      console.log('Request completed');
    }
  });


  }

  DraftNewInspection():void
  {
    console.log("in draft :", this.inspectionForm.getRawValue());
     const rawFormValue = this.inspectionForm.getRawValue();
    rawFormValue.contacts = rawFormValue.contacts.concat(rawFormValue.ownerContactPerson);
    rawFormValue.contacts = rawFormValue.contacts.concat(rawFormValue.mcContactPerson);
    rawFormValue.contacts = rawFormValue.contacts.concat(rawFormValue.pidecInspectors);
    rawFormValue.contacts = rawFormValue.contacts.concat(rawFormValue.thirdPartInspectors);

    const filteredInspectionForm = {
      ...this.inspectionForm.getRawValue(),

      createdDate : new Date(Date.now()),
      createdBy:this.currentAppState.currentuser().userName,
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
        contacts:this.inspectionForm
          .get('contacts')
          .value.filter((contact) => contact.email !='' && contact.contactType !=ContactTypes.Undefined) as Contact[]
        //   ,
        // attachments:this.inspectionForm
        //   .get('attachments')
        //   .value.filter((attachment) => attachment.name !='' && attachment.content.size !=0) as Attachment[]
    };

     this.formData.delete('inspection');
     this.formData.delete('project');
     this.formData.delete('currentCirculation');
     this.formData.delete('Attachments');
     this.formData.delete('AttachmentIds');
     this.formData.delete('AttachmentNames');
     this.formData.delete('AttachmentVersions');
     this.formData.delete('AttachmentDescriptions');
     this.formData.delete('AttachmentDocNos');
     this.formData.delete('AttachmentStatuses');
     this.formData.delete('HasContent');

   this.formData.append('inspection',JSON.stringify(filteredInspectionForm));
   this.formData.append('project',JSON.stringify(this.currentProject ))
   this.formData.append('currentCirculation',JSON.stringify(this.workflow.currentCirculation));

  this.attachments.forEach((attachment)=>{
     const name = attachment.name;
 if (name && name.trim() !== '') {

    this.formData.append('AttachmentIds', attachment.id);
    this.formData.append('AttachmentNames', attachment.name);
    this.formData.append('AttachmentVersions', attachment.version);
    this.formData.append('AttachmentDescriptions', attachment.description);
     this.formData.append('AttachmentDocNos',attachment.docNo);
    this.formData.append('AttachmentStatuses',attachment.status);

    if (attachment.content) {
    this.circulationData.append('Attachments', attachment.content);
    this.circulationData.append('HasContent', 'true');
  } else {
    // 🔥 placeholder to keep index alignment
    this.circulationData.append(
      'Attachments',
      new Blob(['fakeContent'], { type: 'application/octet-stream' }),
      'fakeContent'
    );
    this.circulationData.append('HasContent', 'false');
  }

  }
  });


    this.inspectionService.draftNewInspectionFull(this.formData).subscribe((result)=>{


      this.inspectionForm.patchValue({
        //...this.inspectionForm.getRawValue(),
        id:result.inspection.id,
        contacts:result.inspection.contacts,
        inspectionSubjects: result.inspection.inspectionSubjects,
        inspectionTypes:result.inspection.inspectionTypes
      })


      this.currentCirculation = result.currentCirculation;

 if(this.currentCirculation != null){
    this.currentCirculation.attachments.forEach((ra)=>{
     // this.Attachments.push(this.generateAttachmentFromValue(ra));
    })
 }
  this.toggleEditMode(false);
    });
  }

//   showHistoryForm(){
// const historyUrl = this.router.serializeUrl(
//       this.router.createUrlTree(['inspection-print'], {
//         queryParams: {
//           requestId: this.RequestId // Pass the key, not the data itself,

//           // Or you could use a unique ID and fetch data on the print page itself
//           // inspectionId: inspectionData.id // If inspectionData has an ID
//         }
//       })
//     );
//   // Open in a new tab/window
//   const newWindow = window.open(historyUrl, '_blank','width=1000,height=800');
//   }

  showHistoryForm(){
    // Previously navigated to 'inspection-print', but that component only reads
    // dataKey/projKey query params (set by printForm()) - it never reads requestId,
    // so this button opened a page it could never populate. Routes to the real
    // history page instead; adjust the path/route name to match your routing module.
    const historyUrl = this.router.serializeUrl(
      this.router.createUrlTree(['inspection-history', this.RequestId])
    );
    const newWindow = window.open(historyUrl, '_blank', 'width=1000,height=800');
  }


printForm() {

   let inspection = this.inspectionForm.getRawValue();
   // Create the target route relative to current activated route
   const urlTree = this.router.createUrlTree(
    ['../../..'+ environment.prefix + 'inspection-print'],
    { relativeTo: this.activatedRoute }
  );
  // Serialize it to an actual browser path
  const url = this.router.serializeUrl(urlTree);

   // 👇 you can use localStorage, sessionStorage, or query params
  //  sessionStorage.setItem('inspectionData', JSON.stringify(inspection));
    const encodedInspectionData = encodeURIComponent(JSON.stringify(inspection));

    // Generate a unique key for this specific data
    const storageDataKey = `inspectionData_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`;

     const storageProjKey = `inspectionData_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`;

    // Store the data in sessionStorage
    try {
      sessionStorage.setItem(storageDataKey, JSON.stringify(inspection));

        sessionStorage.setItem(storageProjKey, JSON.stringify(inspection.order.project));
    } catch (e) {
      console.error('Error saving to sessionStorage:', e);
      // Handle potential quota exceeded errors or other storage issues
      return;
    }

        const urlSegments = this.router.url.split('/').filter(seg => seg !== '');
    // urlSegments e.g., ['pix', 'editform', '49784C11-...', 'D6838324-...']

    // First segment is the variable part (e.g., 'pix')
    const variableSegment = urlSegments[0]; // 'pix'

    let urlFormat : string ;
    // if(environment.apiUrl.trim().length == 0)
    //   urlFormat = variableSegment+'/'+'inspection-print';
    // else
    //   urlFormat= environment.apiUrl+'/'+variableSegment+'/'+'inspection-print';


      //urlFormat = '../../'+variableSegment+'/'+'inspection-print';
      urlFormat = '../../'+'inspection-print';

  //  urlFormat = window.location.origin + this.router.url;

const printUrl = this.router.serializeUrl(


      this.router.createUrlTree([urlFormat], {
        queryParams: {
          dataKey: storageDataKey, // Pass the key, not the data itself,
          projKey: storageProjKey
          // Or you could use a unique ID and fetch data on the print page itself
          // inspectionId: inspectionData.id // If inspectionData has an ID
        }
      })
    );
  // Open in a new tab/window
  let cleanUrl = printUrl.replace(/^\//, '');
  const newWindow = window.open(cleanUrl, '_blank','width=1000,height=800');

  // Optionally transfer state data (because Angular's `state` doesn't cross windows)

}

AddInternalCorrespondenceRow(parentCirculation:Circulation)
{
  let newChildCirculation : Circulation = this.generateNewCirculation();

    newChildCirculation.createdBy = this.currentUserId;
    let MaxSequenceNo =  Math.max(...parentCirculation.childCirculations.filter((circ)=> circ.id != '' && circ.id != '00000000-0000-0000-0000-000000000000').map(x => x.sequenceNo));
    newChildCirculation.sequenceNo = MaxSequenceNo+1;
    newChildCirculation.revisionId = parentCirculation.revisionId;
    parentCirculation.childCirculations .push(newChildCirculation);

}

StartInternalCorrespondence (internalCirculation: Circulation,parentCirculation: Circulation) {
this.internalCorrespondenceFormData = new FormData();
parentCirculation.performedActionDateGregorian = new Date(Date.now());
this.internalCorrespondenceFormData.append('Contributor',JSON.stringify(parentCirculation));
this.internalCorrespondenceFormData.append('AssigneeCirculations',JSON.stringify(parentCirculation.childCirculations));
this.inspectionService.ContributeAccessRightsToEditTask(this.internalCorrespondenceFormData).subscribe(
  {
    next : (result)=>{
       parentCirculation.childCirculations =[parentCirculation.childCirculations,...result];
      console.log(result);},
    error:err=>{
      console.log(err);
    }
  })
}

}
