using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace eToro.DDD.Core.Tests
{

    [TestFixture]
    public class AggregateShould
    {
        public interface IRepository
        {
            void Save();
        }

        private class DummySavedEvent : DomainEvent
        {}

        private class DummyEntity : Entity<Guid>
        {
            public DummyEntity(Guid id)
            {
                this.Id = id;
            }

            public void Save()
            {
                if (_domainEvents != null)
                {
                    _domainEvents.Raise(new DummySavedEvent());
                }
            }
        }

        private class DummyAggregate : Entity<Guid>
        {
            public DummyAggregate(Guid id)
            {
                this.Id = id;
                DummyEntity = new DummyEntity(Guid.NewGuid());
            }

            private DummyEntity DummyEntity { get; set; }

            public override void With(DomainEvents domainEvents)
            {
                DummyEntity.With(domainEvents);
                base.With(domainEvents);
            }

            public void Execute()
            {
                DummyEntity.With(_domainEvents);
                DummyEntity.Save();
            }
        }

        private DummyAggregate _aggregate;
        private Mock<IRepository> _mRepo;

        [SetUp]
        public void SetUp()
        {
            _mRepo = new Mock<IRepository>();
            _aggregate = new DummyAggregate(Guid.NewGuid());
        }

        [Test]
        public void RegisterDomainEvent()
        {
            // Application Layer
            var domainEvents = new DomainEvents();
            using (domainEvents.Register((DummySavedEvent @event) => _mRepo.Object.Save()))
            {
                // Register domain events
                _aggregate.With(domainEvents);

                // Calling domain               
                _aggregate.Execute();
            }

            _mRepo.Verify(x => x.Save());
        }
    }
}
