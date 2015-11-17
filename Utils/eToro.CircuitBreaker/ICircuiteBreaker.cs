using System;

namespace eToro.CircuitBreaker
{
    public interface ICircuiteBreaker
    {
        TResult ExcecuteAction<TResult>(Func<TResult> action);
    }
}