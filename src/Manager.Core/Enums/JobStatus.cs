namespace Manager.Core.Enums;

public enum JobStatus
{
    Pending,
    Queued,
    Running,
    Succeeded,
    Failed,
    Retrying,
    Canceled
}
