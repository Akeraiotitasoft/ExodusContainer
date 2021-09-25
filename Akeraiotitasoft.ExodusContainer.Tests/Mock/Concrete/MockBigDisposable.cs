using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Akeraiotitasoft.ExodusContainer.Tests.Mock.Concrete
{
    public class MockBigDisposable : IDisposable
    {
        private string _myBigString1;
        private string _myBigString2;
        private string _myBigString3;

        public MockBigDisposable()
        {
            _myBigString1 = new string('a', 64 * 1024 * 1024); // 64 MB
            Thread.Sleep(1000);
            _myBigString2 = new string('a', 32 * 1024 * 1024); // 32 MB
            Thread.Sleep(1000);
            _myBigString3 = new string('a', 128 * 1024 * 1024); // 128 MB
        }
        public int DisposeCount { get; private set; } = 0;

        public void Dispose()
        {
            _myBigString1 = null;
            _myBigString2 = null;
            _myBigString3 = null;
            DisposeCount++;
        }
    }
}
