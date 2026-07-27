using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.Inspector;
using Inspection.Domain.Entities.Projects;
using Inspection.Domain.Entities.ValueObjects;
using Inspection.Domain.Entities.Vendor;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;



namespace Inspection.Domain.Entities.Orders;

public sealed class Order : AggregateRoot<OrderId>
{
    
    public OrderId OrderId { get;  set; }
    private readonly List<VendorDocument>? _vendorDocuments = new();
    public Project? Project { get;  set; }
    public string? No { get;  set; }
    public string? Title { get;  set; }
    public string? Name { get;  set; }
    public int? Version { get;  set; }
    public string? EdmsId { get;  set; }
    public string? EquipmentTagNo { get;  set; }
    public string? ITP_NO { get;  set; }

    public List<OrderContact>? Contacts { get;  set; }

   

    public List<OrdersettingException>? ExceptionContacts { get; set; } = new();

    public List<OrderInspectionSubject>? Subjects { get;  set; } =new();

    public IReadOnlyList<VendorDocument>? VendorDocuments => _vendorDocuments;

    public Order() {
        Name = string.Empty;
        Version = 0;
        EquipmentTagNo = string.Empty;
        ITP_NO = string.Empty;
        Title = string.Empty;
        No = string.Empty;
    }

    public Order(OrderId id) : base(id)
    {
       this.OrderId = id;
    }

    public void SetProject(Project project)
    {
        Project = project;
    }


    public static Order Create(string orderName, string titel, string number, Project project, string edmsId)
    {
        var order = new Order(OrderId.CreateUnique())
        {
            Name = orderName,
            Title = orderName,
            No = orderName,
            Project = project,
            EdmsId = edmsId
        };
        return order;
    }


    public static Order CreateEmpty()
    {
        var EmptyOrder = new Order(OrderId.CreateUnique())
        {

        };
        return EmptyOrder;
    }

    public void AddVendorDocument(VendorDocument vendorDocument)
    {
        _vendorDocuments.Add(vendorDocument);
    }
}
