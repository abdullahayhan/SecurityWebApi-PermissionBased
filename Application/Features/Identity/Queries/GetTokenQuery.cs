using Common.Requests;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Identity.Queries;

public record class GetTokenQuery(TokenRequest TokenRequest) : IRequest<IResponseWrapper>;