using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eToro.DDD.Core
{
    public class DomainEvents
    {
        private readonly List<Delegate> _actions = new List<Delegate>();

        public IDisposable Register<T>(Action<T> callback) where T : DomainEvent
        {
            _actions.Add(callback);

            return new DomainEventRegistrationRemover(() => _actions.Remove(callback));
        }

        public void Raise<T>(T domainEvent) where T : DomainEvent
        {
            foreach (var action in _actions)
            {
                var typedAction = action as Action<T>;
                if (typedAction != null)
                {
                    typedAction(domainEvent);
                }
            }
        }

        private sealed class DomainEventRegistrationRemover : IDisposable
        {
            private readonly Action _callOnDispose;

            public DomainEventRegistrationRemover(Action toCall)
            {
                _callOnDispose = toCall;
            }

            public void Dispose()
            {
                _callOnDispose();
            }
        }

    }
}
