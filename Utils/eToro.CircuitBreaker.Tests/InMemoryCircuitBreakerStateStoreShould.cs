using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;

namespace eToro.CircuitBreaker.Tests
{
    [TestFixture]
    public class InMemoryCircuitBreakerStateStoreShould
    {
        private const int NumberOfFailuresRequiredToTransitionToOpenState = 2;
        private const int NumberOfSuccessesRequiredToTransitionToClosedState = 2;
        private const int RecoveryTimeInMilliseconds = 100;

        private ICircuitBreakerStateStore CreateStateStore()
        {
            return new InMemoryCircuitBreakerStateStore(NumberOfFailuresRequiredToTransitionToOpenState, NumberOfSuccessesRequiredToTransitionToClosedState, RecoveryTimeInMilliseconds);
        }

        [Test]
        public void ReturnState()
        {
            // Given
            var stateStore = CreateStateStore();

            // When
            var result = stateStore.State;

            // Then
            result.ShouldBe(CircuitBreakerState.Closed);
        }

        [Test]
        public void ReturnIsOpen()
        {
            // Given
            var stateStore = CreateStateStore();

            // When
            var result = stateStore.IsOpen;

            // Then
            result.ShouldBe(false);
        }

        [Test]
        public void HandleErrorGivenFailureCounterHasNotExceeeded()
        {
            // Given
            var stateStore = CreateStateStore();
            var ex = new Exception("OMG!");
            
            // When
            stateStore.Error(ex);

            // Then
            stateStore.State.ShouldBe(CircuitBreakerState.Closed);
        }

        [Test]
        public void HandleErrorGivenFailureCounterHasExceeeded()
        {
            // Given
            var stateStore = CreateStateStore();
            var ex = new Exception("OMG!");

            // When
            stateStore.Error(ex);
            stateStore.Error(ex);

            // Then
            stateStore.State.ShouldBe(CircuitBreakerState.Open);
        }

        [Test]
        public void ChangeToHalfOpenStateAfterSomeTime()
        {
            // Given
            var stateStore = CreateStateStore();
            var ex = new Exception("OMG!");
            stateStore.Error(ex);
            stateStore.Error(ex);

            // When
            Thread.Sleep(RecoveryTimeInMilliseconds * 2);

            // Then
            stateStore.State.ShouldBe(CircuitBreakerState.HalfOpen);
        }

        [Test]
        public void ChangeToClosedStateAfterEnoughSuccessesHaveOccurred()
        {
            // Given
            var stateStore = CreateStateStore();
            var ex = new Exception("OMG!");
            stateStore.Error(ex);
            stateStore.Error(ex);
            stateStore.Error(ex);
            Thread.Sleep(RecoveryTimeInMilliseconds * 2);

            // When
            stateStore.Success();
            stateStore.Success();

            // Then
            stateStore.State.ShouldBe(CircuitBreakerState.Closed);
        }

        [Test]
        public void NotChangeToClosedStateAfterNotEnoughSuccessesHaveOccurred()
        {
            // Given
            var stateStore = CreateStateStore();
            var ex = new Exception("OMG!");
            stateStore.Error(ex);
            stateStore.Error(ex);
            stateStore.Error(ex);
            Thread.Sleep(RecoveryTimeInMilliseconds * 2);

            // When
            stateStore.Success();

            // Then
            stateStore.State.ShouldBe(CircuitBreakerState.HalfOpen);
        }

        [Test]
        public void ResetCountersGivenIsClosedStateIsStableEnough()
        {
            // Given
            var stateStore = CreateStateStore();
            stateStore.Error(new Exception());
            Thread.Sleep(RecoveryTimeInMilliseconds * 2);

            // When
            stateStore.Success();

            // Then
            stateStore.State.ShouldBe(CircuitBreakerState.Closed);
            stateStore.Error(new Exception());
            stateStore.State.ShouldBe(CircuitBreakerState.Closed);
        }

        [Test]
        public void ReturnTrueForIsExecutionAllowedGivenClosedState()
        {
            // Given
            var stateStore = CreateStateStore();

            // When
            var result = stateStore.IsExecutionAllowed;

            // Then
            result.ShouldBe(true);
        }

        [Test]
        public void ReturnFalseIsExecutionAllowedGivenOpenState()
        {
            // Given
            var stateStore = CreateStateStore();
            stateStore.Error(new Exception());
            stateStore.Error(new Exception());

            // When
            var result = stateStore.IsExecutionAllowed;

            // Then
            result.ShouldBe(false);
        }

        [Test]
        public void ReturnIsExecutionAllowedGivenOpenState()
        {
            // Given
            var stateStore = CreateStateStore();
            stateStore.Error(new Exception());
            stateStore.Error(new Exception());
            Thread.Sleep(RecoveryTimeInMilliseconds * 2);

            // When
            var results = new List<bool>();
            for (var i = 0; i < 100; i++)
            {
                results.Add(stateStore.IsExecutionAllowed);
            }

            // Then
            results.ShouldContain(true);
            results.ShouldContain(false);
        }

        [Test]
        public void ReturnLastException()
        {
            // Given
            var stateStore = CreateStateStore();
            var ex = new Exception();

            // When
            stateStore.Error(ex);

            // Then
            stateStore.LastException.ShouldBe(ex);
        }

        [Test]
        public void Dispose()
        {
            // Given
            var stateStore = CreateStateStore();

            // When &  Then
            (stateStore as IDisposable).Dispose();
        }
    }
}
