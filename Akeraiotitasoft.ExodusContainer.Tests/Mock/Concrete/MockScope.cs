using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Akeraiotitasoft.ExodusContainer.Tests.Mock.Concrete
{
    public class MockScope
    {
        public static MockScope Current { get; set; }

        public static void CreateScope()
        {
            Current = new MockScope();
        }
    }
}
