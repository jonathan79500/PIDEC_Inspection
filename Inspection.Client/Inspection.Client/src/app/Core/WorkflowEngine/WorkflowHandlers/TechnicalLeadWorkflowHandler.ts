import { InspectionWorkflowContext } from "../InspectionWorkflowContext.model";
import { ContactTypes } from "../../Models/ContactTypes.enum";
import { InspectionWorkflowViewModel } from "../InspectionWorkflowViewModel.model";
import { Circulation } from "../../Models/Circulation.model";
import { WorkflowSearchHelper } from "../WorkflowSearchHelper";
import { WorkflowHandlerBase } from "./WorkflowHandlerBase";
import { Injectable } from "@angular/core";

@Injectable()
export class TechnicalLeadWorkflowHandler extends WorkflowHandlerBase {


    canHandle(context: InspectionWorkflowContext): boolean {

        return context.currentCirculation.performerContactType ==
                ContactTypes.QC_Lead

            ||

               context.currentCirculation.performerContactType ==
                ContactTypes.QC_Manager;
    }

    build(context: InspectionWorkflowContext,vm: InspectionWorkflowViewModel): InspectionWorkflowViewModel {


        vm.editMode = this.isEditable(context);

        const lastVendor =
            this.helper.findLastVendorCirculation(context.inspections);

        vm.attachments = structuredClone(lastVendor?.circulation.attachments ?? []);

        vm.currentCirculation = context.currentCirculation;

        vm.allCirculationsInCurrentRevision =
            context.currentRevision.circulations;

        vm.requestCirculations =
            this.buildRequestCirculations(context);

        vm.requestCirculationsVM =
            vm.requestCirculations.map(c => ({
                original: c,
                current: this.helper.findLastAssignedCirculation(
                    c,
                    vm.allCirculationsInCurrentRevision)
            }));

        vm.currentCirculation.childCirculations =
            this.helper.findChildren(
                vm.currentCirculation,
                vm.allCirculationsInCurrentRevision);

              vm.shownRevisionNumber =
          this.helper.calculateShownRevisionNumber(
          context.currentRevision.revisionNumber,
          false);

        return vm;
    }

    public override isEditable(
        context: InspectionWorkflowContext): boolean {

        return context.currentCirculation
            .performedActionDateGregorian
            ?.getFullYear() == 1;
    }

    private buildRequestCirculations(
        context: InspectionWorkflowContext): Circulation[] {

        return context.currentRevision.circulations
            .filter(c =>
                c.performerContactType != ContactTypes.Vendor
                &&
                c.sequenceNo < 2);
    }

     override isActive(
    circulation: Circulation,
    context: InspectionWorkflowContext
): boolean {

    if (!this.canCurrentUserAct(circulation, context))
        return false;

    return true;
}

}
