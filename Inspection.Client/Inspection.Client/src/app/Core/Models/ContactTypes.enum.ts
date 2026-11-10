
export enum ContactTypes
{

    Undefined = 0,
    Owner = 1,
    Vendor = 2,
    MC = 3,
    QC_Manager = 4,
    Supplier = 5,
    SubSupplier = 6,
    QC_Lead = 7,
    PIDEC_Inspector = 8,
    ThirdPart_Inspector = 9,
    Department_Lead =10 ,
    Department_Originator =11
}
export const CONTACT_TYPES: { value: ContactTypes; label: string }[] = [
  { value: ContactTypes.Owner, label: 'Owner' },
  { value: ContactTypes.Vendor, label: 'Vendor' },
  { value: ContactTypes.MC, label: 'MC' },
  { value: ContactTypes.QC_Manager, label: 'PIDEC' },
  { value: ContactTypes.Supplier, label: 'Supplier' },
  { value: ContactTypes.SubSupplier, label: 'Sub-supplier' },
  { value: ContactTypes.QC_Lead, label: 'PIDEC CC' },
  { value: ContactTypes.PIDEC_Inspector, label: 'PIDEC inspector' },
  { value: ContactTypes.ThirdPart_Inspector, label: 'TP inspector' },
];
