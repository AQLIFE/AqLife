using AqLife.Application.Abstractions.Persistence; using AqLife.Application.Validators; using AqLife.Shared.Exceptions;
namespace AqLife.Application.Business.Todo.Validator;
public abstract class TodoValidatorBase<T>(IApplicationDbContext db,string errorMessage):AbstractValidator<T>{
protected IApplicationDbContext Db{get;}=db; private Exception? failure;
private protected override string ErrorMessage{get;init;}=errorMessage;
protected bool Reject(Exception ex){failure=ex;return false;}
private protected override Exception CreateException(string message)=>failure??new DomainLegalityException(message);
}