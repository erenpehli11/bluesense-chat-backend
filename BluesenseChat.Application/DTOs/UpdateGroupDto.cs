using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluesenseChat.Application.DTOs
{
    public class UpdateGroupDto
    {
        public Guid GroupId { get; set; }
        public string? Name { get; set; }
        public bool IsPrivate { get; set; }
    }

}
