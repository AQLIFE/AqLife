using AqLife.Application.File;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AqLife.Application.File.Service
{
    public interface IResumableFileUploadService
    {
        Task<ResumableUploadSession> StartAsync(StartVideoUploadRequest request, CancellationToken ct);
        Task<ResumableUploadStatus> GetStatusAsync(Guid id, CancellationToken ct);
        Task WriteChunkAsync(Guid id, int index, IFormFile chunk, CancellationToken ct);
        Task<CompletedResumableUpload> AssembleAsync(Guid id, CancellationToken ct);
        Task DeleteSessionAsync(Guid id);
    }
}
