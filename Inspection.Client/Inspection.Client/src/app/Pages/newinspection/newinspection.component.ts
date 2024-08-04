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
    projectId: 'dd0c2863-d272-400d-b3b2-de39df43ae01',
    no: 'Order01',
    title: 'Order01',
    name: 'Order01',
    createdDate: undefined,
    modifiedDate: undefined,
    createdBy: '',
    modifiedBy: ''
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
      order: this.order,
      city: 'shiraz',
      notificationNumber: '02 Inspection',
      inspectionSubject: 'Inspection Notification',
      contractShippingDueDate: new Date(Date.now()),
      ActualShippingDueDate: new Date(Date.now()),
      proposedDate: new Date(Date.now()),
      ownerId: '',
      locationId: ''
    }
    
   this.inspectionService.saveNewInspection(this.newInspection);
  }

}
