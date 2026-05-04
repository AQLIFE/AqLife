using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace MyLife.Web.Infrastructure
{
    public static class MvcExtensions
    {
        public static IServiceCollection AddCustomApiConventions(this IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                // 在这里注册你的全局路由前缀
                options.Conventions.Add(new ApiPrefixConvention("api"));
            });
            return services;
        }
    }

    public class ApiPrefixConvention(string prefix) : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            foreach (var selector in controller.Selectors)
            {
                // 如果已经有路由了，就在前面拼上 api
                if (selector.AttributeRouteModel != null)
                {
                    selector.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(
                        new AttributeRouteModel(new Microsoft.AspNetCore.Mvc.RouteAttribute(prefix)),
                        selector.AttributeRouteModel);
                }
                else
                {
                    // 如果没有路由，就直接设为 api/[controller]
                    selector.AttributeRouteModel = new AttributeRouteModel(new Microsoft.AspNetCore.Mvc.RouteAttribute(prefix));
                }
            }
        }
    }
}