using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eToro.CircuitBreaker
{
    public interface ICircuitBreakerStateStore
    {
        CircuitBreakerState State { get; }

        Exception LastException { get; }

        DateTime LastStateChangedDateUtc { get; }

        bool IsOpen { get; }

        void Trip(Exception ex);

        void Reset();

        void HalfOpen();
    }
}
