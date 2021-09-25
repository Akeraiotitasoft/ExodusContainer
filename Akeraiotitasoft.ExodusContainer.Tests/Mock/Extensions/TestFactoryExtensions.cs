using Akeraiotitasoft.ExodusContainer.Tests.Mock.Abstraction;
using Akeraiotitasoft.ExodusContainer.Tests.Mock.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Akeraiotitasoft.ExodusContainer.Tests.Mock.Extensions
{
    public static class TestFactoryExtensions
    {
        public static ITestInterface<T> CreateTestInterface<T>(this ITestFactory testFactory)
        {
            if (testFactory == null)
            {
                throw new ArgumentNullException(nameof(testFactory));
            }

            return new MockFactoryChild<T>(testFactory);
        }
    }
}
