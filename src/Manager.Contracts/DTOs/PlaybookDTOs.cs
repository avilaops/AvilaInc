using Manager.Core.Enums;

namespace Manager.Contracts.DTOs;

public record PlaybookRunResponse(
    Guid Id,
    Guid PlaybookId,
    Guid ProjectId,
    string PlaybookName,
    JobStatus Status,
    DateTime CreatedAt,
    DateTime? StartedAt,
    DateTime? CompletedAt,
    string? ErrorMessage
);

public record PlaybookRunDetailResponse(
    Guid Id,
    Guid PlaybookId,
    Guid ProjectId,
    string PlaybookName,
    JobStatus Status,
    DateTime CreatedAt,
    DateTime? StartedAt,
    DateTime? CompletedAt,
    string? ErrorMessage,
    List<StepRunResponse> Steps
);

public record StepRunResponse(
    Guid Id,
    string StepName,
    int StepOrder,
    JobStatus Status,
    DateTime? StartedAt,
    DateTime? CompletedAt,
    string? ErrorMessage
);
