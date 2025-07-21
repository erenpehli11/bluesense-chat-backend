using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluesenseChat.Application.DTOs
{
    public class CreateGroupDto
    {
        public string Name { get; set; } = null!;
        public bool IsPrivate { get; set; }
    }
}
