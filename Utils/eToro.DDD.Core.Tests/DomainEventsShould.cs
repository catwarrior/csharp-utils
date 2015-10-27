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
    public class DomainEventsShould
    {
        public interface IRepository
        {
            void Save();
        }

        private class DummyDomainEvent : DomainEvent
        {
            
        }

        private DomainEvents _domainEvents;
        private Mock<IRepository> _mRepo;

        [SetUp]
        public void SetUp()
        {
            _domainEvents = new DomainEvents();
            _mRepo = new Mock<IRepository>();
        }

        [Test]
        public void RegisterAndRaiseUntilDisposed()
        {
            var result = _domainEvents.Register((DummyDomainEvent domainEvent) => _mRepo.Object.Save());

            _domainEvents.Raise(new DummyDomainEvent());
            _domainEvents.Raise(new DummyDomainEvent());

            // Unregister callback
            result.Dispose();
            
            // Should not call repo.Save()
            _domainEvents.Raise(new DummyDomainEvent());

            _mRepo.Verify(x => x.Save(), Times.Exactly(2));
        }
    }
}
