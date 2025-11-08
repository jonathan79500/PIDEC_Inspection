import { Circulation } from "./Circulation.model";
import { Contact } from "./Contact.model";
import { Inspection } from "./Inspection.model";
import { InspectionNotificationContact } from "./InspectionNotificationContact.model";
import { InspectionNotificationSubject } from "./InspectionNotificationSubject.model";
import { InspectionNotificationType } from "./InspectionNotificationType.model";

export interface InspectionNotificationFlow{

  inspectionNotifications:Inspection[],
  circulations:Circulation[],
  contacts:InspectionNotificationContact[],
  subjects: InspectionNotificationSubject[],
  types:InspectionNotificationType[]
}