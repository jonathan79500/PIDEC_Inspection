namespace Inspection.Infrastructure.EDMS.Entities;

public class edmsUser
{
    public int Use_Id { get; set;}
    public string SAMAccountName { get; set;}
    public bool IsActive { get; set; }
    public bool IsVendor { get; set; }
    public bool IsOwner { get; set; }
    public bool IsPIDEC { get; set; }
    public bool IsMC { get; set; }
}
