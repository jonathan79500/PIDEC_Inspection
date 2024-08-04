using Inspection.Domain.Entities.Common;

using System;


namespace Inspection.Domain
{
    public sealed class Order:BaseDomainEntity<Guid>
    {

        public Project Project { get; private set; }
        public string No { get; private set; }
        public string Title { get; private set; }
        public string Name { get; private set; }

        public Order() { }

        public Order(Guid id):base(id)
        {
            
        }
        public static Order CreateEmpty()
        {
            var EmptyOrder = new Order(Guid.NewGuid()) { 
            
            };
            return EmptyOrder;
        }
    }


}
