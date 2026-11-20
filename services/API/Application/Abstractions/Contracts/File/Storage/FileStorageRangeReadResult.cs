using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AqLife.Application.Abstractions.Contracts.File.Storage
{
    public sealed record FileStorageRangeReadResult(
        Stream Stream,
        long TotalLength,
        long Start,
        long End)
    {
        public long ContentLength => End - Start + 1;
    }
}
