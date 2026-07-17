using MediatR;
using Microsoft.Extensions.Logging;
using CareerFlow.Application.Features.Certificates.Commands;
using CareerFlow.Application.Features.Certificates.DTOs;
using CareerFlow.Application.Features.Certificates.Queries;
using CareerFlow.Core.Entities;
using CareerFlow.Core.Exceptions;
using CareerFlow.Core.Interfaces;

namespace CareerFlow.Application.Features.Certificates.Handlers;

public class CreateCertificateHandler : IRequestHandler<CreateCertificateCommand, CertificateResponse>
{
    private readonly ICertificateRepository _repo;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCertificateHandler(ICertificateRepository repo, ICurrentUserService currentUser, IUnitOfWork unitOfWork)
    {
        _repo = repo; _currentUser = currentUser; _unitOfWork = unitOfWork;
    }

    public async Task<CertificateResponse> Handle(CreateCertificateCommand cmd, CancellationToken ct)
    {
        var personId = await _currentUser.GetPersonIdAsync(ct);
        if (await _repo.CountByPersonIdAsync(personId, ct) >= 30)
            throw new DomainException("Limite máximo de 30 certificados atingido");

        var cert = Certificate.Create(personId, cmd.Title, cmd.Issuer, cmd.IssueDate,
            cmd.ExpirationDate, cmd.CertificateId, cmd.CredentialId, cmd.CredentialUrl);

        await _repo.AddAsync(cert, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return MapToResponse(cert);
    }

    private static CertificateResponse MapToResponse(Certificate c) => new()
    {
        Id = c.Id,
        Title = c.Title,
        Issuer = c.Issuer,
        IssueDate = c.IssueDate,
        ExpirationDate = c.ExpirationDate,
        CertificateId = c.CertificateId,
        CredentialId = c.CredentialId,
        CredentialUrl = c.CredentialUrl,
        IsActive = c.IsActive,
        DisplayOrder = c.DisplayOrder,
        CreatedAt = c.CreatedAt
    };
}

public class GetCertificatesHandler : IRequestHandler<GetCertificatesQuery, IReadOnlyList<CertificateResponse>>
{
    private readonly ICertificateRepository _repo;
    private readonly ICurrentUserService _currentUser;

    public GetCertificatesHandler(ICertificateRepository repo, ICurrentUserService currentUser)
    {
        _repo = repo; _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<CertificateResponse>> Handle(GetCertificatesQuery req, CancellationToken ct)
    {
        var personId = await _currentUser.GetPersonIdAsync(ct);
        var certs = await _repo.GetByPersonIdAsync(personId, ct);
        return certs.Select(c => new CertificateResponse
        {
            Id = c.Id,
            Title = c.Title,
            Issuer = c.Issuer,
            IssueDate = c.IssueDate,
            ExpirationDate = c.ExpirationDate,
            CertificateId = c.CertificateId,
            CredentialId = c.CredentialId,
            CredentialUrl = c.CredentialUrl,
            IsActive = c.IsActive,
            DisplayOrder = c.DisplayOrder,
            CreatedAt = c.CreatedAt
        }).ToList().AsReadOnly();
    }
}

public class DeleteCertificateHandler : IRequestHandler<DeleteCertificateCommand>
{
    private readonly ICertificateRepository _repo;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCertificateHandler(ICertificateRepository repo, ICurrentUserService currentUser, IUnitOfWork unitOfWork)
    {
        _repo = repo; _currentUser = currentUser; _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteCertificateCommand cmd, CancellationToken ct)
    {
        var personId = await _currentUser.GetPersonIdAsync(ct);
        var cert = await _repo.GetByIdAsync(cmd.Id, ct) ?? throw new NotFoundException("Certificado", cmd.Id);
        if (cert.PersonId != personId) throw new UnauthorizedException();
        _repo.Delete(cert);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}

public class UpdateCertificateHandler : IRequestHandler<UpdateCertificateCommand, CertificateResponse>
{
    private readonly ICertificateRepository _repo;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCertificateHandler(ICertificateRepository repo, ICurrentUserService currentUser, IUnitOfWork unitOfWork)
    {
        _repo = repo; _currentUser = currentUser; _unitOfWork = unitOfWork;
    }

    public async Task<CertificateResponse> Handle(UpdateCertificateCommand cmd, CancellationToken ct)
    {
        var personId = await _currentUser.GetPersonIdAsync(ct);
        var cert = await _repo.GetByIdAsync(cmd.Id, ct) ?? throw new NotFoundException("Certificado", cmd.Id);
        if (cert.PersonId != personId) throw new UnauthorizedException();

        cert.Update(cmd.Title, cmd.Issuer, cmd.IssueDate, cmd.ExpirationDate, cmd.CertificateId, cmd.CredentialUrl);

        _repo.Update(cert);
        await _unitOfWork.SaveChangesAsync(ct);

        return new CertificateResponse
        {
            Id = cert.Id,
            Title = cert.Title,
            Issuer = cert.Issuer,
            IssueDate = cert.IssueDate,
            ExpirationDate = cert.ExpirationDate,
            CertificateId = cert.CertificateId,
            CredentialId = cert.CredentialId,
            CredentialUrl = cert.CredentialUrl,
            IsActive = cert.IsActive,
            DisplayOrder = cert.DisplayOrder,
            CreatedAt = cert.CreatedAt
        };
    }
}
