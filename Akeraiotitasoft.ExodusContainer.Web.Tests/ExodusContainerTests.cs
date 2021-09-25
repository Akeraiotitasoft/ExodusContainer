using Akeraiotitasoft.ExodusContainer;
using Akeraiotitasoft.ExodusContainer.Governor;
using Akeraiotitasoft.ExodusContainer.Tests.Mock.Abstraction;
using Akeraiotitasoft.ExodusContainer.Tests.Mock.Concrete;
using Akeraiotitasoft.ExodusContainer.Tests.Mock.Extensions;
#if NETFRAMEWORK
using Akeraiotitasoft.ExodusContainer.Web;
using Akeraiotitasoft.ExodusContainer.Web.Governor;
using System.Web;
using System.Runtime.Remoting.Messaging;
using System.ComponentModel;
#endif
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Akeraiotitasoft.ExodusContainer.Web.Tests.Mock.Concrete;


#if LOGGING
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
#endif

namespace ExodusBuildingBlocks.Container.Web.Tests
{
    public class Tests
    {
        private IExodusContainer _container;

        [SetUp]
        public void Setup()
        {
#if LOGGING
            ILoggerFactory loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(new DebugLoggerProvider());
            var logger = loggerFactory.CreateLogger<ExodusContainer>();
            _container = new ExodusContainer(logger);
#else
            _container = new ExodusContainer();
#endif
        }

        [TearDown]
        public void TearDown()
        {
            _container.Dispose();
        }

        [Test]
        public void ContainerHasPerRequestScope()
        {
            ExodusRequestScopeModule.ExodusContainer = _container;
            _container.Register(typeof(IDisposable), typeof(MockDisposable), new PreRequestGovernor());
            HttpContext httpContext1 = MockWebServer.GetHttpContext();
            new ExodusRequestScopeModule().Init(httpContext1.ApplicationInstance);
            IDisposable disposable1 = (IDisposable)_container.Resolve(typeof(IDisposable));
            HttpContext httpContext2 = MockWebServer.GetHttpContext();
            new ExodusRequestScopeModule().Init(httpContext2.ApplicationInstance);
            IDisposable disposable2 = (IDisposable)_container.Resolve(typeof(IDisposable));
            Assert.IsNotNull(disposable1);
            Assert.IsNotNull(disposable2);
            Assert.AreNotSame(disposable1, disposable2);
            CallContext.HostContext = httpContext1;
            IDisposable disposable3 = (IDisposable)_container.Resolve(typeof(IDisposable));
            Assert.AreSame(disposable1, disposable3);
            CallContext.HostContext = httpContext2;
            IDisposable disposable4 = (IDisposable)_container.Resolve(typeof(IDisposable));
            Assert.AreSame(disposable2, disposable4);

            MockDisposable mockDisposable1 = (MockDisposable)disposable1;
            MockDisposable mockDisposable2 = (MockDisposable)disposable2;
            Assert.AreEqual(0, mockDisposable1.DisposeCount);
            Assert.AreEqual(0, mockDisposable2.DisposeCount);
            MockWebServer.CallApplicationEndRequest(httpContext1);
            Assert.AreEqual(1, mockDisposable1.DisposeCount);
            Assert.AreEqual(0, mockDisposable2.DisposeCount);
            MockWebServer.CallApplicationEndRequest(httpContext2);
            Assert.AreEqual(1, mockDisposable1.DisposeCount);
            Assert.AreEqual(1, mockDisposable2.DisposeCount);
        }
    }
}