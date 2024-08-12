using Inspection.Domain.Entities.Common;

using System;


namespace Inspection.Domain.Entities
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
            Id = id;
        }

        public static Order Create(Guid id,string orderName,string titel,string number,Project project)
        {
            var order = new Order(id)
            {
                Name = orderName,
                Title = orderName,    
                No = orderName,
                Project = project
            };
            return order;
        }

        public static Order CreateEmpty()
        {
            var EmptyOrder = new Order(Guid.NewGuid()) { 
            
            };
            return EmptyOrder;
        }
    }


}
