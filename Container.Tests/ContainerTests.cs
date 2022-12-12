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
        private IDependencyProvider _provider;

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
            Assert.That(result.GetType(), Is.EqualTo(typeof(ServiceImpl<IRepository>)));
            ServiceImpl<IRepository> service = (ServiceImpl<IRepository>)result;
            Assert.That(service.rep.GetType(), Is.EqualTo(typeof(RepositoryImpl)));
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
            Assert.That(result1, Is.EqualTo(result2)); // links are equal
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
            Assert.That(result, Is.Not.Null);
            Assert.That(result.GetType(), Is.EqualTo(typeof(DefaultConstructorClass)));
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
            Assert.That(result.Count(), Is.EqualTo(2));
        }


        /// <summary>
        /// Open generic test
        /// </summary>
        [Test]
        public void OpenGenericTest()
        { 
            var configuration = new DependencyConfiguration();
            configuration.Register<IRepository, MySqlRepository>();
            configuration.Register<IService<IRepository>, ServiceImpl<IRepository>>();
            configuration.Register(typeof(IService<>), typeof(ServiceImpl<>));
            var provider = new DependencyProvider(configuration);
            var result = provider.Resolve<IService<IRepository>>();
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf(typeof(ServiceImpl<IRepository>)));
        }

        /// <summary>
        /// As self dependency
        /// </summary>
        [Test]
        public void AsSelfDependencyTest()
        {
            var configuration = new DependencyConfiguration();
            configuration.Register<SomeServiceImpl, SomeServiceImpl>();
            var provider = new DependencyProvider(configuration);
            SomeServiceImpl repository = provider.Resolve<SomeServiceImpl>();
            Assert.That(repository, Is.Not.Null);
            Assert.That(repository, Is.InstanceOf(typeof(SomeServiceImpl)));
        }

        /// <summary>
        /// Transient test lifetime
        /// </summary>
        [Test]
        public void TransientTest()
        {
            var configuration = new DependencyConfiguration();
            configuration.Register<IImplementation, DefaultConstructorClass>(LifeTime.Transient);
            var provider = new DependencyProvider(configuration);
            var result1 = provider.Resolve<IImplementation>();
            var result2 = provider.Resolve<IImplementation>();
            Assert.That(result2, Is.Not.EqualTo(result1));
        }


        /// <summary>
        /// Pass null to non-generic method Register
        /// </summary>
        [Test]
        public void NullArgumentRegisterTest()
        {
            var configuration = new DependencyConfiguration();
            Assert.Throws<ArgumentNullException>(() => configuration.Register(null, null));
        }
    }
}