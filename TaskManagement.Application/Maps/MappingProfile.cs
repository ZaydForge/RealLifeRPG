using AutoMapper;
using System.Diagnostics;
using TaskManagement.Application.Dtos;
using TaskManagement.Application.Services;
using TaskManagement.Domain.Entities;
using TaskManagement.Dtos;
using TaskManagement.Entities;

namespace TaskManagement.Maps;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<TaskItem, TaskDto>()
            .ForMember(dest => dest.Category,
                       opt => opt.MapFrom(src => src.Category.ToString()))
           .ForMember(dest => dest.Status,
                       opt => opt.MapFrom(src => src.Status.ToString()))
           .ForMember(dest => dest.ExpiresAt,
                       opt => opt.MapFrom(src => src.ExpiresAt.Date))
           .ForMember(dest => dest.ExpirationType,
                       opt => opt.MapFrom(src => src.ExpirationType.ToString()));

        CreateMap<UserProfile, UserProfileDto>();
        CreateMap<UpdateUserProfileDto, UserProfile>()
    .ForMember(dest => dest.ProfilePictureUrl, opt => opt.Ignore());

        CreateMap<TaskLog, TaskLogDto>()
            .ForMember(dest => dest.Category,
                       opt => opt.MapFrom(src => src.Category.ToString()));
        CreateMap<TaskLogDto, TaskLog>();
        CreateMap<CategoryLevel, CategoryLevelDto>();
        CreateMap<Archive, ArchiveDto>()
            .ForMember(dest => dest.Category,
                       opt => opt.MapFrom(src => src.Category.ToString()));

        CreateMap<CreateTaskDto, TaskItem>();
        CreateMap<UpdateTaskDto, TaskItem>();

        CreateMap<Achievement, AchievementDto>();
        CreateMap<Title, TitleDto>();

        CreateMap<UserAchievement, UserAchievementDto>()
            .ForMember(dest => dest.UserName,
                       opt => opt.MapFrom(src => src.User.Username))
            .ForMember(dest => dest.Name,
                       opt => opt.MapFrom(src => src.Achievement.Name));

        CreateMap<UserTitle, UserTitleDto>()
            .ForMember(dest => dest.UserName,
                       opt => opt.MapFrom(src => src.User.Username))
            .ForMember(dest => dest.Name,
                       opt => opt.MapFrom(src => src.Title.Name));
    }
}
