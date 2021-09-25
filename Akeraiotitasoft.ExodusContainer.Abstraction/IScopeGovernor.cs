using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Akeraiotitasoft.ExodusContainer
{
    public interface IScopeGovernor
    {
        object GetScope(IExodusContainer exodusContainer, IExodusContainerScope exodusContainerScope);
    }
}
