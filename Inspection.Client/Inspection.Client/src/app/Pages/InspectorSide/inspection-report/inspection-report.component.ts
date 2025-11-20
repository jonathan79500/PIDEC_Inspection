import { toJalaali } from 'jalaali-js';
import { currentstateStore } from './../../../Stores/currentstate.store';
import { InspectionReport } from './../../../Core/Models/InspectionReport.model';
import { CommonModule } from '@angular/common';
import { Component, inject, Input, input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { FormAddress, FormAttachment, FormContact, FormInspectionSubject, FormInspectionType, FormLocation, InspectionForm, InspectionReportForm } from '../../../Core/FormTypes/InspectionFormTypes';
import { ActivatedRoute, Router } from '@angular/router';
import { InspectionReportService } from '../../../Core/Services/inspectionReport.service';
import { Inspection } from '../../../Core/Models/Inspection.model';
import { ObjectTypes } from '../../../Core/Models/ObjectTypes.enum';
import { Address } from '../../../Core/Models/Address.model';
import { Contact } from '../../../Core/Models/Contact.model';



@Component({
  selector: 'app-inspection-report',
  imports: [FormsModule,ReactiveFormsModule,CommonModule],
  templateUrl: './inspection-report.component.html',
  styleUrl: './inspection-report.component.css'
})
export class InspectionReportComponent implements OnInit{

  attachedDocs: File[];
  formData : FormData;
  inspectionReportForm: InspectionReportForm;
  RelatedInspectionForm :InspectionForm;
  @Input() relatedInspectionNotification:  InspectionForm;
  currentAppState = inject(currentstateStore);
formattedPersianDate:string

  constructor(private activeRoute : ActivatedRoute,private inspectionReportService: InspectionReportService, private formBuilder: FormBuilder) {
    let DateNow = new Date(Date.now());
    const gregorianDate: Date = DateNow;
    const { jy, jm, jd } = toJalaali(gregorianDate.getFullYear(), gregorianDate.getMonth() + 1, gregorianDate.getDate());

    this.formattedPersianDate = `${jy}/${jm}/${('0' + jd).slice(-2)}`;

  }

  ngOnInit(): void {
      this.InitializeForm();
  }

  InitializeForm():void
  {
    this.activeRoute.paramMap.subscribe(params=>{
      if(params.keys[0]=='id')
      {
        var InspectionReportId = params.get('id').toString();
          let strReadInspectionReport = this.formData.get("readInspectionReport");
                       let readInspectionReportObj:InspectionReport;
                       if(strReadInspectionReport != null)
                       {
                          readInspectionReportObj = JSON.parse(strReadInspectionReport.toString());
                          this.inspectionReportForm =  this.GenerateInspectionReportFormFromValue(readInspectionReportObj);
                       }
                       else
                       {
                         this.inspectionReportService.getInspectionReportById(InspectionReportId).subscribe((inspectionReport)=>{
                           this.inspectionReportForm = this.GenerateInspectionReportFormFromValue(inspectionReport);
                           });
                       }
      }
    })
  }

  GenerateInspectionReportForm():InspectionReportForm
  {
    return this.formBuilder.group({
      currentStatus: -1,
      inspectionReportId : '00000000-0000-0000-0000-000000000000',
      inspectionReportNumber : '',
      inspectionReportAttachments:this.formBuilder.array<FormAttachment>([this.generateAttachment()]),
      inspectorContacts:this.formBuilder.array<FormContact>([this.generateContact()]),
      relatedInspectionNotificationId:'',
      relatedInspectionNotification: this.relatedInspectionNotification,
      createdBy:[this.currentAppState.currentuser().userName],
      issueDate: new Date(Date.now()),
      persianIssueDate:this.formattedPersianDate,
      ReportNotes:'',
      ReportType: 0
    });
  }

 GenerateInspectionReportFormFromValue(inspectionReport: InspectionReport):InspectionReportForm
  {
     return this.formBuilder.group({
      currentStatus: inspectionReport.currentStatus,
      inspectionReportId : inspectionReport.inspectionReportId,
      inspectionReportNumber : inspectionReport.inspectionReportNumber,
      inspectionReportAttachments:this.formBuilder.array<FormAttachment>([this.generateAttachment()]),
      inspectorContacts:this.formBuilder.array<FormContact>([this.generateContact()]),
      relatedInspectionNotificationId:inspectionReport.relatedInspectionNotificationId,
      relatedInspectionNotification: this.relatedInspectionNotification,
      createdBy:inspectionReport.createdBy,
      issueDate: inspectionReport.issueDate,
      persianIssueDate:inspectionReport.persianIssueDate,
      ReportNotes:inspectionReport.ReportNotes,
      ReportType: inspectionReport.ReportType
    });
  }


    generateLocationFromValue(location:Address):FormLocation{
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

    generateAttachment():FormAttachment{
      return this.formBuilder.group({
        id:'00000000-0000-0000-0000-000000000000',
        version:'' ,
        name:'',
        content: []
      });
    }

      generateAddress():FormAddress{
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

     generateContact():FormContact{
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
          belongedTo: ObjectTypes.InspectionReport
        })
      }
   generateAddressFromAddressValue(address:Address):FormAddress{
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

        generateContactFromContactValue(contact:Contact):FormContact{
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
        generateLocation():FormLocation{
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

onDocPicked(event: any) {

  this.attachedDocs = event.target.files;
  this.formData.delete('attachments');

  for(var i = 0; i<this.attachedDocs.length;i++)
    this.formData.append('attachments',this.attachedDocs[i]);


  console.log(this.attachedDocs);
}

onPackingListPicked(event: any) {

  this.attachedDocs = event.target.files;
  this.formData.delete('packingLists');

  for(var i = 0; i<this.attachedDocs.length;i++)
    this.formData.append('packingLists',this.attachedDocs[i]);


  console.log(this.attachedDocs);
}

SaveReport():void{
this.inspectionReportService.saveNewInspectionReport(this.inspectionReportForm.)
}
}

