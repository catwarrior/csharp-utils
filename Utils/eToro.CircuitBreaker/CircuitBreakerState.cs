using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eToro.CircuitBreaker
{
    public enum CircuitBreakerState
    {
        Closed,
        HalfOpen,
        Open
    }
}
