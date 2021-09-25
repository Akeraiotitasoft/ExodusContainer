using Akeraiotitasoft.ExodusContainer.Tests.Mock.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Akeraiotitasoft.ExodusContainer.Tests.Mock.Concrete
{
    public class MockFactoryChild<T> : ITestInterface<T>
    {
        private readonly ITestFactory _testFactory;

        public MockFactoryChild(ITestFactory testFactory)
        {
            if (testFactory == null)
            {
                throw new ArgumentNullException(nameof(testFactory));
            }
            _testFactory = testFactory;
        }
    }
}
