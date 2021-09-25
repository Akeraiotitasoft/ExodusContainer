using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Akeraiotitasoft.ExodusContainer
{
    public interface IExodusContainer : IDisposable
    {
        object Resolve(Type type);

        object Resolve(Type type, string name);

        void Register(Type from, Type to, IScopeGovernor scopeGovernor);

        void Register(Type from, IScopeGovernor scopeGovernor, Func<IExodusContainer, Type, string, object> createFunc);

        void Register(Type from, string name, IScopeGovernor scopeGovernor, Func<IExodusContainer, Type, string, object> createFunc);

        IExodusContainerScope BeginScope();

        void RemoveScopeKey(object scope);
    }
}
