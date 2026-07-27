export interface InspectionType{

  id:string,
  name:string,
  createdBy_ContactId :string,
  createdDate :Date |null,
 visibleByDefault :boolean,
  showOrderNo :number,
  selected : boolean
}
