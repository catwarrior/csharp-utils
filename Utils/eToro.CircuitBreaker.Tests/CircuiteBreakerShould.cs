using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace eToro.CircuitBreaker.Tests
{
    [TestFixture]
    public class CircuiteBreakerShould
    {
        private Mock<ICircuitBreakerStateStore> _mStateStore;
        private Action _action;
        private bool _actionCalled;
        private Func<string> _func;
        private bool _funcCalled;

        private ICircuiteBreaker CreateCircuiteBreaker()
        {
            return new CircuiteBreaker(_mStateStore.Object);
        }

        [SetUp]
        public void SetUp()
        {
            _mStateStore = new Mock<ICircuitBreakerStateStore>();
            _action = () => { _actionCalled = true; };
            _actionCalled = false;
            _func = () =>
            {
                _funcCalled = true;
                return "Gr8";
            };
            _funcCalled = false;
        }

        [Test]
        public void ThrowForExcecuteActionGivenStateIsOpen()
        {
            // Given
            var breaker = CreateCircuiteBreaker();
            _mStateStore.SetupGet(x => x.IsOpen).Returns(true);
            _mStateStore.SetupGet(x => x.LastException).Returns(new Exception());
            
            // When
            var actionResult = Should.Throw<CircuitBreakerOpenException>(() => breaker.ExcecuteAction(_action));
            var funcResult = Should.Throw<CircuitBreakerOpenException>(() => breaker.ExcecuteAction(_func));

            // Then
            _actionCalled.ShouldBe(false);
            _funcCalled.ShouldBe(false);
        }

        [Test]
        public void ThrowForExcecuteActionGivenExecutionIsNotAllowed()
        {
            // Given
            var breaker = CreateCircuiteBreaker();
            _mStateStore.SetupGet(x => x.IsOpen).Returns(false);
            _mStateStore.SetupGet(x => x.LastException).Returns(new Exception());
            _mStateStore.SetupGet(x => x.IsExecutionAllowed).Returns(false);

            // When
            var actionResult = Should.Throw<CircuitBreakerOpenException>(() => breaker.ExcecuteAction(_action));
            var funcResult = Should.Throw<CircuitBreakerOpenException>(() => breaker.ExcecuteAction(_func));

            // Then
            _actionCalled.ShouldBe(false);
            _funcCalled.ShouldBe(false);
        }

        [Test]
        public void ThrowForExcecuteActionGivenActionThrows()
        {
            // Given
            var breaker = CreateCircuiteBreaker();
            _mStateStore.SetupGet(x => x.IsOpen).Returns(false);
            _mStateStore.SetupGet(x => x.LastException).Returns(new Exception());
            _mStateStore.SetupGet(x => x.IsExecutionAllowed).Returns(true);

            // When
            var actionResult = Should.Throw<Exception>(() => breaker.ExcecuteAction(() => {throw new Exception();}));
            var funcResult = Should.Throw<Exception>(() => breaker.ExcecuteAction(() => { throw new Exception(); }));

            // Then
            _mStateStore.Verify(x => x.Error(It.IsAny<Exception>()));
        }


        [Test]
        public void ExcecuteAction()
        {
            // Given
            var breaker = CreateCircuiteBreaker();
            _mStateStore.SetupGet(x => x.IsOpen).Returns(false);
            _mStateStore.SetupGet(x => x.LastException).Returns(new Exception());
            _mStateStore.SetupGet(x => x.IsExecutionAllowed).Returns(true);

            // When
            breaker.ExcecuteAction(_action);
            var funcResult = breaker.ExcecuteAction(_func);

            // Then
            _actionCalled.ShouldBe(true);
            _funcCalled.ShouldBe(true);
            funcResult.ShouldNotBeEmpty();
        }
    }
}
