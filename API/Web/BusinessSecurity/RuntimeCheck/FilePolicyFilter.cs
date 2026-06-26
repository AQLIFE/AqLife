using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using MyLife.Shared.Options;

namespace MyLife.Web.BusinessSecurity.RuntimeCheck
{
    public enum PolicyKey { Upload, Download, Unkown }
    public class FileUploadFilter(IOptions<FilePolicyOption> options) : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            bool validStatus = true;

            string errorMsg = "Upload功能检查未通过";
            if (options is not null)
            {
                if (options.Value.AllowedUpload == null || options.Value.AllowedUpload.Length == 0)
                {
                    errorMsg = "上传功能已关闭";
                    validStatus = false;
                }
            }
            else validStatus = false;

            if (!validStatus)
            {
                context.Result = new ObjectResult(new
                {
                    message = errorMsg
                })
                { StatusCode = 403 };
            }
        }
    }
}