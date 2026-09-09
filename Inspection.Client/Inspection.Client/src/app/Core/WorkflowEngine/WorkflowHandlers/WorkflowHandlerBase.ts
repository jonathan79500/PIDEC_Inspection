import { Circulation } from './../../Models/Circulation.model';
import { Injectable } from "@angular/core";
import { InspectionWorkflowContext } from "../InspectionWorkflowContext.model";
import { InspectionWorkflowViewModel } from "../InspectionWorkflowViewModel.model";
import { IWorkflowHandler } from "../Interfaces/IWorkflowHandler";
import { WorkflowSearchHelper } from "../WorkflowSearchHelper";

@Injectable()
export abstract class WorkflowHandlerBase
implements IWorkflowHandler
{
    protected context: InspectionWorkflowContext;
    constructor(
        protected helper: WorkflowSearchHelper
    ) {}

    abstract canHandle(
        context: InspectionWorkflowContext): boolean;

    abstract build(
        context: InspectionWorkflowContext,vm: InspectionWorkflowViewModel): InspectionWorkflowViewModel;

    abstract isActive(circulation: Circulation,context: InspectionWorkflowContext): boolean ;

    protected canCurrentUserAct(circulation: Circulation,
        context: InspectionWorkflowContext): boolean {

     return circulation.performerContactPerson?.id ===
               context.currentCirculation.performerContactPerson?.id
            &&
               this.isUnperformed(circulation);

}
protected isUnperformed(c: Circulation): boolean {

    if (!c.performedActionDateGregorian) {
        return true;
    }

    return new Date(c.performedActionDateGregorian)
        .getFullYear() === 1;

}

    protected isEditable (
        context: InspectionWorkflowContext): boolean {

        return context.currentCirculation
            .performedActionDateGregorian
            ?.getFullYear() == 1;
    }

    protected buildBaseViewModel(
        context: InspectionWorkflowContext)
    {
        const vm =
            new InspectionWorkflowViewModel();

        vm.currentCirculation =
            context.currentCirculation;

        vm.allCirculationsInCurrentRevision =
            context.currentRevision.circulations;

        return vm;
    }
}
