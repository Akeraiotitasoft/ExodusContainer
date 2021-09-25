using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Akeraiotitasoft.ExodusContainer.Tests.Mock.Concrete
{
    public class MockDisposable : IDisposable
    {
        public int DisposeCount { get; private set; } = 0;
        public void Dispose()
        {
            DisposeCount++;
        }
    }
}
