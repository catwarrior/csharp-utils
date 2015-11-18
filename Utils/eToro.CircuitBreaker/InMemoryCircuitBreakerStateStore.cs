using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;

namespace eToro.CircuitBreaker
{
    public class InMemoryCircuitBreakerStateStore : ICircuitBreakerStateStore, IDisposable
    {
        private int _failureCounter = 0;
        private int _successCounter = 0;
        
        private readonly Random _randomizer = new Random();
        private readonly Timer _timer = new Timer();
        private readonly Stopwatch _lastResetStopwatch = new Stopwatch();

        private readonly int _numberOfFailuresRequiredToTransitionToOpenState;
        private readonly int _numberOfSuccessesRequiredToTransitionToClosedState;
        private readonly long _recoveryTimeInMilliseconds;

        public InMemoryCircuitBreakerStateStore(int numberOfFailuresRequiredToTransitionToOpenState = 50, int numberOfSuccessesRequiredToTransitionToClosedState = 5, long recoveryTimeInMilliseconds = 5000)
        {
            _numberOfFailuresRequiredToTransitionToOpenState = numberOfFailuresRequiredToTransitionToOpenState;
            _numberOfSuccessesRequiredToTransitionToClosedState = numberOfSuccessesRequiredToTransitionToClosedState;
            _recoveryTimeInMilliseconds = recoveryTimeInMilliseconds;
            _timer.Interval = recoveryTimeInMilliseconds;
            
            _timer.Elapsed += (sender, args) => HalfOpen();
            _timer.Start();
            _lastResetStopwatch.Start();
        }

        public CircuitBreakerState State { get; private set; }

        public bool IsOpen
        {
            get { return State == CircuitBreakerState.Open; }
        }

        public Exception LastException { get; private set; }

        public void Error(Exception exception)
        {
            LastException = exception;

            var localFailureCounter = Interlocked.Increment(ref _failureCounter);

            if (FailureRequiresTransitionToOpenState(localFailureCounter))
            {
                State = CircuitBreakerState.Open;
            }
        }

        public void Success()
        {
            if (IsHalfOpen)
            {
                var localSuccessCounter = Interlocked.Increment(ref _successCounter);
                if (localSuccessCounter >= _numberOfSuccessesRequiredToTransitionToClosedState)
                {
                    Reset();
                }
                return;
            }
            
            if (IsClosed)
            {
                if (_lastResetStopwatch.ElapsedMilliseconds > _recoveryTimeInMilliseconds)
                {
                    // The IsClosed state is stable enough.
                    // Reset will prevent sporadically occurring failures to creep up, 
                    // and then result in opening the circuit. 
                    Reset();
                }
            }
        }

        public bool IsExecutionAllowed
        {
            get
            {
                if (State == CircuitBreakerState.Closed)
                    return true;
                
                if (State == CircuitBreakerState.Open)
                    return false;

                var randomValue = _randomizer.Next(1, 100);
                if (randomValue >= 66) 
                    return true;
                
                return false;
            }
        }

        public void Dispose()
        {
            _timer.Dispose();
        }

        private void HalfOpen()
        {
            if (State == CircuitBreakerState.Open)
            {
                State = CircuitBreakerState.HalfOpen;
            }
        }

        private void Reset()
        {
            State = CircuitBreakerState.Closed;
            Interlocked.Exchange(ref _failureCounter, 0);
            Interlocked.Exchange(ref _successCounter, 0);
            _lastResetStopwatch.Restart();
        }

        private bool FailureRequiresTransitionToOpenState(int localFailureCounter)
        {
            if (State == CircuitBreakerState.HalfOpen)
                return true;

            if (localFailureCounter >= _numberOfFailuresRequiredToTransitionToOpenState)
                return true;
                
            return false;
        }

        private bool IsHalfOpen
        {
            get { return State == CircuitBreakerState.HalfOpen; }
        }

        private bool IsClosed
        {
            get { return State == CircuitBreakerState.Closed; }
        }
    }
}
