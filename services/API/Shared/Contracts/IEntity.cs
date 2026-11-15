namespace AqLife.Shared.Contracts
{
    public interface IEntity
    {
        public Guid UID { get; init; }
    }
    /// <summary>
    /// 关系模型,非必要不应该加入导航属性
    /// </summary>
    public interface IRelationship { }
}
