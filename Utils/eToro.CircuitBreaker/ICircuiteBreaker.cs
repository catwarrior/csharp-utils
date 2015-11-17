using System;

namespace eToro.CircuitBreaker
{
    public interface ICircuiteBreaker
    {
        void ExcecuteAction(Action action);

        TResult ExcecuteAction<TResult>(Func<TResult> action);
    }
}