using AutoMapper;
using CareerFlow.Application.DTOs;
using CareerFlow.Domain.Entities;
using CareerFlow.Domain.Enums;

namespace CareerFlow.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User mappings
        CreateMap<User, UserDto>();
        CreateMap<CreateUserDto, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

        // Professional Summary mappings
        CreateMap<ProfessionalSummary, ProfessionalSummaryDto>();
        CreateMap<CreateProfessionalSummaryDto, ProfessionalSummary>();
        CreateMap<UpdateProfessionalSummaryDto, ProfessionalSummary>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // Skill mappings
        CreateMap<Skill, SkillDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.Name))
            .ForMember(dest => dest.Level, opt => opt.MapFrom(src => src.Level.Name));

        CreateMap<CreateSkillDto, Skill>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src =>
                SkillType.TryFromName(src.Type, out var type) ? type : SkillType.TOOLS))
            .ForMember(dest => dest.Level, opt => opt.MapFrom(src =>
                SkillLevel.TryFromName(src.Level, out var level) ? level : SkillLevel.BASIC));

        // Professional Experience mappings
        CreateMap<ProfessionalExperience, ProfessionalExperienceDto>()
            .ForMember(dest => dest.IsCurrent, opt => opt.MapFrom(src => src.IsCurrent))
            .ForMember(dest => dest.Skills, opt => opt.MapFrom(src =>
                src.SkillExperiences.Select(se => se.Skill)));

        CreateMap<CreateProfessionalExperienceDto, ProfessionalExperience>()
            .ForMember(dest => dest.SkillExperiences, opt => opt.Ignore());

        // Academic Background mappings
        CreateMap<AcademicBackground, AcademicBackgroundDto>()
            .ForMember(dest => dest.Level, opt => opt.MapFrom(src => src.Level.Name))
            .ForMember(dest => dest.IsCurrent, opt => opt.MapFrom(src => src.IsCurrent));

        CreateMap<CreateAcademicBackgroundDto, AcademicBackground>()
            .ForMember(dest => dest.Level, opt => opt.MapFrom(src =>
                EducationLevel.TryFromName(src.Level, out var level) ? level : EducationLevel.GRADUATION));

        // Certificate mappings
        CreateMap<Certificate, CertificateDto>()
            .ForMember(dest => dest.IsValid, opt => opt.MapFrom(src => src.IsValid));

        CreateMap<CreateCertificateDto, Certificate>();

        // Language mappings
        CreateMap<Language, LanguageDto>()
            .ForMember(dest => dest.Level, opt => opt.MapFrom(src => src.Level.Name));

        CreateMap<CreateLanguageDto, Language>()
            .ForMember(dest => dest.Level, opt => opt.MapFrom(src =>
                LanguageLevel.TryFromName(src.Level, out var level) ? level : LanguageLevel.BASIC));

        // Social Media mappings
        CreateMap<SocialMedia, SocialMediaDto>();
        CreateMap<CreateSocialMediaDto, SocialMedia>();

        // Dashboard mappings
        CreateMap<Skill, SkillDistributionDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.Name))
            .ForMember(dest => dest.Count, opt => opt.MapFrom(src => 1));
    }
}