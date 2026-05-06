using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLife.Service.Interfaces
{
    public interface IGenericsMapper<T, DTO>
    {
        /// <summary>
        ///  负责 T => DTO 的转换，主要用于脱敏
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        DTO Desensitization(T obj);

        /// <summary>
        /// 负责 DTO => T 的快速转换，主要用于快速组装 T 对象
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        T Assembly(DTO dto);
    }
}
