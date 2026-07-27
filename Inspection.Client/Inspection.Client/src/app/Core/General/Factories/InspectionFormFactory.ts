// import { Injectable } from "@angular/core";
// import { FormBuilder, FormGroup } from "@angular/forms";
// import { InspectionWorkflowViewModel } from "../../WorkflowEngine/InspectionWorkflowViewModel.model";

// @Injectable({
//     providedIn: 'root'
// })
// export class InspectionFormFactory {

//     constructor(
//         private fb: FormBuilder
//     ) {}

//     build(
//         vm: InspectionWorkflowViewModel
//     ): FormGroup {

//         const inspection = vm.inspection;

//         return this.fb.group({

//             id: inspection.id,

//             order:
//                 inspection.order
//                     ? this.generateOrderFromValue(
//                         inspection.order)
//                     : this.generateOrder(),

//             revNumber:
//                 vm.shownRevision,

//             notificationNumber:
//                 inspection.notificationNumber,



//         });

//     }

// }
