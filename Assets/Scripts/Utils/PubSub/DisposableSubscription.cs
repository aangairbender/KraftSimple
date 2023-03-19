using System;

namespace Kraft.Utils
{
    public class DisposableSubscription<T> : IDisposable
    {
        Action<T> m_Handler;
        bool m_IsDisposed;
        IMessageChannel<T> m_MessageChannel;

        public DisposableSubscription(IMessageChannel<T> messageChannel, Action<T> handler)
        {
            m_MessageChannel = messageChannel;
            m_Handler = handler;
            m_IsDisposed = false;
        }

        public void Dispose()
        {
            if (m_IsDisposed) return;

            m_IsDisposed = true;
            if (!m_MessageChannel.IsDisposed)
            {
                m_MessageChannel.Unsubscribe(m_Handler);
            }

            m_Handler = null;
            m_MessageChannel = null;
        }
    }
}
