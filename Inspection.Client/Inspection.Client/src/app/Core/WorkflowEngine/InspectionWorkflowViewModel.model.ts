import { Attachment } from "../Models/Attachment.model";
import { Circulation } from "../Models/Circulation.model";
import { CirculationVM } from "../Models/CirculationVM.model";
import { Inspection } from "../Models/Inspection.model";
import { Revision } from "../Models/Revision.model";

export class InspectionWorkflowViewModel {

    inspection: Inspection;

    currentRevision: Revision;

    currentCirculation: Circulation;

    requestCirculations: Circulation[];

    requestCirculationsVM: CirculationVM[];

    attachments: Attachment[];

    editMode:boolean;

    shownRevision:string;

    shownNotificationNumber!: string;

    shownRevisionNumber!: string;

    allInternalCorrespondences:Circulation[];

    allCirculationsInCurrentRevision:Circulation[];

}
