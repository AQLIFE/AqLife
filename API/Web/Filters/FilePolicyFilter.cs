using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using MyLife.Service.Interfaces;
using MyLife.Service.Strategies;
using MyLife.Shared.Accident;
using MyLife.Shared.Options;

namespace MyLife.Web.Filters
{
    public enum PolicyKey { Upload, Download, Unkown }
    public class FileUploadFilter(IOptions<FilePolicyOption> options,IEnumerable<IUploadStrategy> Checks) : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            bool validStatus = true;

            string errorMsg = "Upload功能检查未通过";
            if (Checks is not null && options is not null)
            {
                #region upload
                if (options.Value.AllowedUpload == null || options.Value.AllowedUpload.Length == 0)
                {
                    errorMsg = "上传功能已关闭";
                    validStatus = false;
                }

                else if (context.HttpContext.Request.HasFormContentType)
                {
                    var file = context.HttpContext.Request.Form.Files.FirstOrDefault();
                    if (file != null)
                        foreach (var strategy in Checks)
                            if (strategy.Check(file) is (false, string message))
                            {
                                errorMsg = message;
                                validStatus = false; break;
                            }

                }
                #endregion


            }
            else validStatus = false;

            if( !validStatus)
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