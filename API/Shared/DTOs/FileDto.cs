using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLife.Shared.DTOs
{
    public record FileDto
    (   
        string FileName,
        ulong FileSize = 0u,
        string FileHash = "",
        DateTime UploadTime = default
    );
}
