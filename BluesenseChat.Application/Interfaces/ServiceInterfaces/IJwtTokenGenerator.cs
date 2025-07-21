using BluesenseChat.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluesenseChat.Application.Interfaces.ServiceInterfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user);
    }
}
