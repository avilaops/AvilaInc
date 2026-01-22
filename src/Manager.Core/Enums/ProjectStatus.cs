namespace Manager.Core.Enums;

public enum ProjectStatus
{
    Draft,
    AwaitingSpec,
    AwaitingPayment,
    QueuedProvisioning,
    ProvisioningRepo,
    Deploying,
    AwaitingDNS,
    Live,
    Suspended,
    Error
}
