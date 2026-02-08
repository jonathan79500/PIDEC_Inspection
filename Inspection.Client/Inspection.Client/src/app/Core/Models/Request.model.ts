import { Contact } from "./Contact.model";
import { Revision } from "./Revision.model";

export interface Request{
 id :string,
 creatorContactPerson:Contact,
 createdDate :Date,
 modifiedDate :Date,
 createdBy :Contact,
modifiedBy :Contact,
currentRevisionId :string,
 revisions:Revision[],
 requestType :string,
 currentStatus :string
}
