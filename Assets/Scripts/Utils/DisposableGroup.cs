using System;
using System.Collections.Generic;

namespace Kraft.Utils
{
    public class DisposableGroup : IDisposable
    {
        readonly List<IDisposable> m_Disposables = new List<IDisposable>();

        public void Dispose()
        {
            m_Disposables.ForEach(x => x.Dispose());
            m_Disposables.Clear();
        }

        public DisposableGroup Add(IDisposable disposable)
        {
            m_Disposables.Add(disposable);
            return this;
        }
    }
}
