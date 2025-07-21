using AutoMapper;
using BluesenseChat.Application.DTOs;
using BluesenseChat.Domain.Entities;

namespace BluesenseChat.Application.Common.Mappings
{
    public class GroupProfile : Profile
    {
        public GroupProfile()
        {
            CreateMap<UpdateGroupDto, Group>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<GroupMember, GroupMemberDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username));

            CreateMap<Group, GroupSummaryDto>()
                .ForMember(dest => dest.GroupId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Members, opt => opt.MapFrom(src => src.Members));
        }
    }
}
