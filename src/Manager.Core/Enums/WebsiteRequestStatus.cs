namespace Manager.Core.Enums;

public enum WebsiteRequestStatus
{
    Received = 0,
    Generating = 1,
    ReadyForReview = 2,
    Published = 3,
    Failed = 4,
    Cancelled = 5
}
