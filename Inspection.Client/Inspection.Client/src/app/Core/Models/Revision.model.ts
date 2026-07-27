import { Circulation } from "./Circulation.model";
import { Contact } from "./Contact.model";
import { Inspection } from "./Inspection.model";
import { InspectionReport } from "./InspectionReport.model";

export interface Revision{
revisionId :string,
requestId :string,
versionNumber :string,
revisionNumber :string,
submittedDate :string,

// Instead of full Contact entity, expose minimal info
submittedById :string,
submittedBy :Contact,

comment :string,
status :string,
isFinal :boolean,
createdByCirculationId:string,
// Circulations summarized
circulations :Circulation[],

// Subtype references (IDs or summaries)
inspectionNotifications :Inspection[],
inspectionReports :InspectionReport[]

}
