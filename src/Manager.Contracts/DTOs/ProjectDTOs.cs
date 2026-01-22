using Manager.Core.Enums;

namespace Manager.Contracts.DTOs;

public record CreateProjectRequest(
    string Name,
    Guid ClientId,
    ProjectType Type,
    string? Description
);

public record UpdateProjectRequest(
    string? Name,
    string? Description,
    string? Domain
);

public record ProjectResponse(
    Guid Id,
    string Name,
    Guid ClientId,
    ProjectType Type,
    ProjectStatus Status,
    string? Description,
    string? Domain,
    string? RepositoryUrl,
    string? DeploymentUrl,
    DateTime CreatedAt
);

public record ProjectDetailResponse(
    Guid Id,
    string Name,
    Guid ClientId,
    ProjectType Type,
    ProjectStatus Status,
    string? Description,
    string? Domain,
    string? RepositoryUrl,
    string? DeploymentUrl,
    DateTime CreatedAt,
    ProjectSpecResponse? Spec
);

public record ProjectSpecResponse(
    string Brand,
    string? Vertical,
    string? Goal
);
