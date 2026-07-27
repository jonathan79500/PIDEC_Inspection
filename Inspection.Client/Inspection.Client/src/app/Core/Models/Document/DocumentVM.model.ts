import { DocumentVersionVM } from "./DocumentVersionVM.model";

export interface DocumentVM {

    id: string;

    title: string;

    documentTypeId: string;

    documentTypeName: string;

    currentVersion: DocumentVersionVM;

    versions: DocumentVersionVM[];

}
