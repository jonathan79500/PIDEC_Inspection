import { NewNotificationNumberQueryParams } from './../Services/inspection.service';
import { Circulation } from '../Models/Circulation.model';
import { Injectable } from "@angular/core";
import { Inspection } from "../Models/Inspection.model";
import { ContactTypes } from "../Models/ContactTypes.enum";
import { Revision } from "../Models/Revision.model";
import { InspectionWorkflowContext } from "./InspectionWorkflowContext.model";

@Injectable({
  providedIn: 'root'
})
export class WorkflowSearchHelper {


  findCurrentUserContext(
    inspections: Inspection[],
    currentUserId: string)
{
    for (const inspection of inspections) {

        for (const revision of inspection.revisions) {

            const circulation = revision.circulations

                .filter(c =>
                    c.performerContactPerson?.userId == currentUserId)

                .sort((a, b) => b.sequenceNo - a.sequenceNo)[0];

            if (circulation) {

                return {

                    inspection,

                    revision,

                     circulation

                };

            }
        }
    }

    return null;
}

findLastVendorCirculation(
    inspections: Inspection[])
    : {inspection:Inspection,revision:Revision,circulation:Circulation}|null
{

    for(const inspection of inspections){

        for(const revision of inspection.revisions){

            const vendor = revision.circulations

                .filter(c=>

                    c.performerContactType==ContactTypes.Vendor)

                .sort((a,b)=>

                    b.performedActionDateGregorian.getTime()

                    -

                    a.performedActionDateGregorian.getTime())[0];

            if(vendor){

                return{

                    inspection,

                    revision,

                    circulation:vendor

                };

            }

        }

    }

    return null;

}

findLastQCCirculation(
    inspections:Inspection[])
{

    for(const inspection of inspections){

        for(const revision of inspection.revisions){

            const qc = revision.circulations

                .filter(c=>

                    (c.performerContactType==ContactTypes.QC_Lead ||

                     c.performerContactType==ContactTypes.QC_Manager)

                    &&

                    c.performedActionDateGregorian.getFullYear()!=1)

                .sort((a,b)=>

                    b.performedActionDateGregorian.getTime()

                    -

                    a.performedActionDateGregorian.getTime())[0];

            if(qc){

                return{

                    inspection,

                    revision,

                    circulation:qc

                };

            }

        }

    }

    return null;

}

findLatestCirculationByPerformer(

    circulations:Circulation[],

    performerType:number,

    userId:string)
{

    return circulations

        .filter(c=>

            c.performerContactType==performerType

            &&

            c.performerContactPerson?.userId==userId)

        .sort((a,b)=>b.sequenceNo-a.sequenceNo)[0];

}

findChildren(

    parent:Circulation,

    circulations:Circulation[])
{

    return circulations

        .filter(c=>c.parentId==parent.id)

        .sort((a,b)=>b.sequenceNo-a.sequenceNo);

}

findInternalCorrespondences(

    circulations:Circulation[],

    performerType:number,

    userId:string)
{

    return circulations

        .filter(c=>

            c.performerContactType==performerType

            &&

            c.performerContactPerson?.userId==userId

            &&

            c.performerContactType!=ContactTypes.Vendor)

        .sort((a,b)=>b.sequenceNo-a.sequenceNo);

}

findLastAssignedCirculation(

    circulation:Circulation,

    circulations:Circulation[])
{

    const assigned = circulations

        .filter(c=>

            c.performerContactType==

                circulation.performerContactType

            &&

            c.performerContactPerson?.userId==

                circulation.performerContactPerson?.userId)

        .sort((a,b)=>b.sequenceNo-a.sequenceNo);

    return assigned[0] ?? circulation;

}

findLastUserCirculation(
    userId: string,
    inspections: Inspection[]
): {
    inspection: Inspection;
    revision: Revision;
    circulation: Circulation;
} | null {

    for (const inspection of inspections) {

        for (const revision of inspection.revisions) {

            const circulation = revision.circulations
                .filter(c => c.performerContactPerson?.userId === userId)
                .sort((a, b) => b.sequenceNo - a.sequenceNo)[0];

            if (circulation) {
                return {
                    inspection,
                    revision,
                    circulation
                };
            }
        }
    }

    return null;
}

findVendorAttachments(
    inspections:Inspection[])
{

    const vendor=this.findLastVendorCirculation(inspections);

    return structuredClone(
        vendor?.circulation.attachments ?? []);
}

CalculateShownNotificationNumber( notificationNumber: string):string{
  let [mainNo, rev] = notificationNumber != null ? notificationNumber.split('.') : ["", ""];
  return mainNo+'.'+rev;
}

calculateShownRevisionNumber(
    revisionNumber: string,
    increase: boolean
): string {

  if (typeof revisionNumber === 'string') {
  let [mainNo, revStr] =revisionNumber!=""? revisionNumber.split('.') : ["", ""];

    const rev = Number(revStr);

    return increase
        ? (rev + 1).toString().padStart(2, '0')
        : rev.toString().padStart(2, '0');
  }else
  {


    return increase
        ? (revisionNumber + 1).toString().padStart(2, '0')
        : (revisionNumber+0).toString().padStart(2, '0');
  }
}

// findChildrenByParent(
//     parentId: string,
//     circulations: Circulation[]
// ): Circulation[] {
//     return circulations
//         .filter(c => c.parentId === parentId)
//         .sort((a, b) => b.sequenceNo - a.sequenceNo);
// }

// findChildrenByUser(
//     userId: string,
//     circulations: Circulation[]
// ): Circulation[] {
//     return circulations
//         .filter(c =>
//             c.performerContactPerson?.userId === userId &&
//             c.sequenceNo! > 1)
//         .sort((a, b) => b.sequenceNo - a.sequenceNo);
// }

findPreviousVendorCirculation(
    context: InspectionWorkflowContext
): {
    inspection: Inspection;
    revision: Revision;
    circulation: Circulation;
} | null {

    const createdById =
        context.currentRevision.createdByCirculationId;

    if (!createdById)
        return null;

    for (const inspection of context.inspections) {

        for (const revision of inspection.revisions) {

            if (
                revision.submittedDate >= context.currentRevision.submittedDate
            ) {
                continue;
            }

            const circulation = revision.circulations.find(
                c => c.id === createdById
            );

            if (circulation) {

                return {
                    inspection,
                    revision,
                    circulation
                };

            }

        }

    }

    return null;
}

buildViewModel(

    requestCirculations:Circulation[],

    allCirculations:Circulation[])
{

    return requestCirculations.map(c=>({

        original:c,

        current:this.findLastAssignedCirculation(

            c,

            allCirculations)

    }));

}


}
