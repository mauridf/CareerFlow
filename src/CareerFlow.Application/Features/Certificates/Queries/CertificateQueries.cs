using MediatR;
using CareerFlow.Application.Features.Certificates.DTOs;

namespace CareerFlow.Application.Features.Certificates.Queries;

public record GetCertificatesQuery : IRequest<IReadOnlyList<CertificateResponse>>;
