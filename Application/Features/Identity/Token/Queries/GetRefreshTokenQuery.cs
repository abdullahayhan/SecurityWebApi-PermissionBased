using Common.Requests.Identity;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Identity.Token.Queries;

public record class GetRefreshTokenQuery(RefreshTokenRequest RefreshTokenRequest) : IRequest<IResponseWrapper>;