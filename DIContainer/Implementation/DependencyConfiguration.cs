﻿using DIContainer.Enum;
using DIContainer.Implementation;
using DIContainer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIContainer
{
    public class DependencyConfiguration : IDependencyConfiguration
    {
        private readonly List<DependencyDescriptor> _dependencies = new();

        public void Register<TDependency, TImplementation>(LifeTime lifeCycle = LifeTime.Transient)
            where TDependency : class
            where TImplementation : TDependency
        {          
            var dependencyDescriptor = new DependencyDescriptor
                (typeof(TDependency),
                typeof(TImplementation),
                lifeCycle
                );
            _dependencies.Add(dependencyDescriptor);
        }

        internal List<DependencyDescriptor> GetDependencies() => _dependencies;

    }
}
