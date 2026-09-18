using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AqLife.Application.Services
{
    public interface IBlogPublishService
    {
        Task<Guid> PublishAsync(Guid guid,CancellationToken ct);
    }
}
