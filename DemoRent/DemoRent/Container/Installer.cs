using Caliburn.Micro;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using DemoRent.Services;
using DemoRent.ViewModels;

namespace DemoRent.Container
{
    public class Installer : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container
                .Register(Component.For<ShellViewModel>().ImplementedBy<ShellViewModel>().LifestyleSingleton())
                .Register(Component.For<IWindowManager>().ImplementedBy<WindowManager>())
                .Register(Component.For<IEventAggregator>().ImplementedBy<EventAggregator>().LifestyleSingleton())
                .Register(Component.For<FlightsViewModel>().ImplementedBy<FlightsViewModel>().LifestyleTransient())
                .Register(Component.For<IFlightScheduleProvider>().ImplementedBy<FlightScheduleProvider>()
                    .LifestyleSingleton());
        }
    }
}