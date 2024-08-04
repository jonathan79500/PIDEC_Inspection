using Inspection.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.Common
{
    public abstract class AggregateRoot<TId>:BaseDomainEntity<TId>
        where TId :notnull
    {

        private readonly List<DomainEvent> _events=new();

        protected AggregateRoot(TId id) :base(id) {
        }

        protected AggregateRoot():base() { }

        public IReadOnlyCollection<DomainEvent> Events => _events.ToList();

        public void RaiseEvent(DomainEvent @event) {
            _events.Add(@event);        
        }
    }
}
