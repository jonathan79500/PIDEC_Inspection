using System;

namespace Inspection.Domain.Entities.Common
{
    public abstract class BaseDomainEntity<TId>:IEquatable<BaseDomainEntity<TId>>
        where TId : notnull
    {
        //public TId Id { get;protected set; }
        //public DateTime CreatedDate { get; set; }
        //public DateTime ModifiedDate { get; set; }
        //public string CreatedBy { get; private set; } = string.Empty;
        //public string ModifiedBy { get; private set; } = string.Empty;

        
        public TId Id { get;  set; }
        public DateTime CreatedDate { get; set; } =DateTime.Now;
        public DateTime ModifiedDate { get; set; }= DateTime.Now.AddSeconds(5);
        public string CreatedBy { get; set; } ="Admin";
        public string ModifiedBy { get; set; } = string.Empty;


        protected BaseDomainEntity(TId id)
        {
            Id = id;
        }

        protected BaseDomainEntity()
        {
            
        }

        public override bool Equals(object obj)
        {
            if(obj == null) return false;
            if(obj.GetType() != GetType())
                return false;
            if(!(obj is BaseDomainEntity<TId> entity))
                return false;


            return obj is BaseDomainEntity<TId> basentity && Id.Equals(entity.Id);
        }

        public bool Equals(BaseDomainEntity<TId> other)
        {
            if (other == null) return false;
            if (other.GetType() != GetType())
                return false;
            if (!(other is BaseDomainEntity<TId> entity))
                return false;


            return Id.Equals(entity.Id);
        }

        public static bool operator ==(BaseDomainEntity<TId>? first, BaseDomainEntity<TId>? second)
        {
            if (ReferenceEquals(first, second))
                return true;
            if ((first is null && second is not null) || (first is not null && second is null))
                return false;
            return first.Equals(second);
        }

        public static bool operator !=(BaseDomainEntity<TId>? first, BaseDomainEntity<TId>? second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }



    //public record EntityId(Guid Value);

}
