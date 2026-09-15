using AqLife.Application.Validators;
using AqLife.Domain.Command;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AqLife.Application.Business.File.Validator
{
    //public class FileQueryValidator(IHttpContextAccessor httpContext) :AbstractValidator<FileQuery>
    //{
    //    private protected override string ErrorMessage { init; get; } = "必须提供一个搜索条件";
    //    private protected override async Task<bool> IsValidAsync(FileQuery query, CancellationToken ct)
    //    {
    //        if (httpContext.HttpContext?.User.Identity?.IsAuthenticated ==true)
    //        {
    //            return true;
    //        }
    //        return query.UID is not null || query.Title is not null; 
    //    }
    //    // UID 和 Title 至少提供一个
    //}
}
