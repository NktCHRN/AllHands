using AllHands.Auth.Contracts.Messaging;
using AllHands.AuthService.Application.Abstractions;
using AllHands.AuthService.Application.Features.User.ResetPassword;

namespace AllHands.AuthService.ConsumersWorker.Handlers;

public sealed class ResetPasswordRequestedEventHandler(IEmailSender emailSender)
{
    public async Task Handle(ResetPasswordRequestedEvent @event, CancellationToken cancellationToken)
    {
        await emailSender.SendResetPasswordEmailAsync(new SendResetPasswordEmailCommand(@event.Email, @event.FirstName, @event.Token), cancellationToken);
    }
}
