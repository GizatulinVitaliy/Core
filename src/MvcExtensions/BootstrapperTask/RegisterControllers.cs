#region Copyright
// Copyright (c) 2009 - 2010, Kazi Manzur Rashid <kazimanzurrashid@gmail.com>.
// This source is subject to the Microsoft Public License. 
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL. 
// All other rights reserved.
#endregion

namespace MvcExtensions
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using JetBrains.Annotations;

    /// <summary>
    /// Defines a class which is used to register available <seealso cref="Controller"/>.
    /// </summary>
    [DependsOn(typeof(RegisterControllerActivator))]
    public class RegisterControllers : IgnorableTypesBootstrapperTask<RegisterControllers, Controller>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterControllers"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public RegisterControllers([NotNull] ContainerAdapter container)
        {
            Invariant.IsNotNull(container, "container");

            Container = container;
        }

        /// <summary>
        /// Gets the container.
        /// </summary>
        /// <value>The container.</value>
        protected ContainerAdapter Container
        {
            get;
            private set;
        }

        /// <summary>
        /// Executes the task. Returns continuation of the next task(s) in the chain.
        /// </summary>
        /// <returns></returns>
        public override TaskContinuation Execute()
        {
            Func<Type, bool> filter = type => KnownTypes.ControllerType.IsAssignableFrom(type) &&
                                              type.Assembly != KnownAssembly.AspNetMvcAssembly &&
                                              !type.Assembly.GetName().Name.Equals(KnownAssembly.AspNetMvcFutureAssemblyName, StringComparison.OrdinalIgnoreCase) &&
                                              !IgnoredTypes.Contains(type);

            Container.GetService<IBuildManager>()
                     .ConcreteTypes
                     .Where(filter)
                     .Each(type => Container.RegisterAsTransient(type));

            return TaskContinuation.Continue;
        }
    }
}