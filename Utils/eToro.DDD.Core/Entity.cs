using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eToro.DDD.Core
{
    public abstract class Entity<TId>
    {
        protected DomainEvents _domainEvents;

        public TId Id { get; protected set; }

        public virtual void With(DomainEvents domainEvents)
        {
            _domainEvents = domainEvents;
        }
    }
}
