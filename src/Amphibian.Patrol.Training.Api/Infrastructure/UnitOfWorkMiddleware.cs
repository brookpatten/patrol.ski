using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Training.Api.Infrastructure
{
    public class UnitOfWorkMiddleware
    {
        private readonly RequestDelegate _next;
        private IList<string> _uowRoutes;

        public UnitOfWorkMiddleware(RequestDelegate next,IList<string> uowRoutes)
        {
            _next = next;

            _uowRoutes = uowRoutes;
        }
        public async Task Invoke(HttpContext httpContext, IUnitOfWork uow)
        {

            string key = httpContext.GetRouteData().Values["controller"] + "Controller." + httpContext.GetRouteData().Values["action"];
            bool transaction = _uowRoutes.Contains(key);
            
            if(transaction)
            {
                await uow.Begin().ConfigureAwait(false);
            }
            try
            {
                await _next(httpContext);
                if (transaction)
                {
                    await uow.Commit().ConfigureAwait(false);
                }
            }
            catch
            {
                if (transaction)
                {
                    await uow.Rollback().ConfigureAwait(false);
                }
                throw;
            }
        }
    }

    public static class UnitOfWorkMiddlewareExtensions
    {
        public static IApplicationBuilder UseUnitOfWorkMiddleware(this IApplicationBuilder builder,Assembly controllerAssembly)
        {
            //make a list of controller methods on which to execute the uow.  we do this one time so that we don't have to do slow
            //reflection calls for each request
            var uowRoutes = new List<string>();

            var controllerTypes = controllerAssembly.GetTypes()
                .AsEnumerable()
                .Where(type => typeof(ControllerBase).IsAssignableFrom(type))
                .ToList();

            foreach (var controller in controllerTypes)
            {
                var methods = controller.GetMethods();

                foreach (var method in methods)
                {
                    var uom = method.GetCustomAttribute<UnitOfWorkAttribute>();
                    if (uom != null)
                    {
                        var routeAttribute = method.GetCustomAttribute<RouteAttribute>();

                        if (routeAttribute != null)
                        {
                            var controllerName = controller.Name;
                            var actionName = method.Name;
                            uowRoutes.Add(controllerName + "." + actionName);
                        }
                    }
                }
            }
            return builder.UseMiddleware<UnitOfWorkMiddleware>(uowRoutes);
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class UnitOfWorkAttribute:Attribute
    {

    }
}
