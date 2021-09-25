using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Akeraiotitasoft.ExodusContainer
{
    public interface IExodusContainerScope : IDisposable
    {
        object Resolve(Type type);
        object Resolve(Type type, string name);
    }
}
