import { DocumentOwnerType } from "./DocumentOwnerType.model";

export interface DocumentDialogData {

    ownerId: string;

    ownerType: DocumentOwnerType;

    readOnly: boolean;

}
