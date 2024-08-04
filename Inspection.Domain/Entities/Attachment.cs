using Inspection.Domain.Entities.Common;
using System;

namespace Inspection.Domain.Model
{
    public sealed class Attachment:BaseDomainEntity<Guid>
    {
        public Attachment(Guid id):base(id) { }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[] Content { get; set; }

    }
}
