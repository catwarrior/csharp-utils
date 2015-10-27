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
    public class ValueObjectShould
    {
        public class DummyValueObject : ValueObject<DummyValueObject> 
        {
            public DummyValueObject(string firstName, string lastName)
            {
                FirstName = firstName;
                LastName = lastName;
            }

            public string FirstName { get; private set; }

            public string LastName { get; private set; }

            protected override IEnumerable<object> GetAttributesToIncludeInEqualityCheck()
            {
                return new List<string> { FirstName, LastName };
            }
        }


        private DummyValueObject _valueObject;
        private DummyValueObject _sameValueObject;
        private DummyValueObject _anotherValueObject;

        [SetUp]
        public void SetUp()
        {
            _valueObject = new DummyValueObject("John", "Doe");
            _sameValueObject = new DummyValueObject("John", "Doe");
            _anotherValueObject = new DummyValueObject("Jane", "Doe");
        }

        [Test]
        public void ReturnForEquals()
        {
            _valueObject.Equals((object)_sameValueObject).ShouldBe(true);
            _valueObject.Equals(_sameValueObject).ShouldBe(true);
            _valueObject.Equals((object)_anotherValueObject).ShouldBe(false);
            _valueObject.Equals(_anotherValueObject).ShouldBe(false);
        }

        [Test]
        public void ReturnForEqualsOperator()
        {
            (_valueObject == _sameValueObject).ShouldBe(true);
            (_valueObject == _anotherValueObject).ShouldBe(false);
        }

        [Test]
        public void ReturnForNotEqualsOperator()
        {
            (_valueObject != _sameValueObject).ShouldBe(false);
            (_valueObject != _anotherValueObject).ShouldBe(true);
        }

        [Test]
        public void ReturnGetHashcode()
        {
            _valueObject.GetHashCode().ShouldBeGreaterThan(0);
        }
    }
}
