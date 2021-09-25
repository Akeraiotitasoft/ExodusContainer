using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Akeraiotitasoft.ExodusContainer.Web
{
    public class ExodusRequestScopeModule : IHttpModule
    {
        public void Dispose()
        {
        }

        public static IExodusContainer ExodusContainer { get; set; }

        public void Init(HttpApplication context)
        {
            context.EndRequest += Context_EndRequest;
        }

        private void Context_EndRequest(object sender, EventArgs e)
        {
            if (ExodusContainer != null)
            {
                ExodusContainer.RemoveScopeKey(HttpContext.Current);
            }
        }
    }
}
