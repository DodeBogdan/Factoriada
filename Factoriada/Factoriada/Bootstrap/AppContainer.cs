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
            builder.RegisterType<AppShellViewModel>();
            builder.RegisterType<LogInViewModel>();
            builder.RegisterType<RegisterViewModel>();
            builder.RegisterType<UserAccountViewModel>();
            builder.RegisterType<CreateApartmentViewModel>();
            builder.RegisterType<MyApartmentViewModel>();
            builder.RegisterType<AnnounceViewModel>();
            builder.RegisterType<BillViewModel>();
            builder.RegisterType<BudgetViewModel>();
            builder.RegisterType<BuyListViewModel>();
            builder.RegisterType<ChatViewModel>();
            builder.RegisterType<RulesViewModel>();
            builder.RegisterType<AddBillViewModel>();
            builder.RegisterType<SeeBillsViewModel>();
            builder.RegisterType<TimeAwayViewModel>();
            builder.RegisterType<SeePersonsFromApartmentViewModel>();

            //services - data
            builder.RegisterType<DialogService>().As<IDialogService>();
            builder.RegisterType<NavigationService>().As<INavigationService>();
            builder.RegisterType<UserService>().As<IUserService>();
            builder.RegisterType<ApartmentService>().As<IApartmentService>();


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
