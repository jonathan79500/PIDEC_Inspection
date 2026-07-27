import { Circulation } from "../../Models/Circulation.model";
import { InspectionWorkflowContext } from "../InspectionWorkflowContext.model";
import { InspectionWorkflowViewModel } from "../InspectionWorkflowViewModel.model";

export interface IWorkflowHandler {

    canHandle(context: InspectionWorkflowContext): boolean;

    build(context: InspectionWorkflowContext,vm:InspectionWorkflowViewModel): InspectionWorkflowViewModel;

    isActive(circulation: Circulation,context: InspectionWorkflowContext): boolean;
}
