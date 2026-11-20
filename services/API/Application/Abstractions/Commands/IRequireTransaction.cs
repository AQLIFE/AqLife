using AqLife.Shared.Contracts;
using Microsoft.AspNetCore.Http;

namespace AqLife.Application.Abstractions.Commands;

/// <summary>
/// 标识接口,自动开启事务管理
/// </summary>
public interface IRequireTransaction { }