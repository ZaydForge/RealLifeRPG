using AutoMapper;
using TaskManagement.Dtos;
using TaskManagement.Entities;

namespace TaskManagement.Maps;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<TaskItem, TaskDto>()
            .ForMember(dest => dest.Category,
                       opt => opt.MapFrom(src => src.Category.ToString()));
        CreateMap<AppUser, UserDto>();
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
    }
}
