namespace Manager.Core.Enums;

public enum JobStatus
{
    Queued,
    Running,
    Succeeded,
    Failed,
    Retrying,
    Canceled
}
