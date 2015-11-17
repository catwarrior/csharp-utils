using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace eToro.CircuitBreaker
{
    public class ImMemoryCircuitBreakerStateStore : ICircuitBreakerStateStore, IDisposable
    {
        private int _failureCounter = 0;
        private int _successCounter = 0;
        
        private readonly Random _randomizer = new Random();
        private readonly Timer _timer = new Timer();

        private readonly int _numberOfFailuresRequiredToTransitionToOpenState;
        private readonly int _numberOfSuccessesRequiredToTransitionToClosedState;

        public ImMemoryCircuitBreakerStateStore(int numberOfFailuresRequiredToTransitionToOpenState = 3, int numberOfSuccessesRequiredToTransitionToClosedState = 3, double changeToHalfOpenStateTimeInMilliseconds = 5000)
        {
            _numberOfFailuresRequiredToTransitionToOpenState = numberOfFailuresRequiredToTransitionToOpenState;
            _numberOfSuccessesRequiredToTransitionToClosedState = numberOfSuccessesRequiredToTransitionToClosedState;
            _timer.Interval = changeToHalfOpenStateTimeInMilliseconds;
            _timer.Enabled = false;
            _timer.Elapsed += (sender, args) => HalfOpen();
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

            _failureCounter++;

            if (FailureRequiresTransitionToOpenState)
            {
                State = CircuitBreakerState.Open;
                _timer.Enabled = true;
            }
        }

        public void Success()
        {
            if (IsOpen)
            {
                _successCounter++;
                if (_successCounter > _numberOfSuccessesRequiredToTransitionToClosedState)
                {
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
            _timer.Enabled = false;
            State = CircuitBreakerState.Closed;
            _failureCounter = 0;
            _successCounter = 0;
        }

        private bool FailureRequiresTransitionToOpenState
        {
            get
            {

                if (State == CircuitBreakerState.HalfOpen)
                    return true;

                if (_failureCounter >= _numberOfFailuresRequiredToTransitionToOpenState)
                    return true;
                
                return false;
            }
        }


  
    }
}
