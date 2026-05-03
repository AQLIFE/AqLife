using Microsoft.AspNetCore.Mvc.ApplicationModels;
namespace MyLife.Core.Provider
{
    public class ApiPrefixConvention : IControllerModelConvention
    {
        private readonly string _prefix;

        public ApiPrefixConvention(string prefix)
        {
            _prefix = prefix;
        }

        public void Apply(ControllerModel controller)
        {
            foreach (var selector in controller.Selectors)
            {
                // 如果已经有路由了，就在前面拼上 api
                if (selector.AttributeRouteModel != null)
                {
                    selector.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(
                        new AttributeRouteModel(new Microsoft.AspNetCore.Mvc.RouteAttribute(_prefix)),
                        selector.AttributeRouteModel);
                }
                else
                {
                    // 如果没有路由，就直接设为 api/[controller]
                    selector.AttributeRouteModel = new AttributeRouteModel(new Microsoft.AspNetCore.Mvc.RouteAttribute(_prefix));
                }
            }
        }
    }
}
