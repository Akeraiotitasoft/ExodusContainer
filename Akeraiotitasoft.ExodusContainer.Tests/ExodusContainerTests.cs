using Akeraiotitasoft.ExodusContainer;
using Akeraiotitasoft.ExodusContainer.Governor;
using Akeraiotitasoft.ExodusContainer.Tests.Mock.Abstraction;
using Akeraiotitasoft.ExodusContainer.Tests.Mock.Concrete;
using Akeraiotitasoft.ExodusContainer.Tests.Mock.Extensions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;


#if LOGGING
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
#endif

namespace ExodusBuildingBlocks.Container.Tests
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
        public void NothingIsRegistered()
        {
            Assert.Throws<InvalidOperationException>(() => _container.Resolve(typeof(ITestInterface)));
        }

        [Test]
        public void ContainerCallsDisposeForSingleton()
        {
            _container.Register(typeof(IDisposable), typeof(MockDisposable), new SingletonGovernor());
            IDisposable myDisposable = (IDisposable)_container.Resolve(typeof(IDisposable));
            _container.Dispose();
            MockDisposable mockDisposable = (MockDisposable)myDisposable;
            Assert.AreEqual(1, mockDisposable.DisposeCount);
        }

        [Test]
        public void ContainerCallsDisposeForTransient()
        {
            _container.Register(typeof(IDisposable), typeof(MockDisposable), new TransientGovernor());
            IDisposable myDisposable = (IDisposable)_container.Resolve(typeof(IDisposable));
            _container.Dispose();
            MockDisposable mockDisposable = (MockDisposable)myDisposable;
            Assert.AreEqual(1, mockDisposable.DisposeCount);
        }

        [Test]
        public void ContainerHasSingletons()
        {
            _container.Register(typeof(IDisposable), typeof(MockDisposable), new SingletonGovernor());
            IDisposable disposable = (IDisposable)_container.Resolve(typeof(IDisposable));
            IDisposable disposable2 = (IDisposable)_container.Resolve(typeof(IDisposable));
            Assert.AreSame(disposable, disposable2);
        }

        [Test]
        public void ContainerHasTransient()
        {
            _container.Register(typeof(IDisposable), typeof(MockDisposable), new TransientGovernor());
            IDisposable disposable = (IDisposable)_container.Resolve(typeof(IDisposable));
            IDisposable disposable2 = (IDisposable)_container.Resolve(typeof(IDisposable));
            Assert.AreNotSame(disposable, disposable2);
        }

        [Test]
        public void ContainerHasMockScope()
        {
            _container.Register(typeof(IDisposable), typeof(MockDisposable), new MockScopeGovernor());

            MockScope.Current = new MockScope();

            IDisposable disposable1 = (IDisposable)_container.Resolve(typeof(IDisposable));
            IDisposable disposable2 = (IDisposable)_container.Resolve(typeof(IDisposable));
            IDisposable disposable3 = (IDisposable)_container.Resolve(typeof(IDisposable));
            Assert.AreSame(disposable1, disposable2);
            Assert.AreSame(disposable1, disposable3);

            MockScope.Current = new MockScope();

            IDisposable disposable4 = (IDisposable)_container.Resolve(typeof(IDisposable));
            IDisposable disposable5 = (IDisposable)_container.Resolve(typeof(IDisposable));
            IDisposable disposable6 = (IDisposable)_container.Resolve(typeof(IDisposable));
            Assert.AreSame(disposable4, disposable5);
            Assert.AreSame(disposable4, disposable6);

            Assert.AreNotSame(disposable1, disposable4);

            MockScope.Current = null;
            IDisposable disposable7 = (IDisposable)_container.Resolve(typeof(IDisposable));
            IDisposable disposable8 = (IDisposable)_container.Resolve(typeof(IDisposable));

            Assert.AreNotSame(disposable1, disposable7);
            Assert.AreNotSame(disposable4, disposable7);
            Assert.AreNotSame(disposable1, disposable8);
            Assert.AreNotSame(disposable4, disposable8);
            Assert.AreNotSame(disposable7, disposable8);
        }

        [Test]
        public void ContainerHasPerThreadScope()
        {
            _container.Register(typeof(IDisposable), typeof(MockDisposable), new PerThreadGovernor());
            IDisposable disposable1 = null;
            IDisposable disposable2 = null;
            Parallel.Invoke(new ParallelOptions() { MaxDegreeOfParallelism = 2 },
                () => disposable1 = (IDisposable)_container.Resolve(typeof(IDisposable)),
                () => disposable2 = (IDisposable)_container.Resolve(typeof(IDisposable)));
            Assert.IsNotNull(disposable1);
            Assert.IsNotNull(disposable2);
            Assert.AreNotSame(disposable1, disposable2);
        }

        [Test]
        public void ContainerHasScopeScope()
        {
            _container.Register(typeof(IDisposable), typeof(MockDisposable), new ScopeGovernor());
            IDisposable disposable1;
            IDisposable disposable2;
            IDisposable disposable3;
            IDisposable disposable4;

            var scope1 = _container.BeginScope();
            disposable1 = (IDisposable)scope1.Resolve(typeof(IDisposable));
            var scope2 = _container.BeginScope();
            disposable2 = (IDisposable)scope2.Resolve(typeof(IDisposable));
            disposable3 = (IDisposable)scope1.Resolve(typeof(IDisposable));
            disposable4 = (IDisposable)scope2.Resolve(typeof(IDisposable));


            MockDisposable mockDisposable1 = (MockDisposable)disposable1;
            MockDisposable mockDisposable2 = (MockDisposable)disposable2;

            Assert.AreEqual(0, mockDisposable1.DisposeCount);
            Assert.AreEqual(0, mockDisposable2.DisposeCount);

            scope1.Dispose();

            Assert.AreEqual(1, mockDisposable1.DisposeCount);
            Assert.AreEqual(0, mockDisposable2.DisposeCount);

            scope2.Dispose();

            Assert.AreEqual(1, mockDisposable1.DisposeCount);
            Assert.AreEqual(1, mockDisposable2.DisposeCount);

            Assert.IsNotNull(disposable1);
            Assert.IsNotNull(disposable2);
            Assert.AreNotSame(disposable1, disposable2);
            Assert.AreSame(disposable1, disposable3);
            Assert.AreSame(disposable2, disposable4);
        }

        [Test]
        public void ContainerCallsDisposeOnMockScope()
        {
            _container.Register(typeof(IDisposable), typeof(MockBigDisposable), new MockScopeGovernor());

            MockScope.CreateScope();

            IDisposable myDisposable = (IDisposable)_container.Resolve(typeof(IDisposable));

            MockScope.Current = null;
            WeakReference proofOfGarbageCollection = CreateProofOfGarbageCollection();
            int i = 0;
            do
            {
                MockBigDisposable anotherBigOne = new MockBigDisposable();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                Thread.Sleep(10000);  // wait 10 seconds
                i++;
            }
            while (proofOfGarbageCollection.IsAlive && i < 10);
            Thread.Sleep(10000);  // wait 10 seconds

            MockBigDisposable mockDisposable = (MockBigDisposable)myDisposable;
            Assert.AreEqual(1, mockDisposable.DisposeCount);
        }

        private WeakReference CreateProofOfGarbageCollection()
        {
            return new WeakReference(new object());
        }

        [Test]
        public void ContainerCallsTestFactoryMethodAsTransient()
        {
            _container.Register(typeof(ITestFactory), typeof(MockFactory), new SingletonGovernor());
            _container.Register(typeof(ITestInterface<>), new TransientGovernor(), (ctr, type, name) =>
            {
                object factory = ctr.Resolve(typeof(ITestFactory));
                Type[] genericArguments = type.GetGenericArguments();
                MethodInfo method = typeof(TestFactoryExtensions).GetMethod("CreateTestInterface").MakeGenericMethod(genericArguments);
                return method.Invoke(null, new[] { factory });
            }
            );

            ITestInterface<int> testInterface = (ITestInterface<int>)_container.Resolve(typeof(ITestInterface<int>));
            ITestInterface<int> testInterface2 = (ITestInterface<int>)_container.Resolve(typeof(ITestInterface<int>));
            Assert.AreNotSame(testInterface, testInterface2);
        }

        [Test]
        public void ContainerCallsTestFactoryMethodAsSingleton()
        {
            _container.Register(typeof(ITestFactory), typeof(MockFactory), new SingletonGovernor());
            _container.Register(typeof(ITestInterface<>), new SingletonGovernor(), (ctr, type, name) =>
            {
                object factory = ctr.Resolve(typeof(ITestFactory));
                Type[] genericArguments = type.GetGenericArguments();
                MethodInfo method = typeof(TestFactoryExtensions).GetMethod("CreateTestInterface").MakeGenericMethod(genericArguments);
                return method.Invoke(null, new[] { factory });
            }
            );

            ITestInterface<int> testInterface = (ITestInterface<int>)_container.Resolve(typeof(ITestInterface<int>));
            ITestInterface<int> testInterface2 = (ITestInterface<int>)_container.Resolve(typeof(ITestInterface<int>));
            Assert.AreSame(testInterface, testInterface2);
        }

        [Test]
        public void ContainerResolvesGeneric()
        {
            _container.Register(typeof(ITestFactory), typeof(MockFactory), new SingletonGovernor());
            _container.Register(typeof(ITestInterface<>), typeof(MockFactoryChild<>), new SingletonGovernor());

            ITestInterface<int> testInterface = (ITestInterface<int>)_container.Resolve(typeof(ITestInterface<int>));
            ITestInterface<int> testInterface2 = (ITestInterface<int>)_container.Resolve(typeof(ITestInterface<int>));
            Assert.AreSame(testInterface, testInterface2);
            Assert.IsAssignableFrom(typeof(MockFactoryChild<int>),testInterface);
            Assert.IsAssignableFrom(typeof(MockFactoryChild<int>), testInterface2);
        }

        [Test]
        public void ContainerCallsTestFactoryMethodCompiledLambdaAsSingleton()
        {
            _container.Register(typeof(ITestFactory), typeof(MockFactory), new SingletonGovernor());
            _container.Register(typeof(ITestInterface<>), new SingletonGovernor(), CallWithoutReflection);
            ITestInterface<int> testInterface = (ITestInterface<int>)_container.Resolve(typeof(ITestInterface<int>));
            ITestInterface<int> testInterface2 = (ITestInterface<int>)_container.Resolve(typeof(ITestInterface<int>));
            Assert.AreSame(testInterface, testInterface2);
        }

        private Dictionary<Type, Func<IExodusContainer, Type, string, object>> _lambdas = new Dictionary<Type, Func<IExodusContainer, Type, string, object>>();
        private object CallWithoutReflection(IExodusContainer _container, Type type, string name)
        {
            if (_lambdas.ContainsKey(type))
            {
                var func = _lambdas[type];
                return func(_container, type, name);
            }
            Type[] genericArguments = type.GetGenericArguments();
            var lambda = CreateLambda(genericArguments);
            object obj = lambda(_container, type, name);
            _lambdas[type] = lambda;
            return obj;
        }

        private Func<IExodusContainer, Type, string, object> CreateLambda(Type[] genericArguments)
        {
            var _containerParameter = Expression.Parameter(typeof(IExodusContainer), "_container");
            var typeParameter = Expression.Parameter(typeof(Type), "type");
            var nameParameter = Expression.Parameter(typeof(string), "name");

            var factoryType = Expression.Constant(typeof(ITestFactory));
            MethodInfo resolveMethod = typeof(IExodusContainer).GetMethod("Resolve", new[] { typeof(Type) });
            var callResolve = Expression.Call(_containerParameter, resolveMethod, factoryType);
            var factoryInstance = Expression.TypeAs(callResolve, typeof(ITestFactory));
            MethodInfo createMethod = typeof(TestFactoryExtensions).GetMethod("CreateTestInterface").MakeGenericMethod(genericArguments);
            var callCreate = Expression.Call(createMethod, new[] { factoryInstance });

            var expression = Expression.Lambda<Func<IExodusContainer, Type, string, object>>(
                callCreate,
                _containerParameter,
                typeParameter,
                nameParameter);

            return expression.Compile();
        }

        [Test]
        public void ContainerCallsTestFactoryMethodAsSingleton_For2DifferentTypeArguments()
        {
            _container.Register(typeof(ITestFactory), typeof(MockFactory), new SingletonGovernor());
            _container.Register(typeof(ITestInterface<>), new SingletonGovernor(), (ctr, type, name) =>
            {
                object factory = ctr.Resolve(typeof(ITestFactory));
                Type[] genericArguments = type.GetGenericArguments();
                MethodInfo method = typeof(TestFactoryExtensions).GetMethod("CreateTestInterface").MakeGenericMethod(genericArguments);
                return method.Invoke(null, new[] { factory });
            }
            );

            object testInterface = _container.Resolve(typeof(ITestInterface<int>));
            object testInterface2 = _container.Resolve(typeof(ITestInterface<long>));
            Assert.IsAssignableFrom(typeof(MockFactoryChild<int>), testInterface);
            Assert.IsAssignableFrom(typeof(MockFactoryChild<long>), testInterface2);
        }

        [Test]
        public void ContainerResolvesGeneric_For2DifferentTypeArguments()
        {
            _container.Register(typeof(ITestFactory), typeof(MockFactory), new SingletonGovernor());
            _container.Register(typeof(ITestInterface<>), typeof(MockFactoryChild<>), new SingletonGovernor());

            object testInterface = _container.Resolve(typeof(ITestInterface<int>));
            object testInterface2 = _container.Resolve(typeof(ITestInterface<long>));
            Assert.IsAssignableFrom(typeof(MockFactoryChild<int>), testInterface);
            Assert.IsAssignableFrom(typeof(MockFactoryChild<long>), testInterface2);
        }
    }
}