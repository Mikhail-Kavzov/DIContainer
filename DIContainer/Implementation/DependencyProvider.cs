using DIContainer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIContainer.Implementation
{
    public class DependencyProvider : IDependencyProvider
    {
        private readonly DependencyConfiguration _configuration;

        public DependencyProvider(DependencyConfiguration configuration)
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
            throw new NotImplementedException();
        }

        private void ValidateConfiguration()
        {

        }
    }
}
