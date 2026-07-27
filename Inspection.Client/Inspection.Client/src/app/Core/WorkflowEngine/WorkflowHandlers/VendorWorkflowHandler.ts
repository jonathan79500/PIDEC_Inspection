import { Injectable } from "@angular/core";
import { IWorkflowHandler } from "../Interfaces/IWorkflowHandler";
import { InspectionWorkflowContext } from "../InspectionWorkflowContext.model";
import { ContactTypes } from "../../Models/ContactTypes.enum";
import { InspectionWorkflowViewModel } from "../InspectionWorkflowViewModel.model";
import { Circulation } from "../../Models/Circulation.model";
import { WorkflowSearchHelper } from "../WorkflowSearchHelper";
import { WorkflowHandlerBase } from "./WorkflowHandlerBase";

@Injectable()
export class VendorWorkflowHandler
extends WorkflowHandlerBase {

    override build(
        context: InspectionWorkflowContext
    ): InspectionWorkflowViewModel {

        const vm =
            this.buildBaseViewModel(context);

        vm.editMode =
            this.isEditable(context);

        const previousVendor =
            this.helper.findPreviousVendorCirculation(context);

        vm.attachments =
            structuredClone(
                previousVendor?.circulation.attachments ?? []
            );

        const lastQC =
            this.helper.findLastQCCirculation(
                context.inspections
            );

        vm.requestCirculations =
            lastQC
                ? [lastQC.circulation]
                : [];

        vm.requestCirculationsVM =
            vm.requestCirculations.map(c => ({

                original: c,

                current:
                    this.helper.findLastAssignedCirculation(
                        c,
                        context.currentRevision.circulations
                    )

            }));

        vm.allCirculationsInCurrentRevision =
            context.currentRevision.circulations;

        vm.shownRevisionNumber =
          this.helper.calculateShownRevisionNumber(
            context.inspection.notificationNumber,
          true);

          vm.shownNotificationNumber = this.helper.CalculateShownNotificationNumber(
            context.inspection.notificationNumber
          );

        return vm;
    }

     canHandle(context: InspectionWorkflowContext): boolean {

        return context.currentCirculation.performerContactType ==
                ContactTypes.Vendor;
    }


}


//ShownRevNumberShouldIncrease = true;
//     let recentVendorcirculationWithActionFound : boolean = false;
//     let recentVendorcirculationWithAction :Circulation = null;
//     insNum=0;
//     while(!recentVendorcirculationWithActionFound && insNum<inspectionLength)
//     {
//       for(let r=0;r<inspections[insNum].revisions.length;r++){
//         if(inspections[insNum].revisions[r].revisionNumber <= currentUserRevisionNumber &&
//            inspections[insNum].revisions[r].submittedDate<currentUserRevision.submittedDate){

//         recentVendorcirculationWithAction = inspections[insNum].revisions[r].circulations.find((circ)=>{
//          return circ.id == currentUserRevision.createdByCirculationId;
//         });

//         if(recentVendorcirculationWithAction != null)
//         {
//           this.attachments = structuredClone(recentVendorcirculationWithAction.attachments)
//           recentVendorcirculationWithActionFound= true;
//           break;
//         }
//       }
//       }
//       insNum = insNum+1;
//     }
//  insNum=0;
//   while(!LastQCCirculationFound && insNum<inspectionLength)
//   {
//   for(let r=0;r<inspections[insNum].revisions.length;r++)
//     {
//       LastQCCirculation = inspections[insNum].revisions[r].circulations.find((circ)=>(circ.performerContactType == ContactTypes.QC_Lead && circ.performedActionDateGregorian.toString().trim() != '0001-01-01T00:00:00')
//        || (circ.performerContactType == ContactTypes.QC_Manager && circ.performedActionDateGregorian.toString().trim() != '0001-01-01T00:00:00'));
//       if(LastQCCirculation != null)
//       {
//         LastQCCirculationFound = true;
//         LastInspectionSentByQC = inspections[insNum];
//         break;
//       }
//     }
//     insNum=insNum+1;
//    }
//    this.requestCirculations = [LastQCCirculation];
//    this.allCirculationsInCurrentRevision = currentUserInspection.revisions[0].circulations;
//    this.requestCirculationsVM =
//     this.requestCirculations.map(c => ({
//       original : c,
//       current: c}));
