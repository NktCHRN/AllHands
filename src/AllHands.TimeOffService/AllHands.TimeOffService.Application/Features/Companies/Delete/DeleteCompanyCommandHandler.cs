using AllHands.Shared.Domain.Exceptions;
using AllHands.TimeOffService.Domain.Models;
using Marten;
using MediatR;

namespace AllHands.TimeOffService.Application.Features.Companies.Delete;

public sealed class DeleteCompanyCommandHandler(IDocumentSession documentSession) : IRequestHandler<DeleteCompanyCommand>
{
    public async Task Handle(DeleteCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = await documentSession.Query<Company>()
                           .FirstOrDefaultAsync(e => e.Id == request.Id, token: cancellationToken)
                       ?? throw new EntityNotFoundException("Company not found");
        
        documentSession.Delete(company);
        await documentSession.SaveChangesAsync(cancellationToken);
    }
}
