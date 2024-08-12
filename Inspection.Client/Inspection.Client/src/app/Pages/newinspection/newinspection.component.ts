import { Component } from '@angular/core';
import { Inspection } from '../../Core/Models/Inspection.model';
import { InspectionService } from '../../Core/Services/inspection.service';
import { Order } from '../../Core/Models/Order.model';

@Component({
  selector: 'app-newinspection',
  standalone: true,
  imports: [],
  templateUrl: './newinspection.component.html',
  styleUrl: './newinspection.component.css'
})
export class NewinspectionComponent {

  order :Order ={
    id :'a5dcc11b-846a-4d16-91e1-1dfb9b86c4e2',
    projectId: 'dd0c2863-d272-400d-b3b2-de39df43ae01',
    no: 'Order01',
    title: 'Order01',
    name: 'Order01',
    createdDate:new Date(Date.now()),
    modifiedDate: new Date(Date.now()),
    createdBy: 'asda',
    modifiedBy: 'adad'
  };
  newInspection !:Inspection ;
  /**
   *
   */
  constructor(private inspectionService:InspectionService) {
   
}


  SaveNewInspection():void
  {

    

    this.newInspection  = {
      name: 'Inspection02',
      // order: this.order,
       orderId:'81882025-6495-48b6-b570-e667fdd09e3f',
       city: 'shiraz',
       notificationNumber: '02 Inspection',
       inspectionSubject: 'Inspection Notification',
       inspectionTypeId:'7f2a9ffc-fd81-4fd1-adfd-e5032cac15e6',
       contractShippingDueDate: new Date(Date.now()),
       ActualShippingDueDate: new Date(Date.now()),
       proposedDate: new Date(Date.now()),
       ownerId:'a5860223-4456-41dd-8948-2dda8009a0d9',
       locationId: '2368e8cc-90fb-4472-a8f1-baf1a8e988ee',
      // locationId: ''
    }
    
   this.inspectionService.saveNewInspection(this.newInspection).subscribe((result)=>{
    console.log(result);
   });
  }

}
