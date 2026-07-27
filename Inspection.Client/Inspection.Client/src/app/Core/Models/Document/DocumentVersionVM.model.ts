import { DocumentStatus } from "./DocumentStatus.model";

export interface DocumentVersionVM {

    id: string;

    versionNo: number;

    fileName: string;

    extension: string;

    size: number;

    status: DocumentStatus;

    remarks?: string;

    createdDate: Date;

    createdBy: string;

}
