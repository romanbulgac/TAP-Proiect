// filepath: /Users/romanbulgac/Documents/University/Semestru VI/TAP/Proiect/BusinessLayer/Mapping/MappingProfile.cs
using AutoMapper;
using DataAccess.Models;
using BusinessLayer.DTOs;
using BusinessLayer.ModelDTOs;

namespace BusinessLayer.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User mappings
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));
            
            CreateMap<Teacher, UserDto>()
                .IncludeBase<User, UserDto>()
                .ForMember(dest => dest.UserType, opt => opt.MapFrom(src => "Teacher"));
                
            CreateMap<Student, UserDto>()
                .IncludeBase<User, UserDto>()
                .ForMember(dest => dest.UserType, opt => opt.MapFrom(src => "Student"));
                
            CreateMap<Admin, UserDto>()
                .IncludeBase<User, UserDto>()
                .ForMember(dest => dest.UserType, opt => opt.MapFrom(src => "Admin"));
            
            // Consultation mappings
            CreateMap<Consultation, ConsultationDto>()
                .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => $"{src.Teacher.Name} {src.Teacher.Surname}"))
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => 
                    src.StudentLinks.Any() && src.StudentLinks.First().Student != null 
                        ? $"{src.StudentLinks.First().Student.Name} {src.StudentLinks.First().Student.Surname}" 
                        : "Not Assigned"))
                .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => 
                    src.StudentLinks.Any() ? src.StudentLinks.First().StudentId : (Guid?)null))
                .ForMember(dest => dest.Materials, opt => opt.MapFrom(src => src.Materials))
                .ForMember(dest => dest.ScheduledAt, opt => opt.MapFrom(src => src.ScheduledAt))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Topic))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.ConsultationStatus.ToString()));
                
            CreateMap<ConsultationDto, Consultation>()
                .ForMember(dest => dest.TeacherId, opt => opt.MapFrom(src => src.TeacherId))
                .ForMember(dest => dest.StudentLinks, opt => opt.Ignore())
                .ForMember(dest => dest.Materials, opt => opt.Ignore())
                .ForMember(dest => dest.ScheduledAt, opt => opt.MapFrom(src => src.ScheduledAt))
                .ForMember(dest => dest.Topic, opt => opt.MapFrom(src => src.Title));
                // Material mappings
            CreateMap<Material, MaterialDto>();
            CreateMap<MaterialDto, Material>();
            
            // Notification mappings
            CreateMap<Notification, NotificationDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.Name} {src.User.Surname}"))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.NotificationType));
            CreateMap<NotificationDto, Notification>()
                .ForMember(dest => dest.NotificationType, opt => opt.MapFrom(src => src.Type));
            
            // Review mappings
            CreateMap<Review, ReviewDto>()
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => $"{src.Student.Name} {src.Student.Surname}"))
                .ForMember(dest => dest.ConsultationTitle, opt => opt.MapFrom(src => src.Consultation.Topic))
                .ForMember(dest => dest.ReviewDate, opt => opt.MapFrom(src => src.CreatedAt));
            CreateMap<ReviewDto, Review>();
            
            // TestModel mappings for repository pattern example
            CreateMap<TestModel, TestModelDto>();
            CreateMap<TestModelDto, TestModel>()
                .ForMember(dest => dest.Id, opt => opt.Condition((src, dest) => src.Id.HasValue))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id ?? Guid.Empty));
        }
    }
}
