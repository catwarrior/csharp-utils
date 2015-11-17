using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace eToro.CircuitBreaker
{
    public class CircuiteBreaker : ICircuiteBreaker
    {
        private readonly ICircuitBreakerStateStore _stateStore;

        public CircuiteBreaker(ICircuitBreakerStateStore stateStore = null)
        {
            _stateStore = stateStore;
        }

        public CircuiteBreaker() : this(new InMemoryCircuitBreakerStateStore())
        {
            
        }

        public void ExcecuteAction(Action action)
        {
            if (_stateStore.IsOpen)
            {
                throw new CircuitBreakerOpenException(_stateStore.LastException);
            }

            try
            {
                if (_stateStore.IsExecutionAllowed)
                {
                    action();
                    _stateStore.Success();
                }
                else
                {
                    throw new CircuitBreakerOpenException(_stateStore.LastException);
                }
            }
            catch (Exception ex)
            {
                TrackException(ex);
                throw;
            }
        }

        public TResult ExcecuteAction<TResult>(Func<TResult> action)
        {
            if (_stateStore.IsOpen)
            {
                throw new CircuitBreakerOpenException(_stateStore.LastException);  
            }
            
            var result = default(TResult);

            try
            {
                if (_stateStore.IsExecutionAllowed)
                {
                    result = action();
                    _stateStore.Success();
                }
                else
                {
                    throw new CircuitBreakerOpenException(_stateStore.LastException);    
                }
            }
            catch (Exception ex)
            {
                TrackException(ex);
                throw;
            }
            
            return result;
        }

        private void TrackException(Exception ex)
        {
            _stateStore.Error(ex);
        }
    }
}
