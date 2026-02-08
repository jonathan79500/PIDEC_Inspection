import { Component, OnInit } from '@angular/core';
import { Inspection } from '../../../Core/Models/Inspection.model';
import { toGregorian, toJalaali } from 'jalaali-js';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Attachment } from '../../../Core/Models/Attachment.model';
import { Contact } from '../../../Core/Models/Contact.model';
import { ContactTypes } from '../../../Core/Models/ContactTypes.enum';

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



  constructor() {
    const state = history.state;

    if (state && state.inspection) {
      this.inspectionData = state.inspection;
    } else {
      // fallback (e.g. redirect or load from service)
      console.error('No state data found');
    }
  }

  ngOnInit(): void {
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

    this.toContacts = this.inspectionData.contacts.filter((cnt)=>{ cnt.contactType == ContactTypes.QC_Manager || cnt.contactType == ContactTypes.QC_Lead}) as Contact[];
    this.vendorContact = this.inspectionData.contacts.filter((cnt)=>{ cnt.contactType == ContactTypes.Vendor})[0] as Contact;
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
