using DIContainer.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIContainer.Interfaces
{
    public interface IDependencyConfiguration    
    {
        void Register<TDependency, TImplementation>(LifeTime lifeCycle = LifeTime.Transient)
            where TDependency : class
            where TImplementation : TDependency;

        void Register(Type tDependency,
            Type tImplementation, LifeTime lifeCycle = LifeTime.Transient);
    }
}
