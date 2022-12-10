using DIContainer.Enum;
using DIContainer.Interfaces;

namespace DIContainer.Implementation
{
    public class DependencyProvider : IDependencyProvider
    {
        private readonly DependencyConfiguration _configuration;
        private readonly Dictionary<Type, object> _singletonList = new();
        private readonly object _lock = new ();

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
            Type tDependency = typeof(T);
            return (T)Resolve(tDependency);           
        }

        private object Resolve(Type tDependency)
        {
            var dependencies = _configuration.GetDependencies();

            var implementation = dependencies.Find(d => d.DependencyType == tDependency);

            if (implementation == null)
            {
                throw new ArgumentNullException($"No dependency implementation {nameof(tDependency)}");
            }

            if (implementation.LifeTime == LifeTime.Singleton)
            {
                return GetSingleton(tDependency, implementation.ImplementationType);
            }

            return CreateElement(implementation.ImplementationType);
        }


        private object CreateElement(Type t)
        {
            var publicConstructors = t.GetConstructors().Where(c => c.IsPublic);

            if (publicConstructors == null)
            {
                throw new ArgumentNullException($"Type doesn't contain public constructor {nameof(t)}");
            }

            var lessParamConstructor = publicConstructors.OrderBy(c => c.GetParameters().Length).First();

            var parameters= lessParamConstructor.GetParameters();

            var paramsArr = new object[parameters.Length];

            for (int i=0; i< paramsArr.Length; i++)
            {
                var typeParam = parameters[i].ParameterType;
                // if interface or abstract class - try to resolve it
                if (typeParam.IsInterface || typeParam.IsAbstract)
                {
                    paramsArr[i] = Resolve(typeParam);
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

        private object GetSingleton(Type tDependency,Type tImplementation)
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
