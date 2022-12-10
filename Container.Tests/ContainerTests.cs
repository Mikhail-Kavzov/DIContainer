using DIContainer;
using DIContainer.Enum;
using DIContainer.Examples;
using DIContainer.Implementation;
using DIContainer.Interfaces;
using NUnit.Framework;

namespace Container.Tests
{
    public class ContainerTests
    {
        private  IDependencyProvider _provider;

        [SetUp]
        public void Setup()
        {
            var configuration = new DependencyConfiguration();
            configuration.Register<IImplementation,DefaultConstructorClass>();
            configuration.Register<IService, ServiceImpl>();
            configuration.Register<ISomeService, SomeServiceImpl>();
            configuration.Register<IRepository, RepositoryImpl>();
            configuration.Register<IService<IRepository>, ServiceImpl<IRepository>>();

            _provider = new DependencyProvider(configuration);
        }

        /// <summary>
        /// Recursion generic test
        /// </summary>
        [Test]
        public void RecursionGenericTest()
        {
            var result=_provider.Resolve<IService<IRepository>>();
            Assert.That(result.GetType() == typeof(ServiceImpl<IRepository>));
            ServiceImpl<IRepository> service = (ServiceImpl<IRepository>)result;
            Assert.That(service.rep.GetType() == typeof(RepositoryImpl));
        }

        /// <summary>
        /// Singleton example
        /// </summary>
        [Test]
        public void SingletonTest()
        {
            var configuration = new DependencyConfiguration();
            configuration.Register<IImplementation, DefaultConstructorClass>(LifeTime.Singleton);
            var provider = new DependencyProvider(configuration);
            var result1 = provider.Resolve<IImplementation>();
            var result2 = provider.Resolve<IImplementation>();
            Assert.That(result1.Equals(result2)); // links are equal
        }

        /// <summary>
        /// Interface -> implementation Test
        /// </summary>
        [Test]
        public void InterfaceImplementationTest()
        {
            var configuration = new DependencyConfiguration();
            configuration.Register<IImplementation, DefaultConstructorClass>();
            var provider = new DependencyProvider(configuration);
            var result = provider.Resolve<IImplementation>();
            Assert.IsNotNull(result);
            Assert.That(result.GetType()==typeof(DefaultConstructorClass));
        }

        /// <summary>
        /// Implementation contains interface - throws exception
        /// </summary>
        [Test]
        public void InterfaceInImplementationTest()
        {
            var configuration = new DependencyConfiguration();
            configuration.Register<IImplementation, Impl>();
            Assert.Throws<ArgumentException>(() => new DependencyProvider(configuration));
        }

        /// <summary>
        /// Implementation contains abstract class - throws exception
        /// </summary>
        [Test]
        public void AbstractClassInImplementationTest()
        {
            var configuration = new DependencyConfiguration();
            configuration.Register<Impl, AbstractClass>();
            Assert.Throws<ArgumentException>(() => new DependencyProvider(configuration));
        }

        /// <summary>
        /// Configuration contains the same implementations (Dependency -> implementation)
        /// LifeTime doesn't matter
        /// </summary>
        [Test]
        public void SameImplementationsTest()
        {
            var configuration = new DependencyConfiguration();
            configuration.Register<IImplementation, DefaultConstructorClass>();
            configuration.Register<IImplementation, DefaultConstructorClass>();
            Assert.Throws<ArgumentException>(() => new DependencyProvider(configuration));
        }

        /// <summary>
        /// Configuration is null
        /// </summary>
        [Test]
        public void NullConfigurationTest()
        {
            Assert.Throws<ArgumentNullException>(() => new DependencyProvider(null));
        }

        /// <summary>
        /// Find all configurations
        /// </summary>
        [Test]
        public void AllConfigutationsTest()
        {
            var configuration = new DependencyConfiguration();
            configuration.Register<IImplementation, DefaultConstructorClass>();
            configuration.Register<IImplementation, DefaultConstructorClass1>();
            var provider = new DependencyProvider(configuration);
            var result = provider.Resolve<IEnumerable<IImplementation>>();
            Assert.That(result.Count() == 2);
        }


        /// <summary>
        /// Open generic test
        /// </summary>
        [Test]
        public void OpenGenericTest()
        { 
            var configuration = new DependencyConfiguration();
            configuration.Register(typeof(IService<>), typeof(ServiceImpl<>));
            configuration.Register<IMySqlRepository, MySqlRepository>();
            var provider = new DependencyProvider(configuration);
            var result = provider.Resolve<IService<IMySqlRepository>>();
           
        }


    }
}