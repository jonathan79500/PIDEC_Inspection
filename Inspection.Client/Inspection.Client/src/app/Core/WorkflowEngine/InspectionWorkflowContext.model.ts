import { Circulation } from "../Models/Circulation.model";
import { Inspection } from "../Models/Inspection.model";
import { Revision } from "../Models/Revision.model";
import { User } from "../Models/User.model";

export class InspectionWorkflowContext{

    inspections : Inspection[];

    inspection!:Inspection;

    revision!:Revision;

    currentCirculation!:Circulation;

    lastVendorCirculation?:Circulation;

    lastQCCirculation?:Circulation;

    editMode=false;

    currentRevision?: Revision;

    currentUser?:User;

}
