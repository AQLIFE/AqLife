using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLife.Shared.DTOs
{
    public record TagDto(string Name, string? AliasName=null,bool IsCategory=false);
}
