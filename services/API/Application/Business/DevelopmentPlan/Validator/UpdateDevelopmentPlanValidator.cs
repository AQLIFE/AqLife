using AqLife.Domain.Command;
using AqLife.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Business.DevelopmentPlan.Validator;
public sealed class UpdateDevelopmentPlanValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db):DevelopmentPlanValidatorBase<UpdateDevelopmentPlanCommand>(db,"开发计划不存在或不可修改"){
    private protected override async Task<bool> IsValidAsync(UpdateDevelopmentPlanCommand q,CancellationToken ct){
        var p=await Db.DevelopmentPlans.FirstOrDefaultAsync(x=>x.UID==q.UID,ct);
        if(p is null)return Reject(new ResourceNotFoundException("开发计划不存在"));
        if(p.IsArchived)return Reject(new DomainLegalityException("已归档的开发计划不允许修改"));
        if(string.IsNullOrWhiteSpace(q.Name)||q.Name.Trim().Length>128)return Reject(new DomainLegalityException("开发计划名称不能为空且不能超过 128 个字符"));
        if(q.Description?.Length>2000)return Reject(new DomainLegalityException("开发计划描述不能超过 2000 个字符"));
        return true;
    }
}
