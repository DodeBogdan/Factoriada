using Autofac;
using Factoriada.Services;
using Factoriada.Services.Interfaces;
using Factoriada.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Factoriada.Bootstrap
{
    public class AppContainer
    {
        private static IContainer _container;

        public static void RegisterDependencies()
        {
            var builder = new ContainerBuilder();

            //ViewModels
            builder.RegisterType<HomeViewModel>();

            //services - data
            builder.RegisterType<DialogService>().As<IDialogService>();
            builder.RegisterType<NavigationService>().As<INavigationService>();

            //General
            
            _container = builder.Build();
        }

        public static object Resolve(Type typeName)
        {
            return _container.Resolve(typeName);
        }

        public static T Resolve<T>()
        {
            return _container.Resolve<T>();
        }
    }
}
