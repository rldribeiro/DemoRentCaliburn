using System.Collections.Generic;

namespace DemoRent
{
    using System;
    using System.Windows;
    using Caliburn.Micro;
    using Castle.Windsor;
    using DemoRent.ViewModels;
    using DemoRent.Container;

    public class Bootstrapper : BootstrapperBase
    {
        private readonly IWindsorContainer _container = new WindsorContainer();

        public Bootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }

        protected override object GetInstance(Type service, string key)
        {
            var instance = _container.Resolve(service);
            if (instance != null)
                return instance;

            throw new InvalidOperationException("Could not locate any instances.");
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            var array = _container.ResolveAll(service);
            var trgArray = new object[array.Length];
            Array.Copy(array, trgArray, array.Length);
            return trgArray;
        }

        protected override void Configure()
        {
            _container.Install(new Installer());
        }
    }
}
