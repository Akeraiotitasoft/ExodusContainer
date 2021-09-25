using System.Web;

namespace Akeraiotitasoft.ExodusContainer.Web.Governor
{
    public class PreRequestGovernor : IScopeGovernor
    {
        public object GetScope(IExodusContainer exodusContainer, IExodusContainerScope exodusContainerScope)
        {
            return HttpContext.Current;
        }
    }
}
