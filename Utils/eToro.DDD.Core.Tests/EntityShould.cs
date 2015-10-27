using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;

namespace eToro.DDD.Core.Tests
{

    [TestFixture]
    public class EntityShould
    {
        private class DummyEntity : Entity<Guid>
        {
            public DummyEntity(Guid id)
            {
                this.Id = id;
            }
        }


        private DummyEntity _entity;
        private Guid _id;

        [SetUp]
        public void SetUp()
        {
            _id = Guid.NewGuid();
            _entity = new DummyEntity(_id);
        }

        [Test]
        public void ReturnId()
        {
            _entity.Id.ShouldBe(_id);
        }
    }
}
