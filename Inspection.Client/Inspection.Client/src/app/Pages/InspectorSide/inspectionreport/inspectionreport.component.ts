import { FormContact } from './../../../Core/FormTypes/InspectionFormTypes';
import { Person } from './../../../Core/Models/Person.model';
import { CommonModule } from '@angular/common';
import { Component, effect, inject, Inject, Injector, Input, OnInit, runInInjectionContext } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatChipsModule } from '@angular/material/chips';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatTooltipModule } from '@angular/material/tooltip';
import { NgPersianDatepickerModule } from 'ng-persian-datepicker';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import * as FormTypes from '../../../Core/FormTypes/InspectionFormTypes';
import { InspectionService } from '../../../Core/Services/inspection.service';
import { Attachment } from '../../../Core/Models/Attachment.model';
import { InspectionReportService } from '../../../Core/Services/inspectionReport.service';
import { Circulation } from '../../../Core/Models/Circulation.model';
import { IsActiveForUserPipe } from "../../../Core/General/Pipes/is-active-for-user.pipe";
import { currentstateStore } from '../../../Stores/currentstate.store';
import { InspectionReport } from '../../../Core/Models/InspectionReport.model';
import { ObjectTypes } from '../../../Core/Models/ObjectTypes.enum';

@Component({
  selector: 'app-inspectionreport',
  imports: [CommonModule, ReactiveFormsModule, FormsModule, MatChipsModule, MatButtonModule, MatIconModule, MatCardModule, NgPersianDatepickerModule, MatDatepickerModule, MatFormFieldModule, MatTooltipModule, MatInputModule, MatAutocompleteModule, MatSelectModule, MatCheckboxModule, IsActiveForUserPipe],
  templateUrl: './inspectionreport.component.html',
  styleUrl: './inspectionreport.component.css'
})
export class InspectionreportComponent implements OnInit{
  reportForm!: FormTypes.InspectionReportForm;
  reportNumber!: string;
  editMode: boolean = true;
  formData: FormData;
  currentCirculation :Circulation;
  attachments: Attachment[] = [];
  currentAppState = inject(currentstateStore);
   injector = inject(Injector);
  actionTypes = ['مشروط', 'نتایج بر اساس جدول 1 می باشد', 'تایید', 'مردود','موارد در انتظار تعیین تکلیف(مطابق جدول 9)','عدم انطباث مشاهده شد(مطابق جدول 8)'];
  circulationData: FormData;
  requestCirculations: Circulation[];
  relatedNotificationNumber : string;
  lastReportVersion :string = "001";

  constructor(
    private fb: FormBuilder,
    private inspectionReportService : InspectionReportService,
    private dialogRef: MatDialogRef<InspectionreportComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { notificationNumber: string; notificationDate: Date ;reportsVersions:InspectionReport[]}
  ) {

      this.formData = new FormData();
      this.relatedNotificationNumber = data.notificationNumber;
  }

  ngOnInit() {
    this.reportNumber = this.generateReportNumber();

    this.reportForm = this.fb.group({
      inspectionReportId: [''],
      inspectionReportNumber: [''],
      currentStatus: [0],
      inspectorContacts: this.fb.array<FormTypes.FormContact>([this.generateContact()]),
      //relatedInspectionNotification: ,
      relatedInspectionNotificationId: [''],
      inspectionReportAttachments: this.fb.array<FormTypes.FormAttachment>([this.generateAttachment()]),
      requestIdOfRelatedInspectionNotification:[''],
      issueDate: [new Date(Date.now())],
      inspectionDate: [new Date(Date.now())],
      revisionId:[''],
      persianIssueDate: [''],
      createdBy: [''],
      reportNotes: [''],
      reportType: [0],
      inspectors: this.fb.array<FormTypes.FormPerson>([this.generatePerson()])

    })// as FormTypes.InspectionReportForm;

    //   inspectionDate: [''],
    //   comment: [''],
    //   ...this.actionTypes.reduce((acc, _, i) => {
    //     acc['action_' + i] = [false];
    //     return acc;
    //   }, {} as any),
    //   inspectors: this.fb.array<FormTypes.FormContact>([this.generateContact()]),
    //   inspectionReportAttachments : this.fb.array<FormTypes.FormAttachment>([this.generateAttachment()])
    // }) as FormTypes.InspectionReportForm;

if(this.data.reportsVersions != null && this.data.reportsVersions.length>0)
{

  const reportIssueDates = this.data.reportsVersions.map((rpt)=> rpt.issueDate.getDate());
  const maxDate = Math.max(...reportIssueDates);
  const lastReport = this.data.reportsVersions.filter((rpt)=>rpt.issueDate.getDate()== maxDate)[0] as InspectionReport;
  this.FillReportForm(lastReport);
}

  }

   generateAddress():FormTypes.FormAddress{
         return this.fb.group({
          id:['00000000-0000-0000-0000-000000000000'],
           city:[''],
           country:[''],
           state:[''],
           streetLine1:[''],
           streetLine2:[''],
           zipCode:['']
         })
       }

    generateContact():FormTypes.FormContact{
      return this.fb.group({
        id:'00000000-0000-0000-0000-000000000000',
        note:'',
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

FillReportForm(lastReport : InspectionReport){
  this.reportForm.patchValue({
     inspectionReportId:lastReport.inspectionReportId,
      inspectionReportNumber: lastReport.inspectionReportNumber,
      currentStatus: 0,
      inspectorContacts: null,
      revisionId : lastReport.revisionId,
      requestIdOfRelatedInspectionNotification: lastReport.requestIdOfRelatedInspectionNotification,
      inspectionDate : lastReport.inspectionDate,
      //relatedInspectionNotification: lastReport.relatedInspectionNotification,
      relatedInspectionNotificationId: lastReport.relatedInspectionNotificationId,
      inspectionReportAttachments: null,
      issueDate: new Date(Date.now()),
      persianIssueDate: lastReport.persianIssueDate,
      createdBy: lastReport.createdBy,
      reportNotes: lastReport.ReportNotes,
      reportType: lastReport.ReportType
  });
}
toggleEditMode(on: boolean) {
  this.editMode = on;

  const inspectionPersons = this.reportForm.get('inspectors');
  const comment = this.reportForm.get('reportNotes');
  const inspectionDate = this.reportForm.get('issueDate');
  const attachments = this.reportForm.get('inspectionReportAttachments');

  inspectionDate?.disable();

  if (on) {

    comment?.enable();
    inspectionPersons?.enable();
    inspectionDate?.enable();
    attachments?.enable();

   } else {

    comment?.disable();
    inspectionPersons?.disable();
    inspectionDate?.disable();
    attachments?.disable();
   }
}

  get personsFormArray(): FormArray<FormGroup> {
    if (this.reportForm) {
      let arr = this.reportForm.get('inspectors') as unknown as FormArray<FormGroup>;
      if (!arr) {
        arr = new FormArray<FormGroup>([]);
        //this.reportForm.addControl('inspectors', arr);
      }
      return arr;
    }
    // if inspectionForm itself is not initialized, return an empty array just to avoid null errors
    return new FormArray<FormGroup>([]);
  }


  get personsControls(): FormTypes.FormPerson[] {
    return this.personsFormArray.controls  as FormTypes.FormPerson[];
  }


    generatePerson():FormTypes.FormPerson{
      return this.fb.group({
        id:['00000000-0000-0000-0000-000000000000'],
        firstName:[''],
        lastName:[''],
        email:[''],
        name:[''],
        phone:[''],
        position:['']
      })
    }

    generateAttachment():FormTypes.FormAttachment{
        return this.fb.group({
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

    generateReportNumber(): string {
    return  this.relatedNotificationNumber+"IVR-"+this.lastReportVersion;
    }

    fetchFile(id){

  this.inspectionReportService.FetchFileFromServer(id);

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

 removeFile(index: number) {
    this.attachments.splice(index, 1);
  }

  removePresentPerson(person: FormTypes.FormPerson): void {
    const idx = this.personsFormArray.controls.indexOf(person);
    if (idx >= 0) {
      this.personsFormArray.removeAt(idx);
    }
  }
    addNewPresentPerson():void{

     let rawPerson = this.fb.group({
        email:[''],
        name:[''],
        phone:[''],
       position:['']
      })
      this.personsFormArray.push(rawPerson);
    }
   isActiveForCurrentUser(circulation: Circulation) : boolean{
  if(circulation.performerContactPerson.id == this.currentCirculation.performerContactPerson.id && circulation.actionType ==null)
    return true;
  return false;
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

  }
//-----------------------Comments Section Actions-----------------------------------
showAll = true;

onAction(circulation: Circulation,actionType: number) {

  if(actionType == 1)
    {

     circulation.actionType = 1;
    circulation.comment =  circulation.comment;
    }


     this.circulationData.delete('Attachments');
     this.circulationData.delete('AttachmentIds');
     this.circulationData.delete('AttachmentNames');
     this.circulationData.delete('AttachmentVersions');
     this.circulationData.delete('AttachmentDescriptions');
     if(circulation.attachments !=null)
     {
   circulation.attachments.forEach((attachment)=>{
    let name=''
    if(attachment != null)
      name = attachment.name;
 if (name && name.trim() !== '') {
    this.circulationData.append('Attachments', attachment.content); // File object
    this.circulationData.append('AttachmentIds', attachment.id);
    this.circulationData.append('AttachmentNames', attachment.name);
    this.circulationData.append('AttachmentVersions', attachment.version);
    this.circulationData.append('AttachmentDescriptions', attachment.description);
  }
  });
}
this.circulationData.delete("circulation");
  circulation.attachments = null;
   this.circulationData.append('circulation',JSON.stringify(circulation));



  this.inspectionReportService.saveCirculationData(this.circulationData).subscribe((savedCirculation)=>{
    if(savedCirculation)
    {
      this.currentCirculation.actionType= savedCirculation.actionType;
      this.currentCirculation.performedActionDate = savedCirculation.performedActionDate;
      this.currentCirculation.performedActionDateGregorian = savedCirculation.performedActionDateGregorian;
      this.currentCirculation.attachments = savedCirculation.attachments;
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

 shouldShowButton(circulation: Circulation, action: 'approve' | 'reject' | 'upload'): boolean {
  if (!this.editMode || this.reportForm.get('id').value == '00000000-0000-0000-0000-000000000000') return false;
  if (!this.isActiveForCurrentUser(circulation)) return false;

  return action === 'approve' || action === 'reject' || action === 'upload';
 }
//---------------------End of Comments Section Actions-------------------------------

  submitReport() {
    const reportValues = this.reportForm.getRawValue();
    const reportData = {
      ...reportValues,
      currentStatus : 1,
      inspectorContacts: null,
      inspectionReportAttachments : null
    };

     this.formData.delete('inspectionReport');
     this.formData.delete('currentCirculation');
     this.formData.delete('Attachments');
     this.formData.delete('AttachmentIds');
     this.formData.delete('AttachmentNames');
     this.formData.delete('AttachmentVersions');
     this.formData.delete('AttachmentDescriptions');
     this.formData.delete('AttachmentDocNos');
     this.formData.delete('AttachmentStatuses');

   this.formData.append('inspectionReport',JSON.stringify(reportData));
   //this.formData.append('currentCirculation',JSON.stringify(this.currentCirculation));

   this.attachments.forEach((attachment)=>{
     const name = attachment.name;
 if (name && name.trim() !== '') {
    this.formData.append('Attachments', attachment.content); // File object
    this.formData.append('AttachmentIds', attachment.id);
    this.formData.append('AttachmentNames', attachment.name);
    this.formData.append('AttachmentVersions', attachment.version);
    this.formData.append('AttachmentDescriptions', attachment.description);
    this.formData.append('AttachmentDocNos',attachment.docNo);
    this.formData.append('AttachmentStatuses',attachment.status);
  }
  });

      this.inspectionReportService.saveNewInspectionReport(this.formData).subscribe((result)=>{

      this.currentCirculation = result.currentCirculation;

 if(this.currentCirculation != null){
    this.currentCirculation.attachments.forEach((ra)=>{
     // this.Attachments.push(this.generateAttachmentFromValue(ra));
    })
 }
  this.toggleEditMode(false);
    });

    console.log('Submitting report:', reportData);
    this.dialogRef.close(reportData);
  }
  close(){
    this.dialogRef.close();
  }
}
