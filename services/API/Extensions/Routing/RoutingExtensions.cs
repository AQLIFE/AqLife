using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.DependencyInjection;

namespace AqLife.Extensions.Routing;

public static class RoutingExtensions
{
    public static IServiceCollection AddRouteAdapter(
        this IServiceCollection services)
    {
        services
            .AddControllers(options =>
            {
                options.Conventions.Add(
                    new ApiPrefixConvention("api"));
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(
                    new StrictEnumJsonConverterFactory());
            });

        return services;
    }
}

public sealed class ApiPrefixConvention(string prefix)
    : IControllerModelConvention
{
    public void Apply(ControllerModel controller)
    {
        foreach (var selector in controller.Selectors)
        {
            if (selector.AttributeRouteModel is not null)
            {
                selector.AttributeRouteModel =
                    AttributeRouteModel.CombineAttributeRouteModel(
                        new AttributeRouteModel(
                            new Microsoft.AspNetCore.Mvc.RouteAttribute(prefix)),
                        selector.AttributeRouteModel);
            }
            else
            {
                selector.AttributeRouteModel =
                    new AttributeRouteModel(
                        new Microsoft.AspNetCore.Mvc.RouteAttribute(prefix));
            }
        }
    }
}
