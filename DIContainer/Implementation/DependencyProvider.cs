using DIContainer.Enum;
using DIContainer.Interfaces;
using System.Reflection;
using System.Collections;

namespace DIContainer.Implementation
{
    public class DependencyProvider : IDependencyProvider
    {
        private readonly DependencyConfiguration _configuration;
        private readonly Dictionary<Type, object> _singletonList = new();
        private readonly object _lock = new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public DependencyProvider(DependencyConfiguration? configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            _configuration = configuration;
            ValidateConfiguration();
        }

        public T Resolve<T>() where T : class
        {
            return (T)Resolve(typeof(T));
        }


        private object GenerateImplementation(Type t)
        {
            var dependencies = _configuration.GetDependencies();
            var implementation = dependencies.Find(d => d.DependencyType == t);

            if (implementation == null)
            {
                if (!t.IsGenericType)
                {
                    throw new ArgumentNullException($"No dependency implementation {nameof(t)}");
                }

                //get definition - ex. typeof(IService<>)
                var typeDef = t.GetGenericTypeDefinition();
                implementation = dependencies.Find(d => d.DependencyType == typeDef);
                if (implementation == null)
                {
                    throw new ArgumentNullException($"No open generic implementation {nameof(t)}");
                }

                var argument = t.GetGenericArguments()[0];

                var argumentImplementation = dependencies.Find(d => d.DependencyType == argument);
                if (argumentImplementation == null)
                {
                    throw new ArgumentNullException(nameof(argumentImplementation));
                }

                var newType = implementation.ImplementationType
                    .MakeGenericType(argumentImplementation.ImplementationType);
                _configuration.Register(t, newType, implementation.LifeTime);

            }
            return DefineSingleton(implementation, t);
        }

        private object DefineSingleton(DependencyDescriptor implementation, Type t)
        {
            if (implementation.LifeTime == LifeTime.Singleton)
            {
                return GetSingleton(t, implementation.ImplementationType);
            }
            return CreateElement(implementation.ImplementationType);
        }

        private object Resolve(Type tDependency)
        {
            if (tDependency.IsGenericType && tDependency.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                return GenerateImplementations(tDependency.GetGenericArguments().ElementAt(0));
            }
            return GenerateImplementation(tDependency);
        }

        private IEnumerable<object> GenerateImplementations(Type tDependency)
        {
            var dependencies = _configuration.GetDependencies();
            var implementations = dependencies.FindAll(d => d.DependencyType == tDependency);
            var instances = (IList?)Activator.CreateInstance(typeof(List<>).MakeGenericType(tDependency));
            foreach (var implementation in implementations)
            {
                instances!.Add(DefineSingleton(implementation, tDependency));
            }
            return (IEnumerable<object>)instances;
        }

        private object CreateElement(Type t)
        {
            var publicConstructors = t.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

            if (publicConstructors == null)
            {
                throw new ArgumentNullException($"Type doesn't contain public constructor {nameof(t)}");
            }

            var lessParamConstructor = publicConstructors.OrderBy(c => c.GetParameters().Length).First();

            var parameters = lessParamConstructor.GetParameters();

            var paramsArr = new object[parameters.Length];

            for (int i = 0; i < paramsArr.Length; i++)
            {
                var typeParam = parameters[i].ParameterType;
                // if interface or abstract class - try to resolve it
                if (typeParam.IsInterface || typeParam.IsAbstract)
                {
                    paramsArr[i] = GenerateImplementation(typeParam);
                }
                else //otherwise - create new object
                {
                    paramsArr[i] = CreateElement(typeParam);
                }
            }

            try
            {
                return Activator.CreateInstance(t, args: paramsArr);
            }
            catch
            {
                throw new Exception($"Error during instance creation {nameof(t)}");
            }
        }

        private object GetSingleton(Type tDependency, Type tImplementation)
        {
            // avoid blocking if value exists
            if (!_singletonList.TryGetValue(tDependency, out var singleton))
            {
                lock (_lock) // lock
                {
                    if (singleton == null) // if another thread added element
                    {
                        _singletonList.Add(tDependency, CreateElement(tImplementation));
                    }
                }
            }
            return _singletonList[tDependency];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        private void ValidateConfiguration()
        {
            var dependencies = _configuration.GetDependencies();
            bool hasSame = dependencies.Count != dependencies.Distinct().Count();

            if (hasSame) // if container contains the same implementations
            {
                throw new ArgumentException($"Configuration contains the same implementations");
            }

            bool hasAbstract = dependencies.Any(d => d.ImplementationType.IsAbstract
            || d.ImplementationType.IsInterface);

            if (hasAbstract) // implementation is interface or abstract class
            {
                throw new ArgumentException($"Configuration contains abstract or interface implementation");
            }
        }
    }
}
