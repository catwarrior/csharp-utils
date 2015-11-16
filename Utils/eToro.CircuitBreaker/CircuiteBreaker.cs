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
    public class CircuiteBreaker
    {
        private readonly ICircuitBreakerStateStore _stateStore = new ImMemoryCircuitBreakerStateStore();

        private readonly object _halfOpenSyncObject = new object();

        public TResult ExcecuteAction<TResult>(Func<TResult> action)
        {
            var result = default(TResult);
            
            if (_stateStore.IsOpen)
            {
                return result;
            }
            
            try
            {
                if (_stateStore.IsExecutionAllowed)
                {
                    result = action();
                    _stateStore.Success();
                }
                else
                {
                    throw new CircuitBreakerOpenException();    
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
            _stateStore.Trip(ex);
        }
    }
}
