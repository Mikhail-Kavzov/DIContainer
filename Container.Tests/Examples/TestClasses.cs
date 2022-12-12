using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIContainer.Examples
{
    interface Impl : IImplementation
    {
        void Method2();
    }

    interface IImplementation
    {
        void Method();
    }

    class DefaultConstructorClass : IImplementation
    {
        public DefaultConstructorClass()
        {
        }

        public void Method()
        {
        }
    }

    class DefaultConstructorClass1 : IImplementation
    {
        public DefaultConstructorClass1()
        {
        }

        public void Method()
        {
        }
    }

    abstract class AbstractClass : Impl
    {

        public void Method()
        {

        }

        public void Method2()
        {

        }
    }

    interface IRepository
    {

    }

    interface IService<TRepository> where TRepository : IRepository
    {
    }


    class MySqlRepository : IRepository
    {

    }

    class ServiceImpl<TRepository> : IService<TRepository>
        where TRepository : IRepository
    {
        public TRepository rep;

        public ServiceImpl(TRepository repository)
        {
            rep = repository;
        }

    }


    interface IService { }
    class ServiceImpl : IService
    {
        public IRepository _rep;

        public ServiceImpl(IRepository repository) 
        {
            _rep=repository;
        }
    }


    interface ISomeService { }
    class RepositoryImpl : IRepository
    {
        public ISomeService _rep;

        public RepositoryImpl(ISomeService service)
        {
            _rep=service;
        } 
    }

    class SomeServiceImpl : ISomeService { }
}
