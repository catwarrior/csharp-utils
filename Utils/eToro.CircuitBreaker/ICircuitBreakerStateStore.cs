﻿using System;
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

        bool IsExecutionAllowed { get; }

        void Trip(Exception ex);

        void Success();

        void Reset();

        void HalfOpen();
    }
}
