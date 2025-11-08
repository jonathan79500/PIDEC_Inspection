import { InspectionSubject } from "./InspectionSubject.model"
import { Order } from "./Order.model"

export interface OrderInspectionSubject
{

  id :string
 order :Order
 orderId:string
inspectionSubject:InspectionSubject
inspectionSubjectId:string
}