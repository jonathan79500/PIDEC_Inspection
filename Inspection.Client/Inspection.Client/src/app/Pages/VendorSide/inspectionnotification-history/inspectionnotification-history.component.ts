import { Revision } from './../../../Core/Models/Revision.model';
import { InspectionService } from './../../../Core/Services/inspection.service';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { InspectionNotificationHistory } from '../../../Core/Models/InspectionNotificationHistory.model';

@Component({
  selector: 'app-inspectionnotification-history',
  imports: [],
  templateUrl: './inspectionnotification-history.component.html',
  styleUrl: './inspectionnotification-history.component.css'
})
export class InspectionnotificationHistoryComponent implements OnInit{

  /**
   *
   */
  constructor(private route: ActivatedRoute, private router: Router,private inspectionService: InspectionService) {


  }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      //const encodedData = params['data'];
      const requestId = params['requestId'];
      let historyData: InspectionNotificationHistory[];
      this.inspectionService.getInspectionNotificationHistoryByRequestId(requestId).subscribe((result)=>{

        historyData = result;
        ProcessHistoricalData(historyData);

      })
  })
}


}
function ProcessHistoricalData(historyData: InspectionNotificationHistory[]) {

  let historyRecordsCount = historyData.length;
  let RevisionId:string;
  while(historyRecordsCount>0)
  {

  }
}

