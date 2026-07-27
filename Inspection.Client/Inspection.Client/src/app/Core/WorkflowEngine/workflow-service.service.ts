import * as helpers from './WorkflowSearchHelper';
import { Inject, Injectable } from '@angular/core';
import { Inspection } from '../Models/Inspection.model';
import { InspectionWorkflowViewModel } from './InspectionWorkflowViewModel.model';
import { Circulation } from '../Models/Circulation.model';
import { InspectionWorkflowContext } from './InspectionWorkflowContext.model';
import { Revision } from '../Models/Revision.model';
import { User } from '../Models/User.model';
import { ContactTypes } from '../Models/ContactTypes.enum';
import { WORKFLOW_HANDLERS } from './WorkflowHandlers/workflow-handler.token';
import { IWorkflowHandler } from './Interfaces/IWorkflowHandler';

@Injectable({
  providedIn: 'root'
})
export class WorkflowServiceService {

   private currentHandler!: IWorkflowHandler;

  constructor(private helperCs:helpers.WorkflowSearchHelper,@Inject(WORKFLOW_HANDLERS)
    private handlers: IWorkflowHandler[]) {
      console.log(this.handlers);
     }

  buildWorkflow(inspections:Inspection[],  currentUser : User):InspectionWorkflowViewModel
  {

    this.normalizeDates(inspections);

    const vm = new InspectionWorkflowViewModel();
    const context =
            this.findCurrentActor(inspections, currentUser);

        if (!context.editMode)
            return this.buildReadOnlyWorkflow(context,currentUser);

        const handler =
            this.handlers.find(h => h.canHandle(context));

        if (!handler)
            throw new Error(
                `No handler for ${context.currentCirculation.performerContactType}`
            );

        this.currentHandler = handler;
        handler.build(context,vm);

        return vm;

  }

private toDate(value: unknown): Date | null {

    if (!value) {
        return null;
    }

    if (value instanceof Date) {
        return value;
    }

    return new Date(value as string);
}

  private normalizeDates(inspections: Inspection[]): void {

    inspections.forEach(inspection => {

        inspection.createdDate =
            this.toDate(inspection.createdDate);

        inspection.modifiedDate =
            this.toDate(inspection.modifiedDate);

        inspection.issueDate =
            this.toDate(inspection.issueDate);

        inspection.requestInspectionDateStart =
            this.toDate(inspection.requestInspectionDateStart);

        inspection.requestInspectionDateEnd =
            this.toDate(inspection.requestInspectionDateEnd);



        inspection.revisions.forEach(revision => {


            revision.circulations.forEach(c => {

                c.performedActionDateGregorian =
                    this.toDate(c.performedActionDateGregorian);

            });

        });

    });

}
private buildOtherUserRequests(
    context: InspectionWorkflowContext,
    lastQC: Circulation | null,
    lastVendor: Circulation | null,
    circulations: Circulation[],
    currentUser:User
): Circulation[] {

    const result: Circulation[] = [];

    if (lastQC)
        result.push(lastQC);

    const own =
        this.helperCs.findLastUserCirculation(
            currentUser.id,
            context.inspections);

    if (own)
        result.push(own.circulation);

    if (!lastQC && lastVendor) {

        const children =
            this.helperCs.findChildren(
                lastVendor,
                circulations);

        lastVendor.childCirculations =
            children;

        result.length = 0;

        result.push(lastVendor);

    }

    return result;
}
private buildReadOnlyRequests(
    context: InspectionWorkflowContext,
    lastQC: Circulation | null,
    lastVendor: Circulation | null,
    circulations: Circulation[],
    currentUser:User
): Circulation[] {

    const type =
        context.currentCirculation.performerContactType;

    switch (type) {

        case ContactTypes.QC_Lead:
        case ContactTypes.QC_Manager:

            return circulations.filter(c =>
                c.performerContactType != ContactTypes.Vendor);

        case ContactTypes.Vendor:

            return lastQC
                ? [lastQC]
                : [];

        default:

            return this.buildOtherUserRequests(
                context,
                lastQC,
                lastVendor,
                circulations,
              currentUser);
    }

}
  buildReadOnlyWorkflow(context: InspectionWorkflowContext,currentUser:User): InspectionWorkflowViewModel {
   const vm = new InspectionWorkflowViewModel();



    const lastQC =
        this.helperCs.findLastQCCirculation(context.inspections);


    const lastVendor =
        this.helperCs.findLastVendorCirculation(context.inspections);

    if(lastVendor.circulation != null && lastVendor.circulation.id != '00000000-0000-0000-0000-000000000000')
        vm.inspection = lastQC.inspection;



    const latestRevision =
        lastQC?.revision
        ?? lastVendor?.revision
        ?? context.currentRevision;

    vm.editMode = false;

    vm.currentCirculation =
        context.currentCirculation;

    vm.allCirculationsInCurrentRevision =
        latestRevision?.circulations ?? [];

    vm.attachments =
        structuredClone(
            lastVendor?.circulation.attachments ?? []
        );

    vm.requestCirculations =
        this.buildReadOnlyRequests(
            context,
            lastQC?.circulation,
            lastVendor?.circulation,
            vm.allCirculationsInCurrentRevision,
            currentUser
        );

    vm.requestCirculationsVM =
        vm.requestCirculations.map(c => ({
            original: c,
            current: this.helperCs.findLastAssignedCirculation(
                c,
                vm.allCirculationsInCurrentRevision
            )
        }));


         vm.shownRevisionNumber =
          this.helperCs.calculateShownRevisionNumber(
            context.inspection.notificationNumber,
          false);

          vm.shownNotificationNumber = this.helperCs.CalculateShownNotificationNumber(
            context.inspection.notificationNumber
          );

    return vm;
  }

  findCurrentActor(inspections:Inspection[],currentUser:User):InspectionWorkflowContext{

   const currentActor : InspectionWorkflowContext= new InspectionWorkflowContext();

   currentActor.currentUser = currentUser;
   currentActor.inspections = inspections;

    let inspectionLength : number=0;
    let currentUserLastCirculationFound : boolean= false;
    let currentUserCirculation :Circulation = null;
    let insNum: number=0;


inspectionLength = inspections.length;

while(!currentUserLastCirculationFound && insNum<inspectionLength)
{
  for(let r=0;r<inspections[insNum].revisions.length;r++)
  {
    let currentUserAllCirculations = inspections[insNum].revisions[r].circulations.filter((circ)=>circ.performerContactPerson.userId == currentUser.id).sort((a,b)=>{
          return b.sequenceNo - a.sequenceNo;
      });
    currentUserCirculation = currentUserAllCirculations[0];
    if(currentUserCirculation != null)
    {
      currentActor.inspection = inspections[insNum];
      currentActor.currentRevision = inspections[insNum].revisions[r];
      break;
    }
  }
  insNum=insNum+1;
}
currentActor.currentCirculation = currentUserCirculation;
return currentActor;
}

}
