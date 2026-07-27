
export interface Document
{
    documentId: string,
    attachmentId: string,
    ownerType :string,
     ownerId :string,
     documentTypeId:string,
     status :string ,
     approvedBy :string,
     approvedDate :Date | undefined | null ,
     issueDate :Date | undefined | null ,
     expireDate :Date | undefined | null ,
     version:string
}
