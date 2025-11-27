using Inspection.Domain.Entities.Contacts;
using Inspection.Domain.Entities.ValueObjects;
using System;

namespace Inspection.Domain.Entities.Orders;

public class OrderContact
{
    public OrderContact()
    {

    }
    public Guid? Id { get; set; }
    public OrderId? OrderId { get; set; }
    public Guid? ContactId { get; set; }
    public Order? Order { get; set; }
    public Contact? Contact { get; set; }
}
