using Common.Requests.Identity;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Identity.Token.Queries;

public record class GetTokenQuery(TokenRequest TokenRequest) : IRequest<IResponseWrapper>;