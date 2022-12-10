using DIContainer.Enum;
using DIContainer.Implementation;
using DIContainer.Interfaces;
using Container.Tests.Extensions;

namespace DIContainer
{
    public class DependencyConfiguration : IDependencyConfiguration
    {
        private readonly List<DependencyDescriptor> _dependencies = new();

        public void Register<TDependency, TImplementation>(LifeTime lifeCycle = LifeTime.Transient)
            where TDependency : class
            where TImplementation : TDependency
        {
            _dependencies.Add(CreateDependency(typeof(TDependency), typeof(TImplementation), lifeCycle));
        }

        public void Register(Type tDependency, Type tImplementation,
            LifeTime lifeCycle = LifeTime.Transient)
        {
            ValidateConfig(tDependency, tImplementation, lifeCycle);
            _dependencies.Add(CreateDependency(tDependency, tImplementation, lifeCycle));
        }

        private static DependencyDescriptor CreateDependency(
            Type tDependency, Type tImplementation, LifeTime lifeTime)
        {
            return new DependencyDescriptor(tDependency,tImplementation,lifeTime);
        }

        private static void ValidateConfig(Type tDependency, Type tImplementation, LifeTime lifeCycle)
        {
            if (tDependency == null)
            {
                throw new ArgumentNullException(nameof(tDependency));
            }

            if (tImplementation == null)
            {
                throw new ArgumentNullException(nameof(tImplementation));
            }

            if (tImplementation.IsInterface || tImplementation.IsAbstract)
            {
                throw new ArgumentException("Implementation can't be interface or abstract class");
            }

            if (tDependency.IsGenericType ^ tImplementation.IsGenericType)
            {
                throw new ArgumentException(
                    "Dependency and Implementation types should be both generic or not");
            }

            if (tDependency.IsGenericTypeDefinition)
            {
                if (lifeCycle == LifeTime.Singleton)
                {
                    throw new ArgumentException("Open generic can't be singleton");
                }

                if (!tImplementation.IsAssignableToGenericType(tDependency))
                {
                    throw new ArgumentException("Implementation generic isn't assignable to dependency");
                }
            }
            else
            {
                if (!tImplementation.IsAssignableTo(tDependency))
                {
                    throw new ArgumentException($"Implementation isn't assignable to dependency" +
                        $" {tDependency} {tImplementation}");
                }
            }

        }

        internal List<DependencyDescriptor> GetDependencies() => _dependencies;

    }
}
