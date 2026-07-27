import { Injectable } from "@angular/core";
import { Circulation } from "../../Models/Circulation.model";
import { ContactTypes } from "../../Models/ContactTypes.enum";

@Injectable({providedIn:'root'})
export class CirculationFactory {

    create(): Circulation {

        return {

                   id: '00000000-0000-0000-0000-000000000000',
                   actionType: 0,
                   attachments: [],
                   childCirculations: [],
                   comment: '',
                   createdBy: '',
                   createdDate: new Date(),
                   description: '',
                   isHidden: false,
                   modifiedDate: new Date(),
                   performedActionDate: '',
                   performedActionDateGregorian: new Date(),
                   performerContactPerson: {
                     id: '',
                     address: undefined,
                     phone: '',
                     fax: '',
                     name: '',
                     email: '',
                     homePage: '',
                     note: '',
                     contactType: 0,
                     mobile: '',
                     cc: '',
                     userId: '',
                     isVisible: false,
                     password: '',
                     belongedTo: 0
                   },
                   revisionId: '',
                   performerPN: '',
                   sequenceNo: 0,
                   performerContactType: ContactTypes.Undefined,
                   qCActionType: 0,
                   previousActionType: 0,
                   creatorComment:'',
                   parentId:''

        };

    }

}
