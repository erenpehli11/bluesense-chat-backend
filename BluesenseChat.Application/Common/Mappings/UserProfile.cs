using AutoMapper;
using BluesenseChat.Application.DTOs;
using BluesenseChat.Domain.Entities;

namespace BluesenseChat.Application.Common.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UpdateUserProfileDto, User>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}