using System.Net;
using System.Text.Json;
using AllHands.Application.Abstractions;
using AllHands.Application.Features.Employee.Create;
using AllHands.Application.Features.User.ForgotPassword;
using Amazon.SimpleEmailV2;
using Amazon.SimpleEmailV2.Model;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AllHands.Infrastructure.Email;

public sealed class EmailSender(IAmazonSimpleEmailServiceV2 ses, IOptionsMonitor<EmailSenderOptions> optionsMonitor, ILogger<EmailSender> logger) : IEmailSender
{
    public async Task SendResetPasswordEmailAsync(SendResetPasswordEmailCommand command, CancellationToken cancellationToken)
    {
        var queryParameters = new Dictionary<string, string?>()
        {
            { "token", command.Token },
            { "email", command.Email },
        };
        var fullUrl = QueryHelpers.AddQueryString(optionsMonitor.CurrentValue.ResetPasswordUrl, queryParameters);
        
        var request = new SendEmailRequest
        {
            FromEmailAddress = optionsMonitor.CurrentValue.FromEmailAddress,

            Destination = new Destination
            {
                ToAddresses = [command.Email]
            },

            Content = new EmailContent
            {
                Template = new Template()
                {
                    TemplateName = optionsMonitor.CurrentValue.ResetPasswordTemplateName,
                    TemplateData = JsonSerializer.Serialize(new
                    {
                        link = fullUrl,
                        name = command.FirstName
                    })
                }
            }
        };

        var result = await ses.SendEmailAsync(request, cancellationToken);
        if (!IsSuccess(result.HttpStatusCode))
        {
            logger.LogError("Email sending failed {Response}", result);
            throw new InvalidOperationException("Email sending failed");
        }
    }

    public async Task SendCompleteRegistrationEmailAsync(SendCompleteRegistrationEmailCommand command,
        CancellationToken cancellationToken)
    {
        var queryParameters = new Dictionary<string, string?>()
        {
            { "token", command.Token },
            { "invitationId", command.InvitationId.ToString() },
        };
        var fullUrl = QueryHelpers.AddQueryString(optionsMonitor.CurrentValue.CompleteRegistrationUrl, queryParameters);
        
        var request = new SendEmailRequest
        {
            FromEmailAddress = optionsMonitor.CurrentValue.FromEmailAddress,

            Destination = new Destination
            {
                ToAddresses = [command.Email]
            },

            Content = new EmailContent
            {
                Template = new Template()
                {
                    TemplateName = optionsMonitor.CurrentValue.CompleteRegistrationTemplateName,
                    TemplateData = JsonSerializer.Serialize(new
                    {
                        link = fullUrl,
                        name = command.FirstName,
                        adminName = command.AdminName
                    })
                }
            }
        };

        var result = await ses.SendEmailAsync(request, cancellationToken);
        if (!IsSuccess(result.HttpStatusCode))
        {
            logger.LogError("Email sending failed {Response}", result);
            throw new InvalidOperationException("Email sending failed");
        }
    }

    private static bool IsSuccess(HttpStatusCode statusCode)
        => (int)statusCode >= 200 && (int)statusCode <= 299;
}
