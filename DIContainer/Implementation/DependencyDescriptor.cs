using DIContainer.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIContainer.Implementation
{
    internal class DependencyDescriptor
    {
        public Type DependencyType { get; }
        public Type ImplementationType { get; }
        public LifeTime LifeTime { get; }

        internal DependencyDescriptor(Type dependencyType, Type implementationType, LifeTime lifeTime)
        {
            DependencyType = dependencyType;
            ImplementationType = implementationType;
            LifeTime = lifeTime;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj.GetType() != typeof(DependencyDescriptor))
            {
                return false;
            }
            
            var obj2 = (DependencyDescriptor)obj;
            return DependencyType==obj2.DependencyType && ImplementationType==obj2.ImplementationType;
        }

        public override int GetHashCode()
        {
            return DependencyType.GetHashCode() ^ ImplementationType.GetHashCode();
        }
    }
}
