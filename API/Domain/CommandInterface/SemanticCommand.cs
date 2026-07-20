using MediatR;

namespace MyLife.Domain.CommandInterface;

// ==========================================
// 2. 语义化子命令 (Semantic Sub-Commands)
// ==========================================

/// <summary> 创建命令：默认返回新资源的 Guid </summary>
public interface ICreateCommand : ICommand { }
public interface ICreateCommand<T> : ICommand<T> { }

/// <summary> 
/// 更新命令：默认返回资源的 Guid。
/// 优化：取消了接口泛型，将其降级为具体 Command 的属性，保持接口绝对干净
/// </summary>
public interface IUpdateCommand : ICommand { }
public interface IUpdateCommand<T> : ICommand<T> { }

/// <summary> 
/// 删除命令：写死返回 Unit（无返回值）。
/// 注意：这里继承顶层的 ICommand<Unit>，完美避开 Guid 冲突
/// </summary>
public interface IDeleteCommand : ICommand<Unit> { }