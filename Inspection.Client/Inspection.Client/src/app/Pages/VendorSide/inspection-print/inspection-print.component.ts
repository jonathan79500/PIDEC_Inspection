import { Component, inject, OnInit } from '@angular/core';
import { Inspection } from '../../../Core/Models/Inspection.model';
import { toGregorian, toJalaali } from 'jalaali-js';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Attachment } from '../../../Core/Models/Attachment.model';
import { Contact } from '../../../Core/Models/Contact.model';
import { ContactTypes } from '../../../Core/Models/ContactTypes.enum';
import { ActivatedRoute, Router } from '@angular/router';
import { currentstateStore } from '../../../Stores/currentstate.store';
import { Project } from '../../../Core/Models/Project.model';
import { json } from 'stream/consumers';

@Component({
  selector: 'app-inspection-print',
  imports: [ReactiveFormsModule,FormsModule,CommonModule],
  templateUrl: './inspection-print.component.html',
  styleUrl: './inspection-print.component.css'
})
export class InspectionPrintComponent implements OnInit{

   inspectionData: Inspection;
    inspectionMainNumber: string='';
    inspectionRevNumber :string;
    formattedGregorianIssueDate: string = '';
    formattedPersianIssueDate: string = '';
    attachments: Attachment[] = [];
    formatedRequestInspectionPersianDateStart :string='';
    formatedRequestInspectionPersianDateEnd :string ='';
    requestInspectionDateStart :string='';
    requestInspectionDateEnd :string ='';
    requestInspectionDueDate :string ='';
    inspectionTimeComment :string ='';
    equipmentTagNo :string = '';
    orderName :string = '';
    orderTitle :string ='';
    toContacts : Contact[];
    vendorContact:Contact;
    inspectionActions: string[] = [];
    receiverContacts :Contact[];
    private dataKey: string | null = null;
    currentAppState = inject(currentstateStore);
    currentProject:Project;
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



  constructor(private route: ActivatedRoute, private router: Router)  {
    // const state = history.state;

    // if (state && state.inspection) {
    //   this.inspectionData = state.inspection;
    // } else {
    //   // fallback (e.g. redirect or load from service)
    //   console.error('No state data found');
    // }
   // this.inspectionData = JSON.parse(sessionStorage.getItem('inspectionData') || '{}');
  }

  ngOnInit(): void {
  //   const data = sessionStorage.getItem('inspectionData');
  // if (data) {
  //   this.inspectionData = JSON.parse(data);
  // }
   this.route.queryParams.subscribe(params => {
      //const encodedData = params['data'];
      const key = params['dataKey'];
      const projKey = params['projKey'];
      if (key) {
        this.dataKey = key; // Store the key
        const data = sessionStorage.getItem(key);
        const projectData = sessionStorage.getItem(projKey);
       if (data) {
          try {
            this.inspectionData = JSON.parse(data);
            this.currentProject = JSON.parse(projectData);
            // Optional: Immediately remove from session storage once read
            // to prevent it from lingering if the window stays open
            // sessionStorage.removeItem(key);
        //  this.inspectionData = JSON.parse(decodeURIComponent(encodedData));

      //  this.currentProject = this.currentAppState.currentproject();
          const [mainNo, rev] = this.inspectionData.notificationNumber.split('.');
    this.inspectionMainNumber = mainNo;
    this.inspectionRevNumber = rev;
    this.formattedPersianIssueDate = this.getFormattedPersianDate(this.inspectionData.issueDate);
    this.formattedGregorianIssueDate = this.getFormattedGregorianDate(this.inspectionData.issueDate);
    this.formatedRequestInspectionPersianDateStart =this.getFormattedPersianDate(this.inspectionData.requestInspectionDateStart)
    this.formatedRequestInspectionPersianDateEnd =this.getFormattedPersianDate(this.inspectionData.requestInspectionDateEnd)
    this.requestInspectionDateStart = new Date(this.inspectionData.requestInspectionDateStart).toLocaleDateString();
    this.requestInspectionDateEnd =new Date (this.inspectionData.requestInspectionDateEnd).toLocaleDateString();
    this.requestInspectionDueDate =this.inspectionData.requestInspectionDueDate.toString();
    this.inspectionTimeComment = this.inspectionData.inspectionTimeComment;
    this.equipmentTagNo = this.inspectionData.equipmentTagNo;
    this.orderName = this.inspectionData.orderName;
    this.orderTitle = this.inspectionData.order.name;

     if (this.inspectionData.inspectionAction)
    {
      this.inspectionActions = this.inspectionData.inspectionAction.split(',');
    }
    this.receiverContacts = this.inspectionData.contacts.filter((cnt)=>cnt.contactType != ContactTypes.QC_Lead && cnt.contactType != ContactTypes.QC_Manager && cnt.contactType != ContactTypes.Vendor && cnt.email.trim() !='');
    this.toContacts = this.inspectionData.contacts.filter((cnt)=> cnt.contactType == ContactTypes.QC_Manager || cnt.contactType == ContactTypes.QC_Lead) ;
    this.vendorContact = this.inspectionData.contacts.filter((cnt)=> cnt.contactType == ContactTypes.Vendor)[0] as Contact;

          console.log('Inspection data loaded:', this.inspectionData); // For debugging

        } catch (e) {
          console.error('Error parsing inspection data:', e);
        }
      }
     } else {
        console.warn('No inspection data found in query params.');
        // Handle cases where data might be missing (e.g., direct URL access)
        // You might want to navigate away or show an error message.
      }
    });


  }

  /*Helper methods*/

   log(tcnt)
    {
      console.log("In Template contact have value",tcnt,"for name");
    }
  /*End of Helper methods*/


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

   getFormattedPersianDate(date: any) :string{

      try {
        // Get the selected Gregorian date
        const gregorianDate = new Date(date);

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
       return `${jd} ${persianMonthName} ${jy}`;

      } catch (error) {
        return "error";
      } finally {

      }
    }

getFormattedGregorianDate(date: any): string {

    try {
      // Get Gregorian date from the event
      const viaGregorianDate = new Date(date);

      // Format Gregorian date with month name
      const gregorianDay = viaGregorianDate.getDate();
      const gregorianMonth = viaGregorianDate.getMonth();
      const gregorianYear = viaGregorianDate.getFullYear();
      const gregorianMonthName = this.getGregorianMonthName(gregorianMonth);

      // Set formatted Gregorian date with month name
     return `${gregorianDay} ${gregorianMonthName} ${gregorianYear}`;

    } catch (error) {
      return "error Miladi Date";
    }
  }
}
