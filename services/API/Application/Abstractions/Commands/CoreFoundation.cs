using MediatR;

namespace AqLife.Application.Abstractions.Commands;

// ==========================================
// 1. 核心 CQRS 架构基石 (Core Foundation)
// ==========================================
public interface IApplicationRequest { }
public interface IQuery<out TResponse> : IRequest<TResponse>, IApplicationRequest { }

/// <summary>
/// 顶层命令接口（留作后路：允许自定义任意返回类型）
/// </summary>
public interface ICommand<out TResponse> : IRequest<TResponse>, IRequireTransaction, IApplicationRequest { }

/// <summary>
/// 默认命令接口（主力军：强制统一返回 Guid，消灭 90% 场景的泛型噪声）
/// </summary>
public interface ICommand : ICommand<Guid> { }