using System.Collections.Generic;
using Nancy.Bootstrapper;
using StructureMap;

namespace Nancy.Bootstrappers.StructureMap
{
    using Nancy.ViewEngines;

    /// <summary>
    /// Nancy bootstrapper for the StructureMap container.
    /// </summary>
    public abstract class StructureMapNancyBootstrapper : NancyBootstrapperWithRequestContainerBase<IContainer>
    {
        /// <summary>
        /// Gets all registered startup tasks
        /// </summary>
        /// <returns>An <see cref="System.Collections.Generic.IEnumerable{T}"/> instance containing <see cref="IStartup"/> instances. </returns>
        protected override IEnumerable<IStartup> GetStartupTasks()
        {
            return this.ApplicationContainer.GetAllInstances<IStartup>();
        }

        /// <summary>
        /// Resolve INancyEngine
        /// </summary>
        /// <returns>INancyEngine implementation</returns>
        protected override INancyEngine GetEngineInternal()
        {
            return this.ApplicationContainer.GetInstance<INancyEngine>();
        }

        /// <summary>
        /// Get the moduleKey generator
        /// </summary>
        /// <returns>IModuleKeyGenerator instance</returns>
        protected override IModuleKeyGenerator GetModuleKeyGenerator()
        {
            return this.ApplicationContainer.GetInstance<IModuleKeyGenerator>();
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
        /// to take the responsibility of registering things like INancyModuleCatalog manually.
        /// </summary>
        /// <param name="applicationContainer">Application container to register into</param>
        protected override void RegisterBootstrapperTypes(IContainer applicationContainer)
        {
            applicationContainer.Configure(registry => registry.For<INancyModuleCatalog>().Singleton().Use(this));

            // Adding this hear because SM doesn't use the greediest resolvable
            // constructor, just the greediest
            applicationContainer.Configure(registry => registry.For<IFileSystemReader>().Singleton().Use<DefaultFileSystemReader>());
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
        /// <param name="moduleRegistrationTypes">NancyModule types</param>
        protected override void RegisterRequestContainerModules(IContainer container, IEnumerable<ModuleRegistration> moduleRegistrationTypes)
        {
            container.Configure(registry =>
            {
                foreach (var registrationType in moduleRegistrationTypes)
                {
                    registry.For(typeof(NancyModule))
                        .LifecycleIs(InstanceScope.Unique)
                        .Use(registrationType.ModuleType)
                        .Named(registrationType.ModuleKey);
                }
            });
        }

        /// <summary>
        /// Retrieve all module instances from the container
        /// </summary>
        /// <param name="container">Container to use</param>
        /// <returns>Collection of NancyModule instances</returns>
        protected override IEnumerable<NancyModule> GetAllModules(IContainer container)
        {
            return container.GetAllInstances<NancyModule>();
        }

        /// <summary>
        /// Retreive a specific module instance from the container by its key
        /// </summary>
        /// <param name="container">Container to use</param>
        /// <param name="moduleKey">Module key of the module</param>
        /// <returns>NancyModule instance</returns>
        protected override NancyModule GetModuleByKey(IContainer container, string moduleKey)
        {
            return container.TryGetInstance<NancyModule>(moduleKey);
        }
    }
}