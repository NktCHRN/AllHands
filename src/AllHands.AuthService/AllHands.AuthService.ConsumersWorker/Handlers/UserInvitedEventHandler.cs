using AllHands.Auth.Contracts.Messaging;
using AllHands.AuthService.Application.Abstractions;
using AllHands.AuthService.Application.Features.User.Create;

namespace AllHands.AuthService.ConsumersWorker.Handlers;

public sealed class UserInvitedEventHandler(IEmailSender emailSender)
{
    public async Task Handle(UserInvitedEvent @event, CancellationToken cancellationToken)
    {
        await emailSender.SendCompleteRegistrationEmailAsync(
            new SendCompleteRegistrationEmailCommand(
                @event.Email,
                @event.FirstName,
                @event.AdminName,
                @event.InvitationId,
                @event.Token), 
            cancellationToken);
    }
}
