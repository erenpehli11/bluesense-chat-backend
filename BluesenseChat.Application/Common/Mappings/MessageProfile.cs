using AutoMapper;
using BluesenseChat.Application.DTOs;
using BluesenseChat.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluesenseChat.Application.Common.Mappings
{
    public class MessageProfile : Profile
    {
        public MessageProfile()
        {
            CreateMap<Attachment, AttachmentDto>();

            CreateMap<Message, MessageDto>()
                .ForMember(dest => dest.SenderUsername, opt => opt.MapFrom(src => src.Sender.Username))
                .ForMember(dest => dest.SentAt, opt => opt.MapFrom(src => src.CreatedAt));
        }
    }

}
