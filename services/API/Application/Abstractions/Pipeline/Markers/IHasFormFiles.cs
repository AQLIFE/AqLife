using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AqLife.Application.Abstractions.Pipeline.Markers
{
    /// <summary>
    /// 标识接口, 标识启用UploadContext
    /// </summary>
    public interface IHasFormFiles
    {
        public IEnumerable<IFormFile> GetFiles();
    }
}
