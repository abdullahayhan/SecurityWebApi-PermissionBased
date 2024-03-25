using Common.Requests.Identity;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Identity.Queries;

public record class GetRefreshTokenQuery (RefreshTokenRequest RefreshTokenRequest) : IRequest<IResponseWrapper>;