using AqLife.Application.Abstractions.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AqLife.Application.Abstractions.Pipeline.Markers
{
    /// <summary>
    /// 标识接口 , 自动触发 开发计划存档 规则检查 : 存档即不允许任何 ICommand
    /// </summary>
    public interface IRequireDevPlan
    {
        public Guid PlanId { get; init; }
    }
    /// <summary>
    /// 标识接口 , 在事务中会自动更新依赖图版本;同时该接口的集成还包含了 IRequireDevPlan 的检查业务
    /// </summary>
    public interface IRequireDependencyVersionIncrement : IRequireDevPlan, IRequireTransaction { }
}
