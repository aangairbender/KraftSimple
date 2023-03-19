using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Assertions;

namespace Kraft.Utils
{
    public class MessageChannel<T> : IMessageChannel<T>
    {
        readonly List<Action<T>> m_MessageHandlers = new List<Action<T>>();

        /// This dictionary of handlers to be either added or removed is used to prevent problems from immediate
        /// modification of the list of subscribers. It could happen if one decides to unsubscribe in a message handler
        /// etc. A true value means this handler should be added, and a false one means it should be removed
        readonly Dictionary<Action<T>, bool> m_PendingHandlers = new Dictionary<Action<T>, bool>();

        public bool IsDisposed { get; private set; } = false;

        public virtual void Dispose()
        {
            if (IsDisposed) return;

            IsDisposed = true;
            m_MessageHandlers.Clear();
            m_PendingHandlers.Clear();
        }

        public virtual void Publish(T message)
        {
            foreach (var (handler, shouldAdd) in m_PendingHandlers)
            {
                if (shouldAdd)
                {
                    m_MessageHandlers.Add(handler);
                }
                else
                {
                    m_MessageHandlers.Remove(handler);
                }
            }
            m_PendingHandlers.Clear();

            foreach (var handler in m_MessageHandlers)
            {
                handler?.Invoke(message);
            }
        }

        public virtual IDisposable Subscribe(Action<T> handler)
        {
            Assert.IsFalse(IsSubscribed(handler), "Attempting to subscribe with the same handler more than once");

            if (m_PendingHandlers.TryGetValue(handler, out var shouldAdd))
            {
                if (!shouldAdd)
                {
                    m_PendingHandlers.Remove(handler);
                }
            }
            else
            {
                m_PendingHandlers[handler] = true;
            }

            return new DisposableSubscription<T>(this, handler);
        }

        public virtual void Unsubscribe(Action<T> handler)
        {
            if (!IsSubscribed(handler)) return;

            if (m_PendingHandlers.TryGetValue(handler, out var shouldAdd))
            {
                if (shouldAdd)
                {
                    m_PendingHandlers.Remove(handler);
                }
            }
            else
            {
                m_PendingHandlers[handler] = false;
            }
        }

        private bool IsSubscribed(Action<T> handler)
        {
            var isPending = m_PendingHandlers.TryGetValue(handler, out var shouldAdd);
            var isPendingRemoval = isPending && !shouldAdd;
            var isPendingAddition = isPending && shouldAdd;
            return (m_MessageHandlers.Contains(handler) && !isPendingRemoval) || isPendingAddition;
        }
    }
}
