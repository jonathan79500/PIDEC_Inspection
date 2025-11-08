import { Vendor } from './../../../Core/Models/Vendor.model';
import { Order } from './../../../Core/Models/Order.model';
import { currentstateStore } from './../../../Stores/currentstate.store';
import { CommonModule } from '@angular/common';
import { Component, effect, inject, Injector, OnInit, runInInjectionContext } from '@angular/core';
import { FormArray,  FormBuilder,  FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { AgGridModule } from 'ag-grid-angular';
import { ColDef } from 'ag-grid-community';
import * as FormTypes from '../../../Core/FormTypes/InspectionFormTypes';
import { Contact } from '../../../Core/Models/Contact.model';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { ContactpersonSelectorComponent } from '../../contactperson-selector/contactperson-selector.component';
import { InspectionSubject } from '../../../Core/Models/InspectionSubject.model';
import { QCInfoStore } from '../../../Stores/QCInfo.store';
import { BasicQCMemberInfo } from '../../../Core/Models/BasicQCMemberInfo.model';

import { BasicInfoService } from '../../../Core/Services/basic-info.service';
import { ContactTypes } from '../../../Core/Models/ContactTypes.enum';
import { Address } from '../../../Core/Models/Address.model';
import { ObjectTypes } from '../../../Core/Models/ObjectTypes.enum';
import { take } from 'rxjs';
import { ProjectContact } from '../../../Core/Models/ProjectContact.model';



@Component({
    selector: 'app-order-management',
    imports: [CommonModule,ReactiveFormsModule, FormsModule, AgGridModule,MatDialogModule],
    templateUrl: './order-management.component.html',
    styleUrl: './order-management.component.css'
})
export class OrderManagementComponent implements OnInit{

  orderMngForm:FormTypes.FormOrderManagement;
  inspectionSubjects:FormArray<FormTypes.FormInspectionSubject>;
  selectedOrder: any;
  selectedPidecContactPerson:Contact;
  currentContacts :FormArray<FormTypes.FormContact>;// Contact[];
   injector = inject(Injector);
  readonly dialog = inject(MatDialog);
  currentQCStore = inject(QCInfoStore);
  currentAppState = inject(currentstateStore);
  currentQCData :BasicQCMemberInfo;
  currentOrderForm: FormTypes.FormOrder;
  exceptedProjectContacts: Contact[] = [];
  EdmsOrders: Order[]=[];
   rowData =
   [
     { make: 'Tesla', model: 'Model Y', price: 64950 },
     { make: 'Ford', model: 'F-Series', price: 33850 },
     { make: 'Toyota', model: 'Corolla', price: 29600 }
   ];
   columnDefs:ColDef[] = [ { field: 'make' }, { field: 'model' }, { field: 'price' }];

   /**
    *
    */
   constructor(private formBuilder:FormBuilder,private basicService:BasicInfoService) {
   // console.log("EDMS Orders :",this.currentQCStore.orders());

   console.log('this.currentQCData.inspectionSubjects: ', this.currentQCStore.inspectionSubjects());

    //this.currentQCData = this.currentQCStore.SetBasicQCInfo()
    this.inspectionSubjects = this.formBuilder.array<FormTypes.FormInspectionSubject>([]);

    const existingNames = new Set<string>();


    this.currentQCStore.orderInspectionSubjects().forEach((inspectionSubject)=>{
      const subjectName = inspectionSubject.inspectionSubject.name.trim().toLowerCase();

      // Check if the name is already in the Set
      if (!existingNames.has(subjectName)) {
        // Add name to the Set (to track it as added)
        existingNames.add(subjectName);

      this.inspectionSubjects.push(this.formBuilder.group({
        id: new FormControl<string>(inspectionSubject.inspectionSubject.id),
        name :new FormControl<string>(inspectionSubject.inspectionSubject.name),
        selected: new FormControl<boolean>(false)

      }) as FormTypes.FormInspectionSubject);
    }
    })


   }


     generatAddress():FormTypes.FormAddress{
       return this.formBuilder.group({
        id: '00000000-0000-0000-0000-000000000000',
         city:'',
         country:'',
         state:'',
         streetLine1:'',
         streetLine2:'',
         zipCode:''
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
      address:this.generatAddress(),
      contactType:0,
      homePage:'',
      isVisible:[true],
      userId:'',
      password:'',
      belongedTo: [ObjectTypes.Undefined]
    })
  }

 generateInspectionSubject():FormTypes.FormInspectionSubject{
    return this.formBuilder.group({
      id:'' ,
      name:'',
      selected : new FormControl<boolean>(false)
    });
  }
   generateOrderForm():FormTypes.FormOrder{
     let currentOrderNo:string = '';
        let currentOrderName:string = '';
        let currentOrderProjectId:string = '';
        let currentOrder:Order ;
        if(this.selectedOrder != undefined)
          currentOrder = this.selectedOrder;

        return this.formBuilder.group({
          createdBy:'',
          createdDate: new Date(Date.now()),
          id:'',
          modifiedBy:'',
          modifiedDate:new Date(Date.now()),
          edmsId:currentOrder?.edmsId,
          name:currentOrderName,
          no:currentOrderNo,
          project:this.currentAppState.currentproject(),
          version:'0',
          title:currentOrderName,
          equipmentTagNo:'',
          itP_NO:currentOrder?.itP_NO,
          exceptionContacts: this.formBuilder.array<FormTypes.FormContact>([this.generateContact()]),
          contacts:this.formBuilder.array<FormTypes.FormContact>([this.generateContact()]),
          inspectionSubjects:this.formBuilder.array<FormTypes.FormInspectionSubject>([this.generateInspectionSubject()]),
        })
   }

   ngOnInit(): void {
    // this.currentAppState.GetEdmsOrders_ProId(this.basicService).subscribe((edmsOrders)=>{
    //   this.EdmsOrders = edmsOrders;//this.currentAppState.qCMemberBasicInfo().eDMSOrders;
    // });
    runInInjectionContext(this.injector,()=>{
    effect(() => {
      this.EdmsOrders = this.currentAppState.qCMemberBasicInfo().eDMSOrders;
    });
  })
    this.orderMngForm = this.formBuilder.group({

      order: this.generateOrderForm(),

      newInspectionSubject : this.formBuilder.group(
        {
          name:'',
          selected:new FormControl<boolean>(false),
          id:'0'
        }
      )
    });


    console.log('Oredre Mng form initialized:', this.orderMngForm);

   }
   get filteredOwnerContacts(): FormGroup[] | undefined {
    // Access the FormArray and filter the controls dynamically
    return this.orderContacts?.controls.filter((control: FormGroup) => {
      const contactType = control.get('contactType')?.value;
      console.log('contactType in filteredOwnerContacts',contactType);
      return contactType && contactType==ContactTypes.Owner // Example filter condition
    }) as FormGroup[]; // Cast to FormGroup array
  }

  get filteredMCContacts(): FormGroup[] | undefined {
    // Access the FormArray and filter the controls dynamically
    return this.orderContacts?.controls.filter((control: FormGroup) => {
      const contactType = control.get('contactType')?.value;
      console.log('contactType in filteredMCContacts',contactType);
      return contactType && contactType==ContactTypes.MC // Example filter condition
    }) as FormGroup[]; // Cast to FormGroup array
  }


  get filteredQCContacts(): FormGroup[] | undefined {
    // Access the FormArray and filter the controls dynamically
    return this.orderContacts?.controls.filter((control: FormGroup) => {
      const contactType = control.get('contactType')?.value;
      console.log('contactType in filteredQCContacts',contactType);
      return contactType && contactType==ContactTypes.PIDEC_TO // Example filter condition
    }) as FormGroup[]; // Cast to FormGroup array
  }



   get currentOrder():FormTypes.FormOrder | undefined{
    return this.orderMngForm?.controls.order as FormTypes.FormOrder;
   }
get newInspectionSubject(){
  return this.orderMngForm?.controls?.newInspectionSubject
}
  get orderContacts() :FormArray<FormTypes.FormContact> | undefined{
    return this.currentOrder?.controls?.contacts as FormArray<FormTypes.FormContact>;
  }

   get exceptionOrderContacts() :FormArray<FormTypes.FormContact> | undefined{
    return this.currentOrder?.controls?.exceptionContacts as FormArray<FormTypes.FormContact>;
  }


  get orderInspectionSubjects() :FormArray<FormTypes.FormInspectionSubject> | undefined{
    return this.currentOrder?.controls?.inspectionSubjects as FormArray<FormTypes.FormInspectionSubject>; //this.orderMngForm?.controls.subjects as FormArray
  }

  ActivateOrderInInspectionSystem(edmsOrder:Order):void
  {
    this.orderMngForm.patchValue({
      order:{
        edmsId:edmsOrder.edmsId,
        name:edmsOrder.name,
        title:edmsOrder.title,
        equipmentTagNo:edmsOrder.equipmentTagNo,
        no:edmsOrder.no,
        version:'0',
        project:this.currentAppState.currentproject()

      }
    })

    let orderProp = this.orderMngForm.getRawValue();

    let currentContactPerson;
    for(let i=orderProp.order.contacts.length-1;i>=0;i--){
       currentContactPerson = orderProp.order.contacts[i];
      if(currentContactPerson.email==undefined || currentContactPerson.email.trim()=='' || currentContactPerson.belongedTo == ObjectTypes.Project)
        {
          orderProp.order.contacts.splice(i,1);
        }
    }



    let currentInspectionSubject;
    for(let i= orderProp.order.inspectionSubjects.length-1;i>=0;i--)
    {
      currentInspectionSubject=orderProp.order.inspectionSubjects[i];
      if(currentInspectionSubject.name==undefined || currentInspectionSubject.name.trim()=='' )
        {
          orderProp.order.inspectionSubjects.splice(i,1);
        }
    }
    // if(orderProp.order.vendorContactPerson.email=='' || orderProp.order.vendorContactPerson.email==undefined)
    //   orderProp.order.vendorContactPerson=null;
    console.log("Selected Order Prop: ", orderProp.order);
    let resultOrder = orderProp.order as Order;


    resultOrder.exceptionContacts = this.exceptedProjectContacts;


    this.basicService.ActivateOrderInInspectionSystem(resultOrder).subscribe((result)=>{});
  }

getSubjectFormGroup(subject)
{
  return subject as FormGroup
}

addNewSubject(): void {
  const newSubjectName = this.orderMngForm.controls.newInspectionSubject.get('name')?.value;
  if (newSubjectName) {
    let newSubject : FormTypes.FormInspectionSubject;
    newSubject = this.formBuilder.group({
      id:new FormControl('00000000-0000-0000-0000-000000000000'),
      name: new FormControl(newSubjectName),
      selected: new FormControl(true),
    });

    this.orderInspectionSubjects.push( newSubject);
    this.newInspectionSubject.get('name')?.reset(); // Clear the input field
  }
}
toggleSelection(subject: FormTypes.FormInspectionSubject): void {
  const subjectGroup = this.orderInspectionSubjects.controls.filter((ois)=>ois.controls.id.value == subject.get("id").value).at(0);//.at(index) as FormGroup;
  const currentSelectedValue = subjectGroup.get('selected')?.value;
  subjectGroup.get('selected')?.setValue(!currentSelectedValue);
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

  generateContactFromContactValue(contact:Contact):FormTypes.FormContact{
    return this.formBuilder.group({
      id:contact.id!=''? contact.id :'00000000-0000-0000-0000-000000000000',
      attention:contact.attention,
      cc:contact.cc,
      email:contact.email,
      fax:contact.fax,
      name:contact.name,
      phone:contact.phone,
      contactType:contact.contactType,
      mobile:contact.mobile,
      homePage:contact.homePage,
      isVisible:[true],
      address:contact!.address != null ? this.generateAddressFromAddressValue(contact.address):this.generatAddress(),
      userId:[contact.userId],
      password:[contact.password],
      belongedTo:[contact.belongedTo]
    })
  }


  openPidecContactSelector():void{

      let currentAddedContacts = this.orderContacts?.getRawValue().filter((contact)=> contact.contactType==ContactTypes.PIDEC_TO && (contact.email !='' || [] )) as Contact[];
      let exceptionContacts = this.currentOrder.controls.exceptionContacts.value.filter((contact)=> contact.contactType==ContactTypes.PIDEC_TO && (contact.email !='' || [] )) as Contact[];
      const dialogRef = this.dialog.open(ContactpersonSelectorComponent, {
        data: {ToContacts: {currentAddedContacts,exceptionContacts} ,ContactType:ContactTypes.PIDEC_TO,timeStamp: new Date().getTime()},panelClass: 'messageDialog', disableClose: true, autoFocus: false
        ,width:'700px', maxWidth:'140vw',height:'56.5vh',minHeight:'300px'
      });

      dialogRef.afterClosed().subscribe(result => {

        if (result !== undefined) {
          this.currentContacts = result;

          //this.orderContacts.clear();
          for (let i = this.orderContacts.length - 1; i >= 0; i--) {
            const group = this.orderContacts.at(i) as FormGroup; // Access the FormGroup
            if ( group.get('contactType')?.value == ContactTypes.PIDEC_TO) {
              this.orderContacts.removeAt(i); // Remove the FormGroup if the condition is true
            }
          }

          this.currentContacts.controls.forEach(contact => {

            let fg= this.generateContactFromContactValue(contact.value as Contact);
            this.orderContacts.push(fg);
          });

        }
      });
    }

    openOwnerContactSelector():void{

      let currentAddedContacts = this.orderContacts?.getRawValue().filter((contact)=> contact.contactType==ContactTypes.Owner && (contact.email !='' || [])) as Contact[];
        let exceptionContacts = this.currentOrder.controls.exceptionContacts.value.filter((contact)=> contact.contactType==ContactTypes.Owner && (contact.email !='' || [])) as Contact[];
        let filteredExceptionProjectContacts = this.exceptedProjectContacts.filter((contact)=> contact.contactType==ContactTypes.Owner && (contact.email !='' || [])) as Contact[];
        if(exceptionContacts != undefined && exceptionContacts != null)
          exceptionContacts=  exceptionContacts.concat(...filteredExceptionProjectContacts);
      const dialogRef = this.dialog.open(ContactpersonSelectorComponent, {
        data: {ToContacts: {currentAddedContacts,exceptionContacts},ContactType:ContactTypes.Owner ,timeStamp: new Date().getTime()},panelClass: 'messageDialog', disableClose: true, autoFocus: false
      });

      let existedContact:Contact = null;

      dialogRef.afterClosed().subscribe(result => {

        if (result !== undefined) {
          this.currentContacts = result;

          for (let i = this.orderContacts.length - 1; i >= 0; i--) {
            const group = this.orderContacts.at(i) as FormGroup; // Access the FormGroup
            if ( group.get('contactType')?.value == ContactTypes.Owner) {

              existedContact = this.currentContacts.controls.find((contact)=> contact.controls.id.value == group.get('id')?.value )?.value as Contact;

              if(existedContact == null)
                this.CheckExceptionContacts(group.value as Contact);

              this.orderContacts.removeAt(i); // Remove the FormGroup if the condition is true
            }
          }

          this.currentContacts.controls.forEach(contact => {

            let fg= this.generateContactFromContactValue(contact.value as Contact);
            this.orderContacts.push(fg);
          });

        }
      });
    }

    openMCContactSelector():void{

      let currentAddedContacts = this.orderContacts?.getRawValue().filter((contact)=> contact.contactType==ContactTypes.MC &&(contact.email !='' )) || [] as Contact[];
      let exceptionContacts = this.currentOrder.controls.exceptionContacts.value.filter((contact)=> contact.contactType==ContactTypes.MC &&(contact.email !='' )) || [] as Contact[];

       let filteredExceptionProjectContacts =  this.exceptedProjectContacts.filter((contact)=> contact.contactType==ContactTypes.MC &&(contact.email !='' )) || [] as Contact[];
      if(exceptionContacts != undefined && exceptionContacts != null)
         exceptionContacts = exceptionContacts.concat(filteredExceptionProjectContacts);
      const dialogRef = this.dialog.open(ContactpersonSelectorComponent, {
        data: {ToContacts: {currentAddedContacts,exceptionContacts},ContactType:ContactTypes.MC ,timeStamp: new Date().getTime()},panelClass: 'messageDialog', disableClose: true, autoFocus: false
      });


      let existedContact:Contact = null;
      dialogRef.afterClosed().subscribe(result => {

        if (result !== undefined) {
          this.currentContacts = result;

          for (let i = this.orderContacts.length - 1; i >= 0; i--) {
            const group = this.orderContacts.at(i) as FormGroup; // Access the FormGroup


            if ( group.get('contactType')?.value == ContactTypes.MC) {

              existedContact = this.currentContacts.controls.find((contact)=> contact.controls.id.value == group.get('id')?.value )?.value as Contact;

              if(existedContact == null)
                this.CheckExceptionContacts(group.value as Contact);

              this.orderContacts.removeAt(i); // Remove the FormGroup if the condition is true
            }

          }


          this.currentContacts.controls.forEach(contact => {
            let fg= this.generateContactFromContactValue(contact.value as Contact);
            this.orderContacts.push(fg);
          });

        }
      });
    }

    CheckExceptionContacts(contact: Contact) : boolean {
      if(contact.belongedTo == ObjectTypes.Project)
      {
        this.exceptedProjectContacts.push(contact);
        return true;
      }
      return false;
    }
  // Remove a contact from FormArray
  removePidecContactPerson(index: number) {
    this.orderContacts.removeAt(index);
  }
// Remove a contact from FormArray
removeMCContactPerson(index: number) {
  this.orderContacts.removeAt(index);
}
// Remove a contact from FormArray
removeOwnerContactPerson(index: number) {
  this.orderContacts.removeAt(index);
}


  generateInspectionSubjectFromValue(inspectionSubjectId:string):FormTypes.FormInspectionSubject{


    return this.formBuilder.group({
      id:inspectionSubjectId,
      name:'inspectionSubject.name',
      selected: new FormControl<boolean>(true)
    });
  }

  ClearOrderForm():void
  {
    this.orderContacts.clear();
    this.orderInspectionSubjects.clear();
    this.currentOrder.patchValue({});
  }

  ChangeEdmsOrder():void{

    this.ClearOrderForm();

  //this.currentAppState.ChangeEdmsOrder(this.basicService,this.selectedOrder.edmsId);//.subscribe((result)=>
  //{
//this.currentAppState.SetCurrentOrder(result);
this.currentAppState.ChangeEdmsOrder(this.basicService, this.selectedOrder.edmsId)
  .subscribe(currentSelectedOrder => {
    // result from HTTP (shared if multiple callers)

if(currentSelectedOrder != null )
{
  if(currentSelectedOrder.contacts != null)
  {
       currentSelectedOrder.contacts.forEach(contact => {
  this.orderContacts.push(
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
      isVisible:[true],
      address:contact!.address != null ? this.generateAddressFromAddressValue(contact.address)  : this.generatAddress(),// Ensure this is not null or adjust validators accordingly
      password:[contact.password],
      belongedTo:[ObjectTypes.Order]
    })
  );
});
  }

 if(currentSelectedOrder.project.contacts != null)
  {
       currentSelectedOrder.project.contacts.forEach(contact => {
  this.orderContacts.push(
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
      isVisible:[true],
      address:contact!.address != null ? this.generateAddressFromAddressValue(contact.address)  : this.generatAddress(),// Ensure this is not null or adjust validators accordingly
      password:[contact.password],
      belongedTo:[ObjectTypes.Project]
    })
  );
});
}


if(currentSelectedOrder.exceptionContacts != null){

  currentSelectedOrder.exceptionContacts.forEach(contact=>{
    this.exceptionOrderContacts.push(
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
      isVisible:[true],
      address:contact!.address != null ? this.generateAddressFromAddressValue(contact.address)  : this.generatAddress(),// Ensure this is not null or adjust validators accordingly
      password:[contact.password],
      belongedTo:[ObjectTypes.Project]
    })
    )
  })
   this.currentOrder.patchValue({
    exceptionContacts: [...this.exceptionOrderContacts.value]
   });
}

  if(currentSelectedOrder.inspectionSubjects != null)
    {
         currentSelectedOrder.inspectionSubjects.forEach(inspectionSubject => {
    this.orderInspectionSubjects.push(
      this.formBuilder.group({
        id: [inspectionSubject.id],
        name: [inspectionSubject.name],
        selected:[true]
      })
    );
  });
    }

     console.log('order Contacts :',this.orderContacts);
        this.currentOrder.patchValue({
          edmsId : currentSelectedOrder.edmsId != null ? currentSelectedOrder.edmsId:'',
          itP_NO:currentSelectedOrder.itP_NO !='' ? currentSelectedOrder.itP_NO : '',
          id: currentSelectedOrder.id !='' ? currentSelectedOrder.id:''
          // inspectionSubjects:result.inspectionSubjects
        });
      }
      else
      {
        this.currentOrder.patchValue({
          edmsId : '',
          itP_NO:'',

          // inspectionSubjects:result.inspectionSubjects
        });

      }

       var vendorcontacts = this.selectedOrder.contacts.filter(cnt=>cnt.contactType==ContactTypes.Vendor) ;
       let vendorcontact = undefined;
       let findedVendorContact = this.orderContacts.value.filter(cnt=>cnt.contactType ==ContactTypes.Vendor) ;
       if(vendorcontacts.length >0 && findedVendorContact.length==0)
       {

        vendorcontact = vendorcontacts[0];
        this.orderContacts.push(this.formBuilder.group({
          id: [vendorcontact.id],
      phone: [vendorcontact.phone],
      fax: [vendorcontact.fax],
      name: [vendorcontact.name],
      email: [vendorcontact.email],
      homePage: [vendorcontact.homePage],
      attention: [vendorcontact.attention],
      contactType: [vendorcontact.contactType],
      mobile: [vendorcontact.mobile],
      cc: [vendorcontact.cc],
      userId:[vendorcontact.userId],
      isVisible:[true],
      address:vendorcontact!.address != null ? this.generateAddressFromAddressValue(vendorcontact.address)  : this.generatAddress(),
      password:[vendorcontact.password],
      belongedTo:[ObjectTypes.Vendor]
        }))
      }
  });
   // this.currentOrder.patchValue({edmsId:this.selectedOrder.edmsId,name:this.selectedOrder.name});
  }
SaveOrderSettings():void
{
  const selectedSubjects = this.orderInspectionSubjects.controls
  .filter(subject => subject.get('selected')?.value) // Filter selected subjects
  .map(subject => subject.get('id')?.value); // Extract their IDs

  selectedSubjects.forEach(id => this.orderInspectionSubjects.push(this.generateInspectionSubjectFromValue(id)));
}
}
