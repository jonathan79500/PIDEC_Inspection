import { Order } from "./Order.model"
import { ProjectContact } from "./ProjectContact.model"

export interface OrderSettingsException{

 id :string | '00000000-0000-0000-0000-000000000000',
 orderId : string |'00000000-0000-0000-0000-000000000000',
 projectContactId : string |'00000000-0000-0000-0000-000000000000',
 order :Order,
 projectContact : ProjectContact
}
