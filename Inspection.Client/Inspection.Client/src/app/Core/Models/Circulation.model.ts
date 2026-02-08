import { ActionType } from "./ActionType.model";
import { Attachment } from "./Attachment.model";
import { Contact } from "./Contact.model";
import { ContactTypes } from "./ContactTypes.enum";
import { Revision } from "./Revision.model";

export interface Circulation
{
    id: string,
    revisionId: string,
    actionType :number,
     performerPN :string,
     performedActionDate :string ,
     sequenceNo :number,
     performedActionDateGregorian :Date | undefined | null ,
     performerContactPerson :Contact,
     performerContactType :ContactTypes,
     qCActionType :number,
     description:string,
     createdDate:Date,
     modifiedDate : Date,
     comment :string
     attachments: Attachment[]
}
