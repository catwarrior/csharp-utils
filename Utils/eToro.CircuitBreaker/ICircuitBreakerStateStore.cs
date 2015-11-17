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

        bool IsOpen { get; }

        bool IsExecutionAllowed { get; }

        Exception LastException { get; }

        void Error(Exception exception);

        void Success();
    }
}
