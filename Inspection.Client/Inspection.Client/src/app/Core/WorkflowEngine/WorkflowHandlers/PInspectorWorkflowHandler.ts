import { InspectionWorkflowContext } from "../InspectionWorkflowContext.model";
import { ContactTypes } from "../../Models/ContactTypes.enum";
import { InspectionWorkflowViewModel } from "../InspectionWorkflowViewModel.model";
import { Circulation } from "../../Models/Circulation.model";
import { WorkflowSearchHelper } from "../WorkflowSearchHelper";
import { WorkflowHandlerBase } from "./WorkflowHandlerBase";


export class PInspectorWorkflowHandler extends WorkflowHandlerBase {



    canHandle(context: InspectionWorkflowContext): boolean {

        return context.currentCirculation.performerContactType ==
                ContactTypes.PIDEC_Inspector
    }

    build(context: InspectionWorkflowContext,vm: InspectionWorkflowViewModel): InspectionWorkflowViewModel {



        vm.editMode = this.isEditable(context);

        const lastVendor =
            this.helper.findLastVendorCirculation(context.inspections);

         vm.inspection = lastVendor.inspection;

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

    public override  isEditable(
        context: InspectionWorkflowContext): boolean {

               const actionDate = context.currentCirculation.performedActionDateGregorian
    ? new Date(context.currentCirculation.performedActionDateGregorian)
    : null;

const editable =
    actionDate?.getFullYear() === 1;
        return editable;

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
