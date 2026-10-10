using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AqLife.Application.BackServices
{
    public interface IFilePublishService
    {
        Task<Guid> PublishAsync(Guid guid,CancellationToken ct);
        Task<int> PublishAsync(IEnumerable<Guid> guids, CancellationToken ct = default);
    }
}
