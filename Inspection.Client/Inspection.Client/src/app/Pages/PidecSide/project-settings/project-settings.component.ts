import { Contact } from './../../../Core/Models/Contact.model';
import { Address } from './../../../Core/Models/Address.model';
import { currentstateStore } from './../../../Stores/currentstate.store';
import { CommonModule } from '@angular/common';
import { Component, effect, inject, Injector, OnInit, runInInjectionContext } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import * as FormTypes from '../../../Core/FormTypes/InspectionFormTypes';
import { Init } from 'v8';
import { BasicInfoService } from '../../../Core/Services/basic-info.service';
import { Project } from '../../../Core/Models/Project.model';
import { ContactTypes } from '../../../Core/Models/ContactTypes.enum';

import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { ContactpersonSelectorComponent } from '../../contactperson-selector/contactperson-selector.component';
import { ObjectTypes } from '../../../Core/Models/ObjectTypes.enum';


@Component({
    selector: 'app-project-settings',
    imports: [CommonModule,ReactiveFormsModule,FormsModule,MatDialogModule],
    templateUrl: './project-settings.component.html',
    styleUrl: './project-settings.component.css'
})
export class ProjectSettingsComponent implements OnInit{

  projectForm:FormTypes.FormProject;
 currentState = inject(currentstateStore);
  injector = inject(Injector);
  readonly dialog = inject(MatDialog);
    currentContacts : Contact[];

  /**
   *
   */
  constructor(private formBuilder:FormBuilder,private basicService:BasicInfoService) {

  }

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm():void{
    runInInjectionContext(this.injector,()=>{
      effect(()=>{
        let currentProject :Project= this.currentState.currentproject();
        this.basicService.getProjectDetailByProjectId(currentProject.id).subscribe((projectDetail)=>{
            this.generateProjectFromValue(projectDetail);
        })

        })
    });
  }
  generateProjectFromValue(project:Project):void{

    this.projectForm = this.formBuilder.group({
      id:project.id,
      fSId:project.fSId,
      createdBy:project.createdBy,
      createdDate:project.createdDate,
      description:project.description,
      mcContacts: this.formBuilder.array<FormTypes.FormContact>([this.generateContact()]),
      modifiedBy: project.modifiedBy,
      modifiedDate:project.modifiedDate,
      number:project.number,
      pidecQCContacts:this.formBuilder.array<FormTypes.FormContact>([this.generateContact()]),
      ownerContacts:this.formBuilder.array<FormTypes.FormContact>([this.generateContact()]),
      pidecInspectors: this.formBuilder.array<FormTypes.FormContact>([this.generateContact()]),
      tpInspectors: this.formBuilder.array<FormTypes.FormContact>([this.generateContact()]),
      title:project.title,
      //vendors : this.formBuilder.array<FormTypes.FormVendor>([this.generateVendor()]),

    })

     if(project.pidecQCContacts !=undefined)
          project.pidecQCContacts!.forEach((relContact)=>{
            this.pidecQCContacts.push(this.generateContactFromContactValue(relContact));
          });

        if(project.mcContacts !=undefined)
          project.mcContacts!.forEach((relContact)=>{
            this.mCContacts.push(this.generateContactFromContactValue(relContact));
          });

        if(project.ownerContacts != undefined)
          project.ownerContacts!.forEach((relContact)=>{
            this.ownerContacts.push(this.generateContactFromContactValue(relContact));
          });

        if(project.pidecInspectors !=undefined)
          project.pidecInspectors!.forEach((relContact)=>{
            this.pidecInspectorContacts.push(this.generateContactFromContactValue(relContact));
          });

        if(project.tpInspectors != undefined)
          project.tpInspectors!.forEach((relContact)=>{
            this.tPInspectorContacts.push(this.generateContactFromContactValue(relContact));
          });

  }

    generateVendor():FormTypes.FormVendor{

      return this.formBuilder.group({
        domainId:0,
        id:'',
        name:'',
       orders:this.formBuilder.array<FormTypes.FormOrder>([]),
        // project:this.generateProject(),
        project:{},
        //projects:this.formBuilder.array<FormProject>([this.generateProject()]),
        projects:this.formBuilder.array<FormTypes.FormProject>([]),
        userName:''

      })
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
          id:address.id!=''? address.id :'00000000-0000-0000-0000-000000000000',
          city:address.city,
          country:address.country,
          state:address.state,
          streetLine1:address.streetLine1,
          streetLine2:address.streetLine2,
          zipCode:address.zipCode
        })
      }

       generateContact():FormTypes.FormContact{
          return this.formBuilder.group({
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

  generateContactFromContactValue(contact:Contact):FormTypes.FormContact{
    return this.formBuilder.group({
      id:contact.id!=''? contact.id :'00000000-0000-0000-0000-000000000000',
      note:contact.note,
      cc:contact.cc,
      email:contact.email,
      fax:contact.fax,
      name:contact.name,
      phone:contact.phone,
      contactType:contact.contactType,
      mobile:contact.mobile,
      homePage:contact.homePage,
      isVisible:[true],
      address:contact!.address != null ? this.generateAddressFromAddressValue(contact.address):this.generateAddress(),
      userId:contact.userId,
      password:contact.password,
      belongedTo: ObjectTypes.Undefined
    })
  }


  openPIDECQCContactSelector():void{

        let currentAddedContacts = this.pidecQCContacts?.getRawValue().filter((contact)=> (contact.contactType==ContactTypes.QC_Lead || contact.contactType==ContactTypes.QC_Manager) && (contact.email !='' || [] )) as Contact[];
         let exceptionContacts : Contact[];
        const dialogRef = this.dialog.open(ContactpersonSelectorComponent, {
          data: {ToContacts: {currentAddedContacts,exceptionContacts} ,ContactType:ContactTypes.QC_Manager,timeStamp: new Date().getTime()},panelClass: 'messageDialog', disableClose: true, autoFocus: false
          ,width:'700px', maxWidth:'140vw',height:'60vh',minHeight:'300px'
        });
       // dialogRef.afterOpened().subscribe(()=>dialogRef.componentInstance.initializeForm());

        dialogRef.afterClosed().subscribe(result => {

          if (result !== undefined) {
            this.currentContacts = result.value;

            //this.orderContacts.clear();
            for (let i = this.pidecQCContacts.length - 1; i >= 0; i--) {
              const group = this.pidecQCContacts.at(i) as FormGroup; // Access the FormGroup
              if ( group.get('contactType')?.value == ContactTypes.QC_Lead  || group.get('contactType')?.value == ContactTypes.QC_Manager) {
                this.pidecQCContacts.removeAt(i); // Remove the FormGroup if the condition is true
              }
            }

            this.currentContacts.forEach(contact => {

              let fg= this.generateContactFromContactValue(contact);
              this.pidecQCContacts.push(fg);
            });

          }
        });
      }
    openPidecInspectorContactSelector():void{

        let currentAddedContacts = this.pidecInspectorContacts?.getRawValue().filter((contact)=> contact.contactType==ContactTypes.PIDEC_Inspector && (contact.email !='' || [] )) as Contact[];
         let exceptionContacts : Contact[];
        const dialogRef = this.dialog.open(ContactpersonSelectorComponent, {
          data: {ToContacts: {currentAddedContacts,exceptionContacts} ,ContactType:ContactTypes.PIDEC_Inspector,timeStamp: new Date().getTime()},panelClass: 'messageDialog', disableClose: true, autoFocus: false
          ,width:'700px', maxWidth:'140vw',height:'60vh',minHeight:'300px'
        });
       // dialogRef.afterOpened().subscribe(()=>dialogRef.componentInstance.initializeForm());

        dialogRef.afterClosed().subscribe(result => {

          if (result !== undefined) {
            this.currentContacts = result.value;

            //this.orderContacts.clear();
            for (let i = this.pidecInspectorContacts.length - 1; i >= 0; i--) {
              const group = this.pidecInspectorContacts.at(i) as FormGroup; // Access the FormGroup
              if ( group.get('contactType')?.value == ContactTypes.PIDEC_Inspector) {
                this.pidecInspectorContacts.removeAt(i); // Remove the FormGroup if the condition is true
              }
            }

            this.currentContacts.forEach(contact => {

              let fg= this.generateContactFromContactValue(contact);
              this.pidecInspectorContacts.push(fg);
            });

          }
        });
      }

       openTPInspectorContactSelector():void{

        let currentAddedContacts = this.tPInspectorContacts?.getRawValue().filter((contact)=> contact.contactType==ContactTypes.ThirdPart_Inspector && (contact.email !='' || [] )) as Contact[];
         let exceptionContacts : Contact[];
        const dialogRef = this.dialog.open(ContactpersonSelectorComponent, {
          data: {ToContacts:{ currentAddedContacts ,exceptionContacts},ContactType:ContactTypes.ThirdPart_Inspector,timeStamp: new Date().getTime()},panelClass: 'messageDialog', disableClose: true, autoFocus: false
          ,width:'700px', maxWidth:'140vw',height:'60vh',minHeight:'300px'
        });

        dialogRef.afterClosed().subscribe(result => {

          if (result !== undefined) {
            this.currentContacts = result.value;

            //this.orderContacts.clear();
            for (let i = this.tPInspectorContacts.length - 1; i >= 0; i--) {
              const group = this.tPInspectorContacts.at(i) as FormGroup; // Access the FormGroup
              if ( group.get('contactType')?.value == ContactTypes.ThirdPart_Inspector) {
                this.tPInspectorContacts.removeAt(i); // Remove the FormGroup if the condition is true
              }
            }

            this.currentContacts.forEach(contact => {

              let fg= this.generateContactFromContactValue(contact);
              this.tPInspectorContacts.push(fg);
            });

          }
        });
      }


      openOwnerContactSelector():void{

        let currentAddedContacts = this.ownerContacts?.getRawValue().filter((contact)=> contact.contactType==ContactTypes.Owner && (contact.email !='' || [])) as Contact[];
        let exceptionContacts : Contact[];
        const dialogRef = this.dialog.open(ContactpersonSelectorComponent, {
          data: {ToContacts: {currentAddedContacts,exceptionContacts},ContactType:ContactTypes.Owner ,timeStamp: new Date().getTime()},panelClass: 'messageDialog', disableClose: true, autoFocus: false
          ,width:'700px', maxWidth:'140vw',height:'60vh',minHeight:'300px'
        });

        dialogRef.afterClosed().subscribe(result => {

          if (result !== undefined) {
            this.currentContacts = result.value;

            for (let i = this.ownerContacts.length - 1; i >= 0; i--) {
              const group = this.ownerContacts.at(i) as FormGroup; // Access the FormGroup
              if ( group.get('contactType')?.value == ContactTypes.Owner) {
                this.ownerContacts.removeAt(i); // Remove the FormGroup if the condition is true
              }
            }

            this.currentContacts.forEach(contact => {

              let fg= this.generateContactFromContactValue(contact);
              this.ownerContacts.push(fg);
            });

          }
        });
      }

      openMCContactSelector():void{

        let currentAddedContacts = this.mCContacts?.getRawValue().filter((contact)=> contact.contactType==ContactTypes.MC &&(contact.email !='' )) || [] as Contact[];
        let exceptionContacts : Contact[];
        const dialogRef = this.dialog.open(ContactpersonSelectorComponent, {
          data: {ToContacts: {currentAddedContacts,exceptionContacts},ContactType:ContactTypes.MC ,timeStamp: new Date().getTime()},panelClass: 'messageDialog', disableClose: true, autoFocus: false
          ,width:'700px', maxWidth:'140vw',height:'60vh',minHeight:'300px'
        });

        dialogRef.afterClosed().subscribe(result => {

          if (result !== undefined) {
            this.currentContacts = result.value;

            for (let i = this.mCContacts.length - 1; i >= 0; i--) {
              const group = this.mCContacts.at(i) as FormGroup; // Access the FormGroup
              if ( group.get('contactType')?.value == ContactTypes.MC) {
                this.mCContacts.removeAt(i); // Remove the FormGroup if the condition is true
              }
            }

            this.currentContacts.forEach(contact => {
              let fg= this.generateContactFromContactValue(contact);
              this.mCContacts.push(fg);
            });

          }
        });
      }

  get pidecQCContacts() :FormArray<FormTypes.FormContact> | undefined{
       return this.projectForm?.controls.pidecQCContacts as FormArray<FormTypes.FormContact>;

   }

  get mCContacts() :FormArray<FormTypes.FormContact> | undefined{
       return this.projectForm?.controls.mcContacts as FormArray<FormTypes.FormContact>;

   }

   get tPInspectorContacts() :FormArray<FormTypes.FormContact> | undefined{
       return this.projectForm?.controls.tpInspectors as FormArray<FormTypes.FormContact>;

   }

  get pidecInspectorContacts() :FormArray<FormTypes.FormContact> | undefined{
       return this.projectForm?.controls.pidecInspectors as FormArray<FormTypes.FormContact>;

   }

   get ownerContacts() :FormArray<FormTypes.FormContact> | undefined{
       return this.projectForm?.controls.ownerContacts as FormArray<FormTypes.FormContact>;

   }



  get filteredPidecQCContacts(): FormGroup[] | undefined {

      return (this.pidecQCContacts.controls.filter(ctrl => ctrl.get('email')?.value?.toString().trim() !== '' || ctrl.get('id')?.value?.toString().trim() !== '00000000-0000-0000-0000-000000000000') || []) as FormGroup[];
    return ((this.projectForm?.get('pidecQCContacts') as FormArray)?.controls
    ?.filter(ctrl => ctrl.get('email')?.value?.toString().trim() !== '' || ctrl.get('id')?.value?.toString().trim() !== '00000000-0000-0000-0000-000000000000') || []) as FormGroup[];

    }

     get filteredOwnerContacts(): FormGroup[] | undefined {

      return (this.ownerContacts.controls.filter(ctrl => ctrl.get('email')?.value?.toString().trim() !== '' || ctrl.get('id')?.value?.toString().trim() !== '00000000-0000-0000-0000-000000000000') || []) as FormGroup[];
    return ((this.projectForm?.get('ownerContacts') as FormArray)?.controls
    ?.filter(ctrl => ctrl.get('email')?.value?.toString().trim() !== '' || ctrl.get('id')?.value?.toString().trim() !== '00000000-0000-0000-0000-000000000000') || []) as FormGroup[];

    }

    get filteredMCContacts(): FormGroup[] | undefined {
       return (this.mCContacts.controls.filter(ctrl => ctrl.get('email')?.value?.toString().trim() !== '' || ctrl.get('id')?.value?.toString().trim() !== '00000000-0000-0000-0000-000000000000') || []) as FormGroup[];
      return ((this.projectForm?.get('mcContacts') as FormArray)?.controls
      ?.filter(ctrl => ctrl.get('email')?.value?.toString().trim() !== '' || ctrl.get('id')?.value?.toString().trim() !== '00000000-0000-0000-0000-000000000000') || []) as FormGroup[];

    }

    get filteredPidecInspectorContacts(): FormGroup[] | undefined {

       return (this.pidecInspectorContacts.controls.filter(ctrl => ctrl.get('email')?.value?.toString().trim() !== '' || ctrl.get('id')?.value?.toString().trim() !== '00000000-0000-0000-0000-000000000000') || []) as FormGroup[];

      return ((this.projectForm?.get('pidecInspectors') as FormArray)?.controls
      ?.filter(ctrl => ctrl.get('email')?.value?.toString().trim() !== '' || ctrl.get('id')?.value?.toString().trim() !== '00000000-0000-0000-0000-000000000000') || []) as FormGroup[];

    }

    get filteredThirdPartContacts(): FormGroup[] | undefined {
       return (this.tPInspectorContacts.controls.filter(ctrl => ctrl.get('email')?.value?.toString().trim() !== '' || ctrl.get('id')?.value?.toString().trim() !== '00000000-0000-0000-0000-000000000000') || []) as FormGroup[];
      return ((this.projectForm?.get('tpInspectors') as FormArray)?.controls
      ?.filter(ctrl => ctrl.get('email')?.value?.toString().trim() !== '' || ctrl.get('id')?.value?.toString().trim() !== '00000000-0000-0000-0000-000000000000') || []) as FormGroup[];

    }

SaveProjectSettings():void
{
    let projectSettings:Project;
    projectSettings = this.projectForm.getRawValue() as Project;


     let currentpidecQCContact;
    for(let i=projectSettings.pidecQCContacts.length-1;i>=0;i--){
       currentpidecQCContact = projectSettings.pidecQCContacts[i];
      if(currentpidecQCContact.email==undefined || currentpidecQCContact.email.trim()=='' )
        {
          projectSettings.pidecQCContacts.splice(i,1);
        }
    }

       let currentMCContact;
    for(let i=projectSettings.mcContacts.length-1;i>=0;i--){
       currentMCContact = projectSettings.mcContacts[i];
      if(currentMCContact.email==undefined || currentMCContact.email.trim()=='' )
        {
          projectSettings.mcContacts.splice(i,1);
        }
    }

          let currentownerContact;
    for(let i=projectSettings.ownerContacts.length-1;i>=0;i--){
       currentownerContact = projectSettings.ownerContacts[i];
      if(currentownerContact.email==undefined || currentownerContact.email.trim()=='' )
        {
          projectSettings.ownerContacts.splice(i,1);
        }
    }

          let currentTPInspector;
    for(let i=projectSettings.tpInspectors.length-1;i>=0;i--){
       currentTPInspector = projectSettings.tpInspectors[i];
      if(currentTPInspector.email==undefined || currentTPInspector.email.trim()=='' )
        {
          projectSettings.tpInspectors.splice(i,1);
        }
    }

          let currentPidecInspector;
    for(let i=projectSettings.pidecInspectors.length-1;i>=0;i--){
       currentPidecInspector = projectSettings.pidecInspectors[i];
      if(currentPidecInspector.email==undefined || currentPidecInspector.email.trim()=='' )
        {
          projectSettings.pidecInspectors.splice(i,1);
        }
    }

    this.basicService.SaveProjectSettings(projectSettings).subscribe((result)=>{});
}

}
