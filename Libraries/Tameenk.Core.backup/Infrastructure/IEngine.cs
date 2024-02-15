using System;
using Tameenk.Core.Configuration;
using Tameenk.Core.Infrastructure.DependencyManagement;

namespace Tameenk.Core.Infrastructure
{
    /// <summary>
    /// Classes implementing this interface can serve as a portal for the 
    /// various services composing the Tameenk engine. Edit functionality, modules
    /// and implementations access most Tameenk functionality through this 
    /// interface.
    /// </summary>
    public interface IEngine
    {
        /// <summary>
        /// Container manager
        /// </summary>
        ContainerManager ContainerManager { get; }

        /// <summary>
        /// Initialize components and plugins in the Tameenk environment.
        /// </summary>
        /// <param name="config">Config</param>
        void Initialize(TameenkConfig config);


        /// <summary>
        /// Initialize components and plugins in the Tameenk environment for web api
        /// </summary>
        /// <param name="config">Config</param>
        /// <param name="httpConfig">httpConfig</param>
        void InitializeWebApi(HttpConfiguration httpConfig, TameenkConfig config);

        /// <summary>
        /// Resolve dependency
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <returns></returns>
        T Resolve<T>() where T : class;

        /// <summary>
        ///  Resolve dependency
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns></returns>
        object Resolve(Type type);

        /// <summary>
        /// Resolve dependencies
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <returns></returns>
        T[] ResolveAll<T>();
    }
}
