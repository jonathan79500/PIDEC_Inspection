import { IDatepickerTheme } from 'ng-persian-datepicker';

import { map } from 'rxjs';
import { qCMembersbasicinfoResolver } from './../../Core/Resolvers/qcmembersbasicinfo.resolver';
import { CommonModule } from '@angular/common';
import { Component,NgZone,Inject, OnInit, inject } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatIcon } from '@angular/material/icon';
import { Contact } from '../../Core/Models/Contact.model';
import * as FormTypes from '../../Core/FormTypes/InspectionFormTypes'
import {Address} from '../../Core/Models/Address.model'
import { ObjectTypes } from '../../Core/Models/ObjectTypes.enum';
import { currentstateStore } from '../../Stores/currentstate.store';



@Component({
    selector: 'app-contactperson-selector',
    imports: [ReactiveFormsModule, FormsModule, CommonModule],
    templateUrl: './contactperson-selector.component.html',
    styleUrl: './contactperson-selector.component.css'
})
export class ContactpersonSelectorComponent implements OnInit{

  //public contactForm:FormTypes.FormContact;
  public newContactForm:FormTypes.FormContact;
  public findedContacts :FormArray<FormTypes.FormContact>;
  public selectedContacts:FormArray<FormTypes.FormContact>;
  public contactType :number = -1;
   currentState = inject(currentstateStore);
   public existingIds : Set<string>;

  constructor(public dialogRef: MatDialogRef<ContactpersonSelectorComponent>, private ngZone: NgZone,
    @Inject(MAT_DIALOG_DATA) public beforeSelectedContacts: any,private fBuilder: FormBuilder) {
      console.log('MAT_DIALOG_DATA:', beforeSelectedContacts);

 this.findedContacts = this.fBuilder.array<FormTypes.FormContact>([]);
     this.selectedContacts = this.fBuilder.array<FormTypes.FormContact>([]);


        this.contactType =  beforeSelectedContacts.ContactType;



   this.newContactForm = this.generateContactForm();
  }

  ngOnInit(): void {
     this.initializeForm();
  }

   initializeForm():void{
     console.log('MAT_DIALOG_DATA in onInit:', this.beforeSelectedContacts);

     let conformContactsBasedOnCorrentContactType : Contact[];
     let qCMembersbasicInfo = this.currentState.qCMemberBasicInfo();
     if(qCMembersbasicInfo != null && qCMembersbasicInfo != undefined)
         if( qCMembersbasicInfo.usefulContacts != null && qCMembersbasicInfo.usefulContacts != undefined)
         {
            conformContactsBasedOnCorrentContactType = qCMembersbasicInfo.usefulContacts.filter((contact)=>
              contact.contactType == this.contactType) as Contact[];
            if(conformContactsBasedOnCorrentContactType.length>0)
            {
              conformContactsBasedOnCorrentContactType.forEach(element => {
              this.findedContacts.push(this.generateContactFromContactValue(element));
              });
            }
         }
    this.newContactForm = this.generateContactForm();
    //console.log('in contact init',this.beforeSelectedContacts.ToContacts);

    this.existingIds = new Set(this.findedContacts.value.map(c => c.id));

     if(this.beforeSelectedContacts?.ToContacts["exceptionContacts"]?.length>0)
    {
      this.beforeSelectedContacts.ToContacts["exceptionContacts"].forEach(element => {
        if (!this.existingIds.has(element.value.id))
        {
          this.existingIds.add(element.value.id);
          this.findedContacts.push(this.generateContactFromContactValue(element));
        }
      });
    }


    if(this.beforeSelectedContacts?.ToContacts["currentAddedContacts"]?.length>0)
    {
      this.beforeSelectedContacts.ToContacts["currentAddedContacts"].forEach(element => {
        //this.selectedContacts.push(this.generateContactFromContactValue(element));
        if(this.existingIds.has(element.id))
        {
          this.addToSelectedContact(element);
        }
      });
    }
      //this.selectedContacts.push(...this.beforeSelectedContacts.ToContacts);
   }

get fcontacts(): FormGroup[] {
  return this.findedContacts.controls as FormGroup[];
}

get scontacts(): FormGroup[] {
  return this.selectedContacts.controls as FormGroup[];
}
  generateContactForm():FormTypes.FormContact{
    return this.fBuilder.group({
      id:'',
      note:'',
      cc:'',
      email:'',
      fax:'',
      name:'',
      phone:'',
      mobile:'',
      address:this.generatAddress(),
      contactType:this.contactType,
      homePage:'',
      isVisible:[true],
      userId:'',
      password:'',
      belongedTo:ObjectTypes.Undefined
    })

  }

  generatAddress():FormTypes.FormAddress{
    return this.fBuilder.group({
      id:'00000000-0000-0000-0000-000000000000',
      city:'',
      country:'',
      state:'',
      streetLine1:'',
      streetLine2:'',
      zipCode:''
    })
  }

  generatAddressFromAddressValue(address:Address):FormTypes.FormAddress{
    return this.fBuilder.group({
      id: address.id!=''? address.id :'00000000-0000-0000-0000-000000000000',
      city:address.city,
      country:address.country,
      state:address.state,
      streetLine1:address.streetLine1,
      streetLine2:address.streetLine2,
      zipCode:address.zipCode
    })
  }
 generateContactFromContactValue(contact:Contact):FormTypes.FormContact{
    return this.fBuilder.group({
      id:contact.id!=''? contact.id :'00000000-0000-0000-0000-000000000000',
      note:contact.note,
      cc:contact.cc,
      email:contact.email,
      fax:contact.fax,
      name:contact.name,
      phone:contact.phone,
      contactType:this.contactType,
      mobile:contact.mobile,
      homePage:contact.homePage,
      isVisible:[true],
      address: contact!.address != null ? this.generatAddressFromAddressValue(contact.address):this.generatAddress(),
      userId:'',
      password:contact.password,
      belongedTo:contact.belongedTo
    })
  }

  addNewContact():void
  {
    let fg= this.generateContactFromContactValue(this.newContactForm.getRawValue() as Contact);

    //let addedContact = this.newContactForm.getRawValue() as Contact;
   // this.findedContacts?.push(fg);
    this.selectedContacts?.push(fg);
    //console.log(this.selectedContacts);
  }

  suggestNewPassword():string
  {
    let length =12;

const upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
  const lower = "abcdefghijklmnopqrstuvwxyz";
  const digits = "0123456789";
  const special = "!@$?_-" ; // Match your PasswordOptions if customized

  const allChars = upper + lower + digits + special;
  const getRandom = (chars) => chars[Math.floor(Math.random() * chars.length)];

  // Ensure minimum complexity: one of each required type
  let password = [
    getRandom(upper),
    getRandom(lower),
    getRandom(digits),
    getRandom(special)
  ];

  // Fill the rest of the password
  for (let i = password.length; i < length; i++) {
    password.push(getRandom(allChars));
  }

  // Shuffle to avoid predictable patterns
  for (let i = password.length - 1; i > 0; i--) {
    const j = Math.floor(Math.random() * (i + 1));
    [password[i], password[j]] = [password[j], password[i]];
  }

  this.newContactForm.controls['password'].patchValue(password.join(''));
  return password.join('');

}

  addToSelectedContact(contact)
  {
    let sg = this.generateContactFromContactValue(contact);
    this.selectedContacts?.push(sg);
    const selIndex = this.findedContacts.value.findIndex(cont=>cont.id == contact.id);
    if(selIndex >=0 ){
      this.findedContacts.removeAt(selIndex);
      this.existingIds.delete(contact.id);
    }
  }

  deleteContact(contact:Contact)
  {
     // 1) find index in selectedContacts
     let findedIndex =  this.selectedContacts.value.findIndex((fcontact)=> fcontact.id == contact.id);
    //const selIndex = this.selectedContacts.controls.indexOf(contact);
    if (findedIndex >= 0) {
      this.selectedContacts.removeAt(findedIndex);
      this.findedContacts.push(this.generateContactFromContactValue(contact));
      this.existingIds.add(contact.id);
    }
  }

//   deleteContactByIndex(i: number) {
//   const formArray = this.selectedContacts as FormArray;
//   if (i >= 0 && i < formArray.length) {
//     formArray.removeAt(i);
//     this.findedContacts.push(this.generateContactFromContactValue(contact));
//       this.existingIds.add(contact.id);
//   }
// }

  close()
  {
    this.dialogRef.close(this.selectedContacts)
  }
}
