using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Tameenk.Core.Configuration;
using Tameenk.Core.Infrastructure.DependencyManagement;

namespace Tameenk.Core.Infrastructure
{
    /// <summary>
    /// Engine
    /// </summary>
    public class TameenkEngine : IEngine
    {
        #region Fields

        private ContainerManager _containerManager;

        #endregion

        #region Utilities

        /// <summary>
        /// Run startup tasks
        /// </summary>
        protected virtual void RunStartupTasks()
        {
            var typeFinder = _containerManager.Resolve<ITypeFinder>();
            var startUpTaskTypes = typeFinder.FindClassesOfType<IStartupTask>();
            var startUpTasks = new List<IStartupTask>();
            foreach (var startUpTaskType in startUpTaskTypes)
                startUpTasks.Add((IStartupTask)Activator.CreateInstance(startUpTaskType));
            //sort
            startUpTasks = startUpTasks.AsQueryable().OrderBy(st => st.Order).ToList();
            foreach (var startUpTask in startUpTasks)
                startUpTask.Execute();
        }

        /// <summary>
        /// Register dependencies
        /// </summary>
        /// <param name="config">Config</param>
        protected virtual void RegisterDependencies(TameenkConfig config)
        {
            var container = BuildDependancyContainer(config);
            this._containerManager = new ContainerManager(container);
            //set dependency resolver
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

        }

        /// <summary>
        /// Register api dependencies
        /// </summary>
        /// <param name="config">Config</param>
        protected virtual void RegisterApiDependencies(HttpConfiguration httpConfig, TameenkConfig config)
        {
            var container = BuildDependancyContainer(config);
            this._containerManager = new ContainerManager(container);
            //set dependency resolver
            httpConfig.DependencyResolver = new AutofacWebApiDependencyResolver(container);

        }

        /// <summary>
        /// Build dependancy container
        /// </summary>
        /// <param name="config">The tameenk configuration.</param>
        /// <returns></returns>
        private IContainer BuildDependancyContainer(TameenkConfig config) {

            //we create new instance of ContainerBuilder
            //because Build() method can only be called once on a ContainerBuilder.
            var builder = new ContainerBuilder();

            // Register all dependencies.
            var typeFinder = new WebAppTypeFinder();
            // Register the configuration if exist. 
            if (config != null)
            {
                builder.RegisterInstance(config).As<TameenkConfig>().SingleInstance();
            }
            // Register the engine itself.
            builder.RegisterInstance(this).As<IEngine>().SingleInstance();
            // Register the type finder.
            builder.RegisterInstance(typeFinder).As<ITypeFinder>().SingleInstance();

            // Register dependencies provided by other assemblies
            var drTypes = typeFinder.FindClassesOfType<IDependencyRegistrar>();
            var drInstances = new List<IDependencyRegistrar>();
            foreach (var drType in drTypes)
                drInstances.Add((IDependencyRegistrar)Activator.CreateInstance(drType));
            // Sort the dependancy registrars.
            drInstances = drInstances.AsQueryable().OrderBy(t => t.Order).ToList();
            // Invoke "Register" method in each dependancy registrar.
            foreach (var dependencyRegistrar in drInstances)
                dependencyRegistrar.Register(builder, typeFinder, config);
            return builder.Build();

        }


        #endregion

        #region Methods

        /// <summary>
        /// Initialize components and plugins in the Tameenk environment.
        /// </summary>
        /// <param name="config">Config</param>
        public void Initialize(TameenkConfig config)
        {
            //register dependencies
            RegisterDependencies(config);

            //startup tasks
            if (!config.IgnoreStartupTasks)
            {
                RunStartupTasks();
            }

        }

        /// <summary>
        /// Initialize components and plugins in the Tameenk environment.
        /// </summary>
        /// <param name="config">Config</param>
        public void InitializeWebApi(HttpConfiguration httpConfig, TameenkConfig config)
        {
            //register dependencies
            RegisterApiDependencies(httpConfig, config);

            //startup tasks
            if (config != null && !config.IgnoreStartupTasks)
            {
                RunStartupTasks();
            }

        }

        /// <summary>
        /// Resolve dependency
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <returns></returns>
        public T Resolve<T>() where T : class
        {
            return ContainerManager.Resolve<T>();
        }

        /// <summary>
        ///  Resolve dependency
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns></returns>
        public object Resolve(Type type)
        {
            return ContainerManager.Resolve(type);
        }

        /// <summary>
        /// Resolve dependencies
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <returns></returns>
        public T[] ResolveAll<T>()
        {
            return ContainerManager.ResolveAll<T>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Container manager
        /// </summary>
        public ContainerManager ContainerManager
        {
            get { return _containerManager; }
        }

        #endregion
    }
}
