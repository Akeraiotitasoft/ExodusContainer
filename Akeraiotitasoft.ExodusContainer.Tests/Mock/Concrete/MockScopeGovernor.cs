using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Akeraiotitasoft.ExodusContainer.Tests.Mock.Concrete
{
    public class MockScopeGovernor : IScopeGovernor
    {
        public object GetScope(IExodusContainer exodusContainer, IExodusContainerScope exodusContainerScope)
        {
            return MockScope.Current;
        }
    }
}
