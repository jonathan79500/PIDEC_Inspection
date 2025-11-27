import { ActionType } from "./ActionType.model";
import { Attachment } from "./Attachment.model";
import { Contact } from "./Contact.model";
import { ContactTypes } from "./ContactTypes.enum";

export interface Circulation
{
    id: string,
    //requestId :string,
    actionType :number,
     performerPN :string,
     performedActionDate :string,
     sequenceNo :number,
     performedActionDateGregorian :Date,
     performerContactPerson :Contact,
     performerContactType :ContactTypes,
     createdDate:Date,
     modifiedDate : Date,
     comment :string
     attachments: Attachment[]
}
