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
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => GetSkillType(src.Type)))
            .ForMember(dest => dest.Level, opt => opt.MapFrom(src => GetSkillLevel(src.Level)));

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
            .ForMember(dest => dest.Level, opt => opt.MapFrom(src => GetEducationLevel(src.Level)));

        // Certificate mappings
        CreateMap<Certificate, CertificateDto>()
            .ForMember(dest => dest.IsValid, opt => opt.MapFrom(src => src.IsValid));

        CreateMap<CreateCertificateDto, Certificate>();

        // Language mappings
        CreateMap<Language, LanguageDto>()
            .ForMember(dest => dest.Level, opt => opt.MapFrom(src => src.Level.Name));

        CreateMap<CreateLanguageDto, Language>()
            .ForMember(dest => dest.Level, opt => opt.MapFrom(src => GetLanguageLevel(src.Level)));

        // Social Media mappings
        CreateMap<SocialMedia, SocialMediaDto>();
        CreateMap<CreateSocialMediaDto, SocialMedia>();

        // Dashboard mappings
        CreateMap<Skill, SkillDistributionDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.Name))
            .ForMember(dest => dest.Count, opt => opt.MapFrom(src => 1));
    }

    // Métodos auxiliares do 'out' parameter
    private SkillType GetSkillType(string typeName)
    {
        return SkillType.TryFromName(typeName, out var type) ? type : SkillType.TOOLS;
    }

    private SkillLevel GetSkillLevel(string levelName)
    {
        return SkillLevel.TryFromName(levelName, out var level) ? level : SkillLevel.BASIC;
    }

    private EducationLevel GetEducationLevel(string levelName)
    {
        return EducationLevel.TryFromName(levelName, out var level) ? level : EducationLevel.GRADUATION;
    }

    private LanguageLevel GetLanguageLevel(string levelName)
    {
        return LanguageLevel.TryFromName(levelName, out var level) ? level : LanguageLevel.BASIC;
    }
}