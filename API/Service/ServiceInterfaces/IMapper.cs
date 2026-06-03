namespace MyLife.Service.ServiceInterfaces
{
    public interface IGenMapper<T, DTO>
    {
        /// <summary>
        ///  负责 T => DTO 的转换，主要用于脱敏
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        DTO ToDto(T obj);

        /// <summary>
        /// 负责 DTO => T 的快速转换，主要用于快速组装 T 对象
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        T ToEntity(DTO dto);
        void UpdateEntity(DTO dto, T entity);
    }
}
