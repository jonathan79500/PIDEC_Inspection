export interface InspectorAttachment{
 id :string
 name :string
 description :string
 content : Uint8Array[],
 size :string
 attachmentTypes : number //PackingList = 0, non-Packing=1

}
