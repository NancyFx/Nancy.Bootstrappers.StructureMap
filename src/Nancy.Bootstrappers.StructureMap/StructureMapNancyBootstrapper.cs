namespace Nancy.Bootstrappers.StructureMap
{
    using System;
    using System.Collections.Generic;
    using Diagnostics;
    using Nancy.Bootstrapper;
    using Nancy.ViewEngines;
    using global::StructureMap;
    using Routing;

    /// <summary>
    /// Nancy bootstrapper for the StructureMap container.
    /// </summary>
    public abstract class StructureMapNancyBootstrapper : NancyBootstrapperWithRequestContainerBase<IContainer>, IDisposable
    {
        /// <summary>
        /// Gets the diagnostics for intialisation
        /// </summary>
        /// <returns>An <see cref="IDiagnostics"/> implementation</returns>
        protected override IDiagnostics GetDiagnostics()
        {
            return this.ApplicationContainer.GetInstance<IDiagnostics>();
        }

        /// <summary>
        /// Gets all registered application startup tasks
        /// </summary>
        /// <returns>An <see cref="System.Collections.Generic.IEnumerable{T}"/> instance containing <see cref="IApplicationStartup"/> instances. </returns>
        protected override IEnumerable<IApplicationStartup> GetApplicationStartupTasks()
        {
            return this.ApplicationContainer.GetAllInstances<IApplicationStartup>();
        }

        /// <summary>
        /// Gets all registered application registration tasks
        /// </summary>
        /// <returns>An <see cref="System.Collections.Generic.IEnumerable{T}"/> instance containing <see cref="IApplicationRegistrations"/> instances.</returns>
        protected override IEnumerable<IApplicationRegistrations> GetApplicationRegistrationTasks()
        {
            return this.ApplicationContainer.GetAllInstances<IApplicationRegistrations>();
        }

        /// <summary>
        /// Resolve <see cref="INancyEngine"/>
        /// </summary>
        /// <returns><see cref="INancyEngine"/> implementation</returns>
        protected override INancyEngine GetEngineInternal()
        {
            return this.ApplicationContainer.GetInstance<INancyEngine>();
        }

        /// <summary>
        /// Gets the application level container
        /// </summary>
        /// <returns>Container instance</returns>
        protected override IContainer GetApplicationContainer()
        {
            return new Container();
        }

        /// <summary>
        /// Register the bootstrapper's implemented types into the container.
        /// This is necessary so a user can pass in a populated container but not have
        /// to take the responsibility of registering things like <see cref="INancyModuleCatalog"/> manually.
        /// </summary>
        /// <param name="applicationContainer">Application container to register into</param>
        protected override void RegisterBootstrapperTypes(IContainer applicationContainer)
        {
            applicationContainer.Configure(registry => registry.For<INancyModuleCatalog>().Singleton().Use(this));

            // Adding this hear because SM doesn't use the greediest resolvable
            // constructor, just the greediest
            applicationContainer.Configure(registry => registry.For<IFileSystemReader>().Singleton().Use<DefaultFileSystemReader>());

            // DefaultRouteCacheProvider doesn't have a parameterless constructor.
            // It has a Func<IRouteCache> parameter, which StructureMap doesn't know how to handle
            var routeCacheFactory = new Func<IRouteCache>(ObjectFactory.GetInstance<IRouteCache>);
            applicationContainer.Configure(registry => registry.For<Func<IRouteCache>>().Use(routeCacheFactory));
        }

        /// <summary>
        /// Register the default implementations of internally used types into the container as singletons
        /// </summary>
        /// <param name="container">Container to register into</param>
        /// <param name="typeRegistrations">Type registrations to register</param>
        protected override void RegisterTypes(IContainer container, IEnumerable<TypeRegistration> typeRegistrations)
        {
            container.Configure(registry =>
                {
                    foreach (var typeRegistration in typeRegistrations)
                    {
                        registry.For(typeRegistration.RegistrationType).LifecycleIs(InstanceScope.Singleton).Use(
                            typeRegistration.ImplementationType);
                    }
                });
        }

        /// <summary>
        /// Register the various collections into the container as singletons to later be resolved
        /// by IEnumerable{Type} constructor dependencies.
        /// </summary>
        /// <param name="container">Container to register into</param>
        /// <param name="collectionTypeRegistrationsn">Collection type registrations to register</param>
        protected override void RegisterCollectionTypes(IContainer container, IEnumerable<CollectionTypeRegistration> collectionTypeRegistrationsn)
        {
            container.Configure(registry =>
            {
                foreach (var collectionTypeRegistration in collectionTypeRegistrationsn)
                {
                    foreach (var implementationType in collectionTypeRegistration.ImplementationTypes)
                    {
                        registry.For(collectionTypeRegistration.RegistrationType).LifecycleIs(InstanceScope.Singleton).Use(implementationType);
                    }
                }
            });
        }

        /// <summary>
        /// Register the given instances into the container
        /// </summary>
        /// <param name="container">Container to register into</param>
        /// <param name="instanceRegistrations">Instance registration types</param>
        protected override void RegisterInstances(IContainer container, IEnumerable<InstanceRegistration> instanceRegistrations)
        {
            container.Configure(registry =>
            {
                foreach (var instanceRegistration in instanceRegistrations)
                {
                    registry.For(instanceRegistration.RegistrationType).LifecycleIs(InstanceScope.Singleton).Use(instanceRegistration.Implementation);
                }
            });
        }

        /// <summary>
        /// Creates a per request child/nested container
        /// </summary>
        /// <returns>Request container instance</returns>
        protected override IContainer CreateRequestContainer()
        {
            return this.ApplicationContainer.GetNestedContainer();
        }

        /// <summary>
        /// Register the given module types into the request container
        /// </summary>
        /// <param name="container">Container to register into</param>
        /// <param name="moduleRegistrationTypes"><see cref="INancyModule"/> types</param>
        protected override void RegisterRequestContainerModules(IContainer container, IEnumerable<ModuleRegistration> moduleRegistrationTypes)
        {
            container.Configure(registry =>
            {
                foreach (var registrationType in moduleRegistrationTypes)
                {
                    registry.For(typeof(INancyModule)).LifecycleIs(InstanceScope.Unique).Use(registrationType.ModuleType);
                }
            });
        }

        /// <summary>
        /// Retrieve all module instances from the container
        /// </summary>
        /// <param name="container">Container to use</param>
        /// <returns>Collection of <see cref="INancyModule"/> instances</returns>
        protected override IEnumerable<INancyModule> GetAllModules(IContainer container)
        {
            return container.GetAllInstances<INancyModule>();
        }

        /// <summary>
        /// Retreive a specific module instance from the container
        /// </summary>
        /// <param name="container">Container to use</param>
        /// <param name="moduleType">Type of the module</param>
        /// <returns>A <see cref="INancyModule"/> instance</returns>
        protected override INancyModule GetModule(IContainer container, Type moduleType)
        {
            container.Configure(registry =>
            {
                registry.For(typeof(INancyModule)).LifecycleIs(InstanceScope.Unique).Use(moduleType);
            });

            return container.TryGetInstance<INancyModule>();
        }

        private bool disposing = false;
        public new void Dispose()
        {
            if (disposing)
                return;

            disposing = true;
            base.Dispose();
        }
    }
}