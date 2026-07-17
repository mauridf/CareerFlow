using CareerFlow.Core.Entities;
using CareerFlow.Core.Enums;
using CareerFlow.Application.Features.Resume.DTOs;
using CareerFlow.Core.Interfaces;

namespace CareerFlow.Application.Features.Resume.Handlers;

internal static class ResumeMapper
{
    public static ResumeData MapToResumeData(Person p) => new()
    {
        PersonName = p.User?.Name ?? "",
        Email = p.User?.Email ?? "",
        Phone = p.Phone,
        City = p.City,
        State = p.State,
        ProfessionalSummary = p.ProfessionalSummary,
        PhotoUrl = p.PhotoUrl,
        CurrentPosition = p.CurrentPosition,
        CurrentCompany = p.CurrentCompany,
        Experiences = p.Experiences.Select(e => new ResumeExperienceData
        {
            CompanyName = e.CompanyName,
            Position = e.Position,
            StartDate = e.StartDate,
            EndDate = e.EndDate,
            IsCurrent = e.IsCurrent,
            Description = e.Description,
            EmploymentType = e.EmploymentType?.GetDisplayName(),
            City = e.City,
            State = e.State,
            DurationFormatted = e.GetFormattedDuration()
        }).ToList(),
        Educations = p.Educations.Select(e => new ResumeEducationData
        {
            Institution = e.Institution,
            Course = e.Course,
            Level = e.EducationLevel.GetDisplayName(),
            StartDate = e.StartDate,
            EndDate = e.EndDate,
            Status = e.Status.GetDisplayName()
        }).ToList(),
        Skills = p.Skills.Select(s => new ResumeSkillData
        {
            Name = s.Name,
            Category = s.Category.GetDisplayName(),
            Level = s.ProficiencyLevel.GetDisplayName(),
            IsPrimary = s.IsPrimary
        }).ToList(),
        Languages = p.Languages.Select(l => new ResumeLanguageData
        {
            LanguageName = l.LanguageName,
            Level = l.ProficiencyLevel.GetDisplayName(),
            IsNative = l.IsNative
        }).ToList(),
        Certificates = p.Certificates.Select(c => new ResumeCertificateData
        {
            Title = c.Title,
            Issuer = c.Issuer,
            IssueDate = c.IssueDate,
            ExpirationDate = c.ExpirationDate,
            CredentialUrl = c.CredentialUrl
        }).ToList(),
        SocialNetworks = p.SocialNetworks.Select(s => new ResumeSocialNetworkData
        {
            NetworkType = s.NetworkType.GetDisplayName(),
            Url = s.Url
        }).ToList()
    };

    public static ResumeResponse MapToResume(Person p) => new()
    {
        Person = new PersonInfo
        {
            Name = p.User?.Name ?? "",
            Email = p.User?.Email ?? "",
            Phone = p.Phone,
            City = p.City,
            State = p.State,
            ProfessionalSummary = p.ProfessionalSummary,
            PhotoUrl = p.PhotoUrl,
            CurrentPosition = p.CurrentPosition,
            CurrentCompany = p.CurrentCompany,
            ResumeSlug = p.ResumeSlug
        },
        Experiences = p.Experiences.Select(e => new ExperienceInfo
        {
            CompanyName = e.CompanyName,
            Position = e.Position,
            StartDate = e.StartDate,
            EndDate = e.EndDate,
            IsCurrent = e.IsCurrent,
            Description = e.Description,
            EmploymentType = e.EmploymentType?.GetDisplayName(),
            DurationFormatted = e.GetFormattedDuration()
        }).ToList(),
        Educations = p.Educations.Select(e => new EducationInfo
        {
            Institution = e.Institution,
            Course = e.Course,
            Level = e.EducationLevel.GetDisplayName(),
            Status = e.Status.GetDisplayName(),
            StartDate = e.StartDate,
            EndDate = e.EndDate
        }).ToList(),
        Skills = p.Skills.Select(s => new SkillInfo
        {
            Name = s.Name,
            Category = s.Category.GetDisplayName(),
            Level = s.ProficiencyLevel.GetDisplayName(),
            Score = s.ProficiencyLevel.GetScore(),
            IsPrimary = s.IsPrimary
        }).ToList(),
        Languages = p.Languages.Select(l => new LanguageInfo
        {
            LanguageName = l.LanguageName,
            Level = l.ProficiencyLevel.GetDisplayName(),
            IsNative = l.IsNative
        }).ToList(),
        Certificates = p.Certificates.Select(c => new CertificateInfo
        {
            Title = c.Title,
            Issuer = c.Issuer,
            IssueDate = c.IssueDate,
            ExpirationDate = c.ExpirationDate,
            CredentialUrl = c.CredentialUrl
        }).ToList(),
        SocialNetworks = p.SocialNetworks.Select(s => new SocialNetworkInfo
        {
            NetworkType = s.NetworkType.GetDisplayName(),
            Url = s.Url
        }).ToList()
    };
}
