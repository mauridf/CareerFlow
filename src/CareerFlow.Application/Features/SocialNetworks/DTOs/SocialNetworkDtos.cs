using CareerFlow.Core.Enums;

namespace CareerFlow.Application.Features.SocialNetworks.DTOs;

public record CreateSocialNetworkRequest(SocialNetworkType NetworkType, string Url, int DisplayOrder = 0);
public record UpdateSocialNetworkRequest(SocialNetworkType NetworkType, string Url, int DisplayOrder);

public class SocialNetworkResponse
{
    public Guid Id { get; set; }
    public string NetworkType { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public DateTime CreatedAt { get; set; }
}
