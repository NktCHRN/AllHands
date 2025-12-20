namespace AllHands.Auth.Contracts.Messaging;

public static class Queues
{
    public const string CompanySessionsRecalculationRequestedEvent = "company-sessions-recalculation-requested-event";
    public const string UserSessionsRecalculationRequestedEvent = "user-sessions-recalculation-requested-event";
    public const string UserInvitedEvent = "user-invited-event";
    public const string ResetPasswordRequestedEvent = "reset-password-requested-event";
}
