using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Akeraiotitasoft.ExodusContainer.Web.Tests.Mock.Concrete
{
    using System.ComponentModel;
    using System.IO;
    using System.Reflection;

    using System.Runtime.Remoting.Messaging;
    using System.Web;
    public class MockWebServer
    {
        public static HttpContext GetHttpContext()
        {
            HttpRequest request = new HttpRequest("test", "http://myurl", "myParameters=1");
            MemoryStream memoryStream = new MemoryStream();
            TextWriter textWriter = new StreamWriter(memoryStream);
            HttpResponse response = new HttpResponse(textWriter);
            HttpContext context = new HttpContext(request, response);
            CallContext.HostContext = context;
            context.ApplicationInstance = new HttpApplication();

            return context;
        }

        public static void CallApplicationEndRequest(HttpContext context)
        {
            CallContext.HostContext = context;
            //var eventInfo = context.ApplicationInstance.GetType().GetEvent("EndRequest", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            EventHandlerList list = (EventHandlerList)context.ApplicationInstance.GetType().GetField("_events", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(context.ApplicationInstance);
            object eventEndRequest = context.ApplicationInstance.GetType().GetField("EventEndRequest", BindingFlags.Static | BindingFlags.NonPublic).GetValue(context.ApplicationInstance);
            var eventDelegate = (MulticastDelegate)list[eventEndRequest];
            var eventArgs = EventArgs.Empty;
            if (eventDelegate != null)
            {
                foreach (var handler in eventDelegate.GetInvocationList())
                {
                    handler.Method.Invoke(handler.Target, new object[] { context.ApplicationInstance, eventArgs });
                }
            }
        }
    }
}
